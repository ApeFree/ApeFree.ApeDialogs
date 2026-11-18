# ApeFree.ApeDialogs.Maui 设计文档

> ApeDialogs 的 .NET MAUI 实现库，遵循 ApeDialog 规范，支持 Android、iOS、macOS 与 Windows。

## 1. 总体架构

```
┌─────────────────────────────────────────────────────────────┐
│                    消费者（MAUI 应用 / ViewModel）          │
│   DialogResult<T> r = await provider.ShowInputDialogAsync(…);│
└──────────────────────────────┬──────────────────────────────┘
                               │ IMauiDialogContext (可选指定 Page)
┌──────────────────────────────▼──────────────────────────────┐
│  ApeDialogsMauiProvider : DialogProvider<IMauiDialogContext>│
│  · 19 个 CreateXxxDialog（生成对应 MAUI Dialog 实例）       │
│  · ShowXxxDialogAsync 由标准库基类提供                      │
│  · 自动解析当前可视 Page（CurrentPageResolver）              │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│  MauiDialogBase<TResult> : BaseDialog<MauiDialogHost,       │
│                            ApeButton, IMauiDialogContext,    │
│                            TResult>                         │
│  · 统一纯自研宿主 MauiDialogHost（遮罩/圆角卡片/阴影/动画） │
│  · 内容区与选项装配：ApeButton / 动态控件布局               │
│  · 软键盘避让（ScrollView 保证可视）                        │
│  · 物理返回键（Android）拦截与 Cancelable 语义控制          │
│  · TrySubmit → Validator → OnValidationFailed（抖动反馈）   │
└─────────────────────────────────────────────────────────────┘

支撑层：
  Theme/       MauiThemeManager（深浅色跟随 + ApeColor 转换）
  Hosting/     MauiDialogHost（纯自研全屏浮层宿主）、CurrentPageResolver
  Controls/    ApeButton, ApeNumericUpDown, ApeRangeSlider(自绘), ApeColorPicker(自绘)
  Extensions/  MauiAppBuilderExtensions（builder.UseApeDialogs() 注册）
```

## 2. 包结构与目标平台

| 包 | TFM | 适用场景 |
|---|---|---|
| `ApeFree.ApeDialogs.Maui` | `net8.0;net8.0-android;net8.0-windows10.0.19041.0;net8.0-ios;net8.0-maccatalyst` | MAUI 跨平台应用 |

## 3. 设计令牌（浅色 / 深色）

| 令牌 | 浅色 | 深色 |
|---|---|---|
| 遮罩背景 `OverlayColor` | `rgba(0, 0, 0, 0.4)` | `rgba(0, 0, 0, 0.6)` |
| 对话框卡片背景 `CardBackground` | `#FFFFFF` | `#202020` |
| 主文本 `TextPrimary` | `#1B1B1B` | `#FFFFFF` |
| 次要文本 `TextSecondary` | `#5C5C5C` | `#A0A0A0` |
| 边框线 `StrokeDefault` | `#E0E0E0` | `#383838` |
| 强调色 `AccentColor` | `#0067C0` | `#60CDFF` |
| 语义：积极 / 消极 / 特殊 | `#0F7B0F` / `#C42B1C` / `#773BDB` | `#6CCB5F` / `#FF99A4` / `#9B8FFF` |
| 卡片圆角 `CardCornerRadius` | 12 | 12 |
| 控件圆角 `ControlCornerRadius` | 6 | 6 |

## 4. 关键决策记录 (ADR)

### ADR-001：宿主采用纯自研 Window/Page 遮罩层（MauiDialogHost）
- **状态**：已采纳（M1）
- **背景**：MAUI 原生弹窗能力极弱，且 CommunityToolkit.Maui 的 Popup 存在版本绑定、额外大体积依赖及平台视图差异。
- **决策**：自研 `MauiDialogHost`，基于全屏半透明遮罩 + 居中圆角卡片。在当前可视 Page 上动态注入浮层。
- **优点**：零第三方依赖、跨平台 100% 视觉一致、支持全平台统一的进出场淡入缩放动画与校验失败抖动动画。

### ADR-002：高级控件（RangeSlider, ColorPicker）采用 Microsoft.Maui.Graphics 自绘
- **状态**：已采纳（M4/M6）
- **决策**：利用 MAUI 内置的 `GraphicsView` + `IDrawable` 进行纯代码 2D 绘制。
- **优点**：无需引用任何第三方组件，跨平台高性能原生硬件渲染，保证多平台交互手感一致。

### ADR-003：上下文设计为 IMauiDialogContext + 缺省自动发现
- **状态**：已采纳（M1）
- **决策**：`DialogProvider<IMauiDialogContext>`，当调用方未传 context 时，内部通过 `CurrentPageResolver` 自动从 `Application.Current.Windows` 或 `Shell.Current` 解析当前激活页面。
