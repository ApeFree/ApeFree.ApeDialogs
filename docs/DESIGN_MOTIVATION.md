# ApeFree.ApeDialogs 开发初衷与设计文档

`ApeFree.ApeDialogs` 是一套面向 .NET 生态的**跨平台 Dialog 标准契约库**（Standard Contract Library）。它将“对话框交互（Dialog）”从具体的 UI 框架中完全抽象剥离，制定了一套统一的展示流程、生命周期、配置模型与结果处理规范。

---

## 1. 开发背景与核心痛点

在 .NET 桌面端、移动端与跨平台应用开发中，对话框（Dialog）是最常见但又最容易导致架构混乱的交互组件。项目的开发初衷主要源于以下四大痛点：

### 1.1 UI 框架碎片化，业务代码与具体 UI 深度绑定
* **痛点现状**：.NET 平台拥有众多的 UI 框架（WinForms、WPF、Avalonia、MAUI/Uno、以及 Web Blazor 和 CLI 命令行终端等）。传统的弹框方式（如 `MessageBox.Show`、特定窗口的 `ShowDialog`）直接依赖具体的 UI 程序集。
* **引发问题**：在 MVVM、DDD 或分层架构中，ViewModel / 业务服务层如果需要弹框提示或获取用户输入，往往被迫引入具体的 UI 引用，导致业务逻辑无法跨平台移植，也无法进行单元测试。

### 1.2 缺乏统一的高频对话框交互规范
* **痛点现状**：提示（Prompt）、文本输入（Input）、密码输入（Password）、单选/多选（Selection）、日期选择（DateTime）、滑块调节（Slider）、表单录入（DataEntrySheet）、进度提示（Progress）、轻提示（Toast）等高频弹窗在各个项目中被反复重复造轮子。
* **引发问题**：每个项目、甚至同一项目的不同模块中，弹窗的 API 签名、传参方式、确认/取消语义各不相同，增加了开发和维护的心智负担。

### 1.3 传统弹框缺乏现代异步与生命周期规范
* **痛点现状**：早期的 Windows 弹窗多为阻塞式模态窗口（`ShowDialog`），缺乏对 `async/await`、`CancellationToken` 异步取消、自动超时关闭（`Duration`）、数据有效性实时校验（`ValidationResult`）的原生支持。
* **引发问题**：许多实现使用抛出异常（如 `OperationCanceledException`）来表示取消，导致异常控制流泛滥，代码冗长且性能较差。

### 1.4 ApeFree 开源生态的统一需求
* ApeFree 组织下拥有多个开源 UI 库（如 ApeForms、AndroidPlus 等）。为了让上层业务代码“一次编写，跨端通用”，需要一套中立的 Dialog 标准契约，由各 UI 库分别实现该标准。

---

## 2. 核心设计原则与架构思想

```
┌─────────────────────────────────────────────────────────────┐
│                    业务代码（消费者）                        │
│   DialogResult<T> r = await provider.ShowInputDialogAsync(…);│
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│  DialogProvider<TContext>（抽象基类）                        │
│  · 19 个 CreateXxxDialog 抽象方法（实现方必做）              │
│  · 19 个 ShowXxxDialogAsync 默认实现（Create + ShowAsync）  │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│  IDialog<TResult>           IDialogController<TResult>      │
│  · ShowAsync(CancellationToken)  · Dismiss(DialogResult<T>) │
│  · Showing/Shown/Dismissing/Dismissed  · Cancel()           │
│  · IsShown                                                  │
│              BaseDialog<TView,TOption,TContext,TResult>     │
│             （实现方继承，含 Duration 自动关闭等内置能力）    │
└─────────────────────────────────────────────────────────────┘
```

为了彻底解决上述痛点，`ApeFree.ApeDialogs` 确立了以下核心设计原则：

| 原则 | 说明 |
|---|---|
| **零 UI 框架依赖 (Zero UI Dependencies)** | 标准核心面向 `netstandard2.0` / `net8.0`，完全剥离 `System.Drawing`，采用内置的 `ApeColor`、`DialogFont`、`ApeSize` 等跨平台值类型。 |
| **消费者与实现方分离 (Role Separation)** | 消费者仅接触 `IDialog<TResult>` 与 `IDialogProvider<TContext>`；UI 实现方继承 `BaseDialog<TView, TOption, TContext, TResult>` 并操作 `IDialogController<TResult>`。 |
| **异步优先与从不抛异常 (Async-First & Never Throw)** | 所有展示入口统一为 `Task<DialogResult<T>>`。用户取消与 Token 取消均返回不可变的 `DialogResult<T>.Canceled`，无需 try-catch 拦截取消。 |
| **不可变结果模型 (Immutable Result)** | `DialogResult<T>` 为 `readonly record struct`，具备值语义与线程安全性，支持 `Match` 分支匹配与 `RequireValue` 提取。 |
| **声明式配置优先 (Declarative First)** | 校验规则（`Validator`）、自动关闭（`Duration`）、生命周期监听（`OnShown`/`OnDismissed`）等直接声明在 `DialogSettings` 中。 |

---

## 3. 标准化对话框能力矩阵

标准库将常见对话框归纳为 **19 种标准 Dialog 体系**，覆盖日常绝大部分弹窗场景：

```
ApeDialogs 体系
├── 1. 基础信息类
│   ├── MessageDialog          (消息提示，结果 bool)
│   ├── PromptDialog           (Yes/No 二选一，结果 bool)
│   ├── TextDialog             (长文本/协议查看，结果 bool)
│   └── ToastDialog            (轻提示，定时自动消失)
├── 2. 输入与表单类
│   ├── InputDialog            (单行/多行文本输入，结果 string)
│   ├── PasswordDialog         (密码输入掩码，结果 string)
│   ├── NumberInputDialog      (数值步进输入，结果 decimal)
│   ├── ComboBoxInputDialog    (可编辑下拉输入，结果 string)
│   └── DataEntrySheetDialog   (多字段复杂表单，结果 DataEntrySheet)
├── 3. 数据选择类
│   ├── SelectionDialog<T>     (单选列表，结果 T)
│   ├── MultipleSelectionDialog<T> (多选列表，结果 IReadOnlyList<T>)
│   ├── DateTimeDialog         (日期时间选择器，结果 DateTime)
│   └── ColorDialog            (跨平台颜色选择，结果 ApeColor)
├── 4. 调节与过程控制类
│   ├── SliderDialog           (单滑块调节，实时推流，结果 decimal)
│   ├── RangeSliderDialog      (双滑块区间调节，结果 ValueRange<decimal>)
│   └── ProgressDialog         (进度条控制，IProgressDialog 接口推进)
└── 5. 存储与系统交互类
    ├── OpenFileDialog         (文件选择，结果 IReadOnlyList<string>)
    ├── SaveFileDialog         (文件保存路径选择，结果 string)
    └── OpenFolderDialog       (文件夹选择，结果 IReadOnlyList<string>)
```

---

## 4. 项目结构与生态划分

当前解决方案采用了模块化与分层设计：

```
ApeFree.ApeDialogs.sln
│
├── ApeFree.ApeDialogs/                        [核心标准层]
│   ├── Core/                                  IDialog、BaseDialog、DialogResult、ValidationResult 等契约
│   ├── Settings/                              19 种 DialogSettings、DataEntrySheet 表单引擎
│   ├── Theme/                                 跨平台 ApeColor、DialogFont、IDialogTheme
│   └── IDialogProvider.cs / DialogProvider.cs 统一 Provider 抽象
│
├── ApeFree.ApeDialogs.DependencyInjection/    [DI 扩展]
│   └── 支持 Microsoft.Extensions.DependencyInjection (AddApeDialogs)
│
├── ApeFree.ApeDialogs.WPF/                    [WPF 原生实现]
│   └── 基于 Windows 11 Fluent 规范（亚克力、圆角、深浅色跟随、自绘控件）
│
├── ApeFree.ApeDialogs.WPF.WinForms/           [WinForms 桥接库]
│   └── 将 WinForms Form 包装为 IWin32Owner，复用现代 WPF Fluent 对话框
│
├── samples/                                   [示例工程]
│   ├── ApeDialogs.WpfDemo/                    WPF 演示
│   └── ApeDialogs.WinFormsDemo/               WinForms 演示
│
└── tests/                                     [单元测试]
    └── ApeFree.ApeDialogs.Tests/              基于 Mock (TestProvider / TestDialog) 的全套测试
```

---

## 5. 项目演进历程与成熟度 (v0.x → v1.0)

从代码提交历史与版本变更记录可以看出项目的演进脉络：

1. **早期探索阶段 (v0.x)**：
   - 提出了 Dialog 配置实体与 Provider 统一入口的雏形；
   - 引入了 `DataEntrySheet` 动态表单、语义 Tag（`Positive`/`Negative`/`Functional`）以及配色风格；
   - *早期遗留问题*：依赖 `System.Drawing` 限制了跨平台能力；使用反射扩展方法；取消操作与异常机制耦合。
2. **现代化重构与标准化阶段 (v1.0)**：
   - **彻底剥离 UI 依赖**：使用 `ApeColor` / `DialogFont` 替代 GDI/System.Drawing；
   - **全面 async-first 与不可变值**：弃用老旧回调，采用 `readonly record struct DialogResult<T>` 与 `Task<DialogResult<T>>`；
   - **强化生命周期与校验**：内置 `Duration` 倒计时、`TrySubmit` 自动校验机制；
   - **落地生产级参考实现**：打造了完整的 Win11 Fluent 风格 WPF 实现及 WinForms 桥接包，并配套了全套 xUnit 单元测试与 DI 扩展。

---

## 6. 总结与价值

`ApeFree.ApeDialogs` 的核心价值在于：
1. **对业务开发者**：只需面向标准接口编程（`provider.ShowXxxDialogAsync(...)`），业务逻辑干净利落，具备极高的可读性、可测试性（通过 Mock Provider 进行单测），且不用关心底层是 WinForms、WPF 还是跨平台应用。
2. **对 UI 框架实现方**：标准库提供了完备的基类（`BaseDialog`、`BaseAdjustDialog`）以及通用的生命周期/超时管理逻辑，实现方只需聚焦于具体的视图渲染和控件绑定。
