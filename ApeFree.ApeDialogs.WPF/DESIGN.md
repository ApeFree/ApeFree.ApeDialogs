# ApeFree.ApeDialogs.WPF 设计文档

> ApeDialogs 的 WPF 实现库（Win11 Fluent 设计风格），同时支持 WPF 与 WinForms 宿主。
> 本文档随里程碑增量维护：架构（M1）、令牌表（M1）、控件规范（M2+）、决策记录 ADR（持续）。

## 1. 总体架构

```
┌─────────────────────────────────────────────────────────────┐
│                    消费者（WPF / WinForms 应用）              │
│   DialogResult<T> r = await provider.ShowInputDialogAsync(…);│
└──────────────────────────────┬──────────────────────────────┘
                               │ IWin32Owner（宿主句柄抽象）
┌──────────────────────────────▼──────────────────────────────┐
│  ApeDialogsWpfProvider : DialogProvider<IWin32Owner>         │
│  19 个 CreateXxxDialog（生成对应 WPF Dialog 实例）            │
│  ShowXxxDialogAsync 由标准库基类提供                          │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│  WpfXxxDialog : BaseDialog<DialogHostWindow, ApeButton,      │
│                              IWin32Owner, TResult>           │
│  · 共享宿主 DialogHostWindow（圆角/阴影/亚克力/动画/选项区）   │
│  · 内容区装配：ApeTextBox / ApeSlider / DataTemplate …       │
│  · TrySubmit → Validator → OnValidationFailed（抖动+错误条）  │
└─────────────────────────────────────────────────────────────┘

支撑层：
  Theme/     ThemeManager（深浅色跟随 + 资源切换）、Win11Theme（IDialogTheme）
             Brushes.Light.xaml / Brushes.Dark.xaml（Win11 令牌）
  Hosting/   DialogHostWindow、WindowsBackdrop（DWM 封装）、WindowsVersion
  Controls/  ApeButton …（Win11 风格控件组，M2 起）
  Interop/   IWin32Owner / WindowOwner（FormOwner 在 WinForms 桥包）
```

## 2. 包结构

| 包 | TFM | 内容 |
|---|---|---|
| `ApeFree.ApeDialogs.WPF` | `net461;net8.0-windows` | 实现库（本库） |
| `ApeFree.ApeDialogs.WPF.WinForms` | `net461;net8.0-windows` | 桥包：FormOwner + Form 扩展（M7） |
| samples（Demo） | net8.0-windows | WPF / WinForms 示例（M7） |

## 3. Win11 设计令牌（浅色 / 深色）

| 令牌 | 浅色 | 深色 |
|---|---|---|
| 窗口背景 `ApeWindowBackground` | `#F3F3F3` | `#202020` |
| 卡片背景 `ApeCardBackground` | `#F9F9F9` | `#2C2C2C` |
| 控件填充 `ApeControlFillDefault` | `#FFFFFF` | `#2C2C2C` |
| 控件填充悬停 / 按下 | `#E5E5E5` / `#D9D9D9` | `#3A3A3A` / `#454545` |
| 文字主 / 次 / 禁用 | `#1B1B1B` / `#5C5C5C` / `#AEAEAE` | `#FFFFFF` / `#D0D0D0` / `#8A8A8A` |
| 边框 `ApeStrokeDefault` | `#E5E5E5` | `#454545` |
| 强调色填充 `ApeAccentFillDefault` | `#0067C0` | `#60CDFF` |
| 强调色上文字 | `#FFFFFF` | `#000000` |
| 语义：积极 / 消极 / 特殊 | `#0F7B0F` / `#C42B1C` / `#773BDB` | `#6CCB5F` / `#FF99A4` / `#9B8FFF` |
| 圆角：覆盖层 / 控件 | 8 / 4 | 同左 |
| 字体 | Segoe UI Variable, Segoe UI | 同左 |
| 阴影 | Blur 16 / Depth 6 / Opacity 0.25 | 同左 |

## 4. 决策记录（ADR）

### ADR-001：对话框外壳采用"自绘圆角 + 自绘阴影 + 透明背景"
- **状态**：已采纳（M1）
- **背景**：DWM 系统阴影/圆角对 `WindowStyle=None` 窗口行为不确定；`AllowsTransparency` 有性能与兼容隐患。
- **决策**：`WindowStyle=None` + `ResizeMode=NoResize` + `Background=Transparent`（非分层窗口）+ RootBorder（Margin 20、CornerRadius 8、DropShadowEffect）。窗口外圈透明区域为阴影留白。
- **代价**：无系统级 resize/snap（对话框无需）；阴影为软件渲染（小窗可接受）。
- **后续**：若出现渲染问题，可切换为 DWM 圆角（`DWMWA_WINDOW_CORNER_PREFERENCE`）+ 系统阴影方案，接口已预留 `WindowsBackdrop.TrySetWindowCornerPreference`。

### ADR-002：背景材质三级回退，对话框默认 Acrylic
- **状态**：已采纳（M1）
- **决策**：Win11 22H2+ → DWM 系统亚克力（`DWMWA_SYSTEMBACKDROP_TYPE=3`，窗口背景透明 + 内容卡片自带底色）；Win10 → 纯色主题背景（`SetWindowCompositionAttribute` 路径列为后续增强）；异常 → 纯色。
- **注意**：仅当 DWM 调用返回成功才切换窗口背景为透明，避免"透明窗口 + 无亚克力"的内容不可见风险。
- Mica 仅提供 API（`BackdropKind.Mica`），供宿主应用主窗口使用，对话框不使用。

### ADR-003：上下文类型使用 `IWin32Owner{IntPtr Handle}` 抽象
- **状态**：已采纳（M1）
- **决策**：`ApeDialogsWpfProvider : DialogProvider<IWin32Owner>`；WPF 场景用 `WindowOwner`（`WindowInteropHelper`），WinForms 场景用桥包的 `FormOwner`。主 WPF 包不引用 System.Windows.Forms。
- **理由**：两个框架共用一个 Provider 类型面；桥包保持独立，主包纯净。

### ADR-004：深浅色跟随系统（注册表 + SystemEvents）
- **状态**：已采纳（M1）
- **决策**：`ThemeManager` 读取 `AppsUseLightTheme` 注册表；`SystemEvents.UserPreferenceChanged` 监听变化并 `Dispatcher.BeginInvoke` 归位 UI 线程；资源切换 = 从 Application.Resources 移除旧字典、插入新字典（DynamicResource 自动重解析）；同步 `DWMWA_USE_IMMERSIVE_DARK_MODE`（Win10 1809+）。
- **net461 注意**：`Environment.OSVersion` 被 app.manifest 限制，统一用 `RtlGetVersion`（`WindowsVersion`）。

### ADR-005：多目标策略 net461 + net8.0-windows
- **状态**：已采纳（M1）
- **决策**：兼容写法优先（不用 `Math.Clamp`/`HashCode`/`LibraryImport` 等 net461 缺失 API），`#if` 预计 ≤5 处；`init`/`record` 由 `Polyfills/IsExternalInit.cs`（`#if NETFRAMEWORK`）支撑；`LangVersion=latest`；NoWarn：`1591;CA1510;CA1707;SYSLIB1054`。
- **代价**：每 TFM 全量编译 XAML，构建时间 ×2；CI 仅在 windows-latest 构建 WPF 包。

### ADR-006：系统对话框策略（Open/Save 原生、Folder 自绘）
- **状态**：已采纳（计划，M6 实现）
- **决策**：OpenFile/SaveFile 直接使用系统对话框（Win11 原生即 Fluent，`Microsoft.Win32.OpenFileDialog/SaveFileDialog`），Settings → Win32 参数映射；FolderPicker 自绘（TreeView + 最近目录），无第三方依赖。

## 5. 控件规范

| 控件 | 规范 | 状态 |
|---|---|---|
| ApeButton | 高 32、MinWidth 96、Padding 16,0、圆角 4、Hover 填充切换、Pressed 缩放 0.98、Focus 描边环、`IsAccent` 主操作变体 | ✅ M1 |
| ApeTextBox | 高 32、圆角 4、`PlaceholderText` 占位符、Hover 边框 `#9E9E9E`、Focus 强调色边框（2px）、禁用填充 | ✅ M2 |
| ApePasswordBox | PasswordBox 密封类 → 包装控件方案（内部 `PART_PasswordBox` + `Password` DP 双向同步）；余同 TextBox | ✅ M2 |
| ApeCheckBox | 20×20 圆角 4 勾选块；勾选 → 强调色填充 + 白色对勾 Path；Hover/Pressed/禁用（Opacity 0.5） | ✅ M2 |
| ApeRadioButton | 20×20 圆；选中 → 强调色描边 + 8px 内点；Hover/Pressed/禁用 | ✅ M2 |
| ApeComboBox | 高 32；触发器 + 覆盖式 SelectionBox 展示；弹层淡入、圆角 4、项样式 `ApeComboBoxItemStyle`（Hover 填充、选中强调色文字） | ✅ M2 |
| ApeProgressBar | 高 4、圆角 2 轨道；`PART_Track`/`PART_Indicator` 确定模式；不确定模式 80px 指示条滑动动画（EnterActions Storyboard） | ✅ M2 |
| ApeListView | 透明边框；项样式 `ApeListViewItemStyle`（圆角 4、Hover 填充、选中强调色文字） | ✅ M2 |

**通用约定**：所有控件圆角取 `ApeControlCornerRadius`(4)；所有状态刷来自 DynamicResource 令牌（深浅色自动切换）；禁用态统一 Opacity 0.5 或禁用令牌。

**样式作用域约定**：隐式样式（TargetType 无 Key）仅用于本库自定义控件类型；公共控件（ComboBoxItem/ListViewItem 等）一律显式 Key，避免污染宿主应用。

## 6. 里程碑

| 里程碑 | 内容 | 状态 |
|---|---|---|
| M1 | 骨架、令牌、宿主、Backdrop、ThemeManager、ApeButton 基础 | ✅ |
| M2 | 控件组（TextBox/PasswordBox/CheckBox/RadioButton/ComboBox/ProgressBar/ListView） | ✅ |
| M3 | 基础 Dialog（Message/Prompt/Input/Password/Text/Toast）+ Provider + 快捷键 | ✅ |
| M4 | 选择 4 种（Selection/MultipleSelection/ComboBoxInput/DateTime） | ✅ |
| M5 | 调节 3 种 + 校验反馈（NumberInput/Slider/RangeSlider/Progress） | ✅ |
| M6 | ColorPicker、DataEntrySheet、系统对话框、FolderPicker | ✅ |
| M7 | WinForms 桥包、示例、文档、打包 | ✅ |

## M4 实现记录

- **WpfSelectionDialog\<T\> / WpfMultipleSelectionDialog\<T\>**：`SelectionItem`（原始项 + 显示文本）包装 + `ApeListView`；默认选中项预选；双击即确认；多选支持 `SelectAll`/`ReverseSelection` 内置行为（基类新增 `OnSelectAllRequested`/`OnReverseSelectionRequested`/`OnSetCurrentTimeRequested` 虚拟方法）。
- **WpfComboBoxInputDialog**：ApeComboBox 增加可编辑模式支持（模板补 `PART_EditableTextBox`，`IsEditable` 触发切换）；非可编辑时按 `DefaultText` 匹配预选项。
- **WpfDateTimeDialog**：按 `Precision` 组合展示（DateTime=日历+时分秒、Date=仅日历、Time=仅时间）；时间用三个 `ApeComboBox`；`SetCurrentTime` 行为置为当前时刻。
- **注**：Calendar 控件沿用默认模板（Win11 重样式列为后续优化项）。

## M5 实现记录

- **ApeSlider**：Fluent 模板（`PART_Track` + 左右 RepeatButton 轨道 + 20px 圆形 Thumb，悬停描边、拖动实心强调色、微阴影）；`TickFrequency > 0` 时自动吸附（`IsSnapToTickEnabled`）。注意 WPF Slider 为 `double`，与标准库 `decimal` 之间显式转换。
- **ApeNumericUpDown**：包装控件（`PART_TextBox`/`PART_UpButton`/`PART_DownButton`）；范围钳制、步进、`DecimalPlaces` 格式化（CurrentCulture）、输入即时解析 + LostFocus 归一化、`Commit()` 提交入口；边界自动禁用按钮。
- **ApeRangeSlider**：**纯代码自绘**（FrameworkElement + OnRender，无 XAML 模板）：4px 圆角轨道 + 中间强调色高亮 + 双圆形滑块；拖拽命中测试（2.2 倍半径）、MoveToPoint（点击轨道移动最近滑块）、`Start <= End` 约束（程序化设置也生效）、`RangeChanged` 拖拽中持续触发。
- **WpfAdjustDialogBase\<TResult\>**：在 WpfDialogBase 之上实现 `IAdjustDialog<T>`（ValueChanged 事件 + `RaiseValueChanged` 同时触发 Settings.OnValueChanged）——避免与标准库 `BaseAdjustDialog` 的单一继承冲突，宿主装配逻辑零重复。
- **WpfSliderDialog / WpfRangeSliderDialog**：拖动实时回调 + 大字号当前值标签（ShowValueLabel）；确认返回最终值；取消不回滚（语义见标准库文档）。
- **WpfNumberInputDialog**：NumericUpDown + 校验错误行。
- **WpfProgressDialog**：ApeProgressBar（确定/不确定）+ 消息行；`UpdateProgress` 支持跨线程（Dispatcher 归位）；**进度达到 1.0 自动以确认结果关闭**。
- **Provider**：14 个 Create 已实现；剩余 5 个（Color/DataEntrySheet/OpenFile/OpenFolder/SaveFile）M6 替换。

## M6 实现记录

- **ApeColorPicker**：SV 面板 + 色相条 + RGB 数值 + Hex 输入 + Alpha 滑块 + 预设色块，多入口统一走 `ApplyColor` 单向收敛（防递归）。SV 面板/色相条纯代码自绘（白色→纯色横向渐变 + 透明→黑纵向渐变叠加；色相条 7 色渐变 + 双圈指示器）；`ApeNumericUpDown` 增加 `ValueChanged` 事件供联动；Hex 支持 `#RRGGBB`/`#AARRGGBB`，非法输入回退。
- **WpfColorDialog**：DefaultColor/AllowAlpha/PresetColors 直通。
- **WpfDataEntrySheetDialog**：10 种字段编辑器（Text/Password/HexBytes/FilePath+浏览/Number/SingleChoice/ComboBox/MultipleChoice/DateTime），**双层校验**（UI 解析失败 → 行内错误；字段 Validator 失败 → 行内错误 + 抖动）；表单放 ScrollViewer（字段多时可滚动）。
- **WpfOpenFileDialog / WpfSaveFileDialog**：直接调用系统对话框（Win11 原生 Fluent），标准契约不变；`SearchPattern` → Filter 转换；宿主不显示（`DismissHandler` 空实现）。
- **WpfOpenFolderDialog**：自绘目录浏览器（面包屑路径 + Up 按钮 + 子目录列表 + 双击进入 + 无权限目录静默跳过）；已知限制：忽略 MultiSelect（结果=当前浏览目录单路径）。
- **Provider**：19 个 Create 全部实现，占位全部清除。

## M7 实现记录

- **桥包 ApeFree.ApeDialogs.WPF.WinForms**（net461;net8.0-windows，UseWPF+UseWindowsForms）：`FormOwner`（Form→IWin32Owner）+ 19 个 `ShowXxxDialogAsync(settings, Form? owner, CancellationToken)` 扩展方法（内部包装 FormOwner）。重载决议：实例方法（IWin32Owner 上下文）与扩展方法（Form 上下文）共存，编译器自动选择。
- **示例**：`ApeDialogs.WpfDemo`（19 种 Dialog 全演示：实时滑块回调、进度循环、动态表单、系统对话框等）、`ApeDialogs.WinFormsDemo`（桥包三连演示）。
- **CI**：main job（ubuntu/windows）构建整个 sln 时增加 `-p:EnableWindowsTargeting=true`（WPF 工程交叉编译）；wpf-build job（windows-latest）保持不变。
- **编码规范**：所有源文件统一 UTF-8（无 BOM），禁止用命令行工具改写含中文的源文件（曾因 PowerShell 编码回写导致注释损坏，已修复并恢复）。
- **已知限制**（已写入 README）：PasswordChar 无效（WPF PasswordBox 限制）；OpenFolderDialog.MultiSelect 无效；Calendar 默认模板。

## M7.1 修复记录：圆角/阴影黑框

- **症状**：窗体四周出现黑色边框。
- **根因**：`DialogHostWindow` 未启用 `AllowsTransparency`（默认 false），WPF 将 `Background="Transparent"` 的像素 alpha 强制为 255，DWM 合成时透明区（阴影留白的 Margin=20 区域）渲染为黑色。原 DWM 亚克力（ACCENT 策略）仅 Win11 22H2+ 生效，非该环境直接暴露黑框。
- **修复**：`AllowsTransparency="True"`（真透明窗口，DropShadowEffect 阴影/圆角/动画均正常）；移除对 layered 窗口无效的 `WindowsBackdrop`（亚克力/深色标题栏）调用，删除 WindowsBackdrop.cs、WindowsVersion.cs。背景材质回退为 RootBorder 的 `ApeWindowBackground` 纯色资源（深/浅色随主题）。
- **取舍**：不再使用 DWM 亚克力/米卡材质（AllowsTransparency=true 的窗口不受 DWM 背景材质策略支持）。如未来需要 Win11 原生材质，需改为"普通窗口 + DWMWA 圆角 + 系统阴影"的另一种宿主实现。

## M3 实现记录

- **WpfDialogBase\<TResult\>**：统一宿主装配（Title/Cancelable/CloseRequested→Cancel/ConfirmRequested→确认）、选项按钮装配（`DialogOptionBehavior` 分发：Cancel→Cancel、Clear→OnClearRequested、其余→OnSelected）、选项标签样式（Positive→强调色按钮、Negative/Special→语义色文字）、模态展示（原生 Owner + 居中）、非 UI 线程 Dismiss 自动归位 Dispatcher、校验失败抖动。
- **6 个 Dialog**：Message（Confirm→TrySubmit(true)）、Prompt（Yes/No→TrySubmit）、Input（含多行、最大长度、错误文本行）、Password（错误文本行）、Text（只读/换行/可复制）、Toast（非模态右下角、不抢焦点、点击→确认）。
- **快捷键**：Esc→取消（仅 Cancelable）、Enter→确认（多行输入时回车不触发）。
- **已知限制**：WPF PasswordBox 不支持自定义 `PasswordChar`（掩码固定平台默认），已在 XML 注释与 README 记录。
- **Provider**：`ApeDialogsWpfProvider`，10 个 Create 已实现；其余 9 个暂时 `NotImplementedException`（M5-M6 逐个替换）。
- **标准库增强**：`DialogOption.IsCancel` 布尔升级为 `DialogOptionBehavior` 枚举（None/Cancel/Clear/SelectAll/ReverseSelection/SetCurrentTime），为 M4/M5 的 Clear/All/Reverse/Now 内置行为铺路。
