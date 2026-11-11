# ApeFree.ApeDialogs

ApeFree.ApeDialogs 是一套**跨平台的 Dialog 标准库**（非 UI 实现库）。它定义对话框的统一契约——展示、结果、配置与 Provider 抽象——让桌面、Web、移动端乃至命令行应用中的"弹框"遵循同一套 API 与语义，业务代码只学一次。

本库由 ApeFree 已开源的 UI 库（ApeForms、AndroidPlus 等）按同一套契约实现。你也可以为自己的 UI 框架实现这套标准。

- 目标框架：`netstandard2.0` / `net8.0`
- 零 UI 框架依赖：标准层不依赖 System.Drawing，跨平台颜色/字体/尺寸使用自有类型
- 授权：Apache-2.0

## 目录

- [安装](#安装)
- [核心概念](#核心概念)
- [快速上手](#快速上手)
- [支持的 Dialog 一览](#支持的-dialog-一览)
- [API 参考](#api-参考)
- [实现方指南](#实现方指南)
- [依赖注入](#依赖注入)
- [v0.x → v1.0 迁移对照](#v0x--v10-迁移对照)
- [项目结构](#项目结构)

## 安装

```bash
dotnet add package ApeFree.ApeDialogs
# 可选：DI 集成
dotnet add package ApeFree.ApeDialogs.DependencyInjection
```

## 核心概念

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

配置侧：
  DialogSettings 家族（Title/Content/Cancelable/Theme/DialogSize/Font/
                    Duration/OnShown/OnDismissed/DefaultOptions + Validator）
  DialogOption（Text/Tag/IsEnabled/IsCancel/OnSelected）
  IDialogTheme（DialogOptionTag → ApeColor）
  DataEntrySheet（多字段表单）
```

**设计原则**

| 原则 | 说明 |
|---|---|
| 不抛异常 | 用户取消与 token 取消均返回 `DialogResult<T>.Canceled`，`ShowAsync` 从不因取消抛异常 |
| 结果不可变 | `DialogResult<T>` 为 `readonly record struct`，值语义、线程安全 |
| async-first | 所有展示入口为 `Task<DialogResult<T>>`，支持 `CancellationToken` |
| 消费者/实现方分离 | `IDialog<T>`（消费）与 `IDialogController<T>`（实现）面向不同角色 |
| 声明式优先 | `Duration`、`Validator`、`OnShown/OnDismissed` 在 Settings 中声明，减少命令式事件 |

## 快速上手

```csharp
using ApeFree.ApeDialogs;

// 一次性展示：创建 + 显示 + 等待结果
DialogResult<string> result = await provider.ShowInputDialogAsync(new InputDialogSettings
{
    Title = "请输入昵称",
    Content = "昵称将展示在排行榜上",
    Validator = v => string.IsNullOrWhiteSpace(v)
        ? ValidationResult.Failure("昵称不能为空")
        : ValidationResult.Success,
    Duration = TimeSpan.FromSeconds(60), // 60 秒未操作自动以取消关闭
});

// 结果处理：分支式，无需判断异常
result.Match(
    onConfirmed: value => Console.WriteLine($"你好，{value}"),
    onCanceled: () => Console.WriteLine("用户取消了"));

// 或直接取值（取消时抛 InvalidOperationException）
if (!result.IsCanceled)
{
    Console.WriteLine($"你好，{result.Value}");
}
```

**取消（CancellationToken）**

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var result = await provider.ShowPromptDialogAsync(settings, cancellationToken: cts.Token);
// 30 秒内用户未操作 → 对话框自动关闭并返回 Canceled，不会抛 OperationCanceledException
```

**监听生命周期**

```csharp
// 方式一：Settings 内委托（一次性展示也适用）
var settings = new MessageDialogSettings
{
    Title = "提示",
    OnShown = dialog => Console.WriteLine("已显示"),
    OnDismissed = dialog => Console.WriteLine("已关闭"),
};

// 方式二：事件（Create 模式）
var dialog = provider.CreateMessageDialog(settings);
dialog.Showing += (_, _) => Console.WriteLine("显示中");
dialog.Shown += (_, _) => Console.WriteLine("已显示");
dialog.Dismissed += (_, _) => Console.WriteLine("已关闭");
await dialog.ShowAsync();
```

**深度定制（Create 模式）**

```csharp
var dialog = provider.CreateSelectionDialog(
    new SelectionDialogSettings<int> { Title = "选择" },
    collection: new[] { 1, 2, 3 },
    defaultSelectedItem: 2);

dialog.AddOption(new DialogOption("自定义操作", DialogOptionTag.Special,
    OnSelected: (s, e) => Console.WriteLine("自定义按钮被点击")));

DialogResult<int> result = await dialog.ShowAsync();
```

## 支持的 Dialog 一览

ApeDialogs 共定义 **19 种标准 Dialog**，每种包含：配置类（`XxxDialogSettings`）、创建方法（`CreateXxxDialog`）与一次性展示方法（`ShowXxxDialogAsync`）。

| # | Dialog | 结果类型 | 配置类 | 默认选项 |
|---|---|---|---|---|
| 1 | 消息对话框 | `bool` | `MessageDialogSettings` | Confirm |
| 2 | 提示对话框（Yes/No） | `bool` | `PromptDialogSettings` | Yes、No |
| 3 | 输入对话框 | `string` | `InputDialogSettings` | Confirm、Cancel、Clear |
| 4 | 密码对话框 | `string` | `PasswordDialogSettings` | Confirm、Cancel、Clear |
| 5 | 单选对话框 | `T` | `SelectionDialogSettings<T>` | Confirm、Cancel |
| 6 | 多选对话框 | `IReadOnlyList<T>` | `MultipleSelectionDialogSettings<T>` | Confirm、Cancel、All、Reverse |
| 7 | 日期时间对话框 | `DateTime` | `DateTimeDialogSettings` | Confirm、Cancel、Now |
| 8 | 多字段表单对话框 | `DataEntrySheet` | `DataEntrySheetDialogSettings` | Confirm、Cancel |
| 9 | 打开文件对话框 | `IReadOnlyList<string>` | `OpenFileDialogSettings` | Confirm、Cancel |
| 10 | 打开文件夹对话框 | `IReadOnlyList<string>` | `OpenFolderDialogSettings` | Confirm、Cancel |
| 11 | 数值输入对话框 | `decimal` | `NumberInputDialogSettings` | Confirm、Cancel |
| 12 | 单滑块调节对话框 | `decimal` | `SliderDialogSettings` | Confirm、Cancel |
| 13 | 双滑块范围调节对话框 | `ValueRange<decimal>` | `RangeSliderDialogSettings` | Confirm、Cancel |
| 14 | 进度对话框 | `bool` | `ProgressDialogSettings` | Cancel |
| 15 | 轻提示对话框（Toast） | `bool`（点击=true） | `ToastDialogSettings` | — |
| 16 | 长文本查看对话框 | `bool` | `TextDialogSettings` | Close |
| 17 | 颜色选择对话框 | `ApeColor` | `ColorDialogSettings` | Confirm、Cancel |
| 18 | 保存文件对话框 | `string` | `SaveFileDialogSettings` | Save、Cancel |
| 19 | 可编辑下拉输入对话框 | `string` | `ComboBoxInputDialogSettings` | Confirm、Cancel |

### 1. 消息对话框（MessageDialog）

仅一个确认按钮，结果恒为 `true`（除非被取消）。

```csharp
var result = await provider.ShowMessageDialogAsync(new MessageDialogSettings
{
    Title = "提示",
    Content = "操作完成",
    ConfirmOption = new DialogOption("知道了", DialogOptionTag.Positive), // 自定义按钮文本
});

if (result.IsCanceled) return;
Console.WriteLine("用户点击了确认");
```

### 2. 提示对话框（PromptDialog）

Yes/No 二选一，结果类型 `bool`（Yes = `true`）。

```csharp
var result = await provider.ShowPromptDialogAsync(new PromptDialogSettings
{
    Title = "删除文件",
    Content = "确定要删除该文件吗？此操作不可恢复。",
    PositiveOption = new DialogOption("删除", DialogOptionTag.Negative), // 可自定义语义配色
    NegativeOption = new DialogOption("保留", DialogOptionTag.Positive),
});

// 推荐用法：按确认结果与选项布尔值分支处理
if (result.IsConfirmed)
{
    if (result.Value)
    {
        Console.WriteLine("执行删除");
    }
    else
    {
        Console.WriteLine("保留文件");
    }
}
else
{
    Console.WriteLine("取消操作");
}
```

### 3. 输入对话框（InputDialog）

单行/多行文本输入，结果类型 `string`。

```csharp
var result = await provider.ShowInputDialogAsync(new InputDialogSettings
{
    Title = "重命名",
    Content = "输入新的名称：",
    DefaultText = "默认名称",
    MaximumLength = 64,
    IsMultiline = false,
    Validator = v => string.IsNullOrWhiteSpace(v)
        ? ValidationResult.Failure("名称不能为空")
        : ValidationResult.Success,       // 校验失败时对话框不关闭
});
```

### 4. 密码对话框（PasswordDialog）

继承 `InputDialogSettings`，密码字符掩码输入。

```csharp
var result = await provider.ShowPasswordDialogAsync(new PasswordDialogSettings
{
    Title = "解锁",
    Content = "请输入密码：",
    PasswordChar = '*',
});
```

### 5. 单选对话框（SelectionDialog）

从集合中单选一项，结果类型 `T`。

```csharp
var result = await provider.ShowSelectionDialogAsync(
    new SelectionDialogSettings<Color> { Title = "选择颜色" },
    collection: new[] { Color.Red, Color.Green, Color.Blue },
    defaultSelectedItem: Color.Green);

if (!result.IsCanceled)
{
    Console.WriteLine($"已选择：{result.Value}");
}
```

`SelectionDialogSettings<T>` 额外属性：

| 属性 | 类型 | 说明 |
|---|---|---|
| `ConfirmOption` / `CancelOption` | `DialogOption` | 确认/取消选项 |
| `ItemDisplayConverter` | `Func<T, string>` | 选项显示文本转换器，默认 `ToString()` |

### 6. 多选对话框（MultipleSelectionDialog）

从集合中多选，结果类型 `IReadOnlyList<T>`。

```csharp
var result = await provider.ShowMultipleSelectionDialogAsync(
    new MultipleSelectionDialogSettings<string> { Title = "选择标签" },
    collection: new[] { "A", "B", "C", "D" },
    defaultSelectedItems: new[] { "A", "C" });

if (!result.IsCanceled)
{
    Console.WriteLine(string.Join(", ", result.Value)); // 已选中的项
}
```

`MultipleSelectionDialogSettings<T>` 额外属性：

| 属性 | 类型 | 说明 |
|---|---|---|
| `ConfirmOption` / `CancelOption` | `DialogOption` | 确认/取消选项 |
| `SelectAllOption` | `DialogOption` | 全选选项（默认文本 "All"） |
| `ReverseSelectedOption` | `DialogOption` | 反选选项（默认文本 "Reverse"） |
| `ItemDisplayConverter` | `Func<T, string>` | 选项显示文本转换器 |

### 7. 日期时间对话框（DateTimeDialog）

日期/时间选择，结果类型 `DateTime`。

```csharp
var result = await provider.ShowDateTimeDialogAsync(new DateTimeDialogSettings
{
    Title = "选择日期",
    DefaultDateTime = DateTime.Now,
    DateTimeFormat = "yyyy-MM-dd",     // 24 小时制，默认 "yyyy-MM-dd HH:mm:ss"
    Precision = DateTimePrecision.Date, // DateTime / Date / Time
});
```

### 8. 多字段表单对话框（DataEntrySheetDialog）

一张表单多个字段（文本、密码、数值、下拉、单选、多选、日期、文件路径……），结果类型 `DataEntrySheet`（校验通过后返回整个表单，字段值已在 `Value` 中）。

```csharp
var sheet = new DataEntrySheet()
    .AddField(new TextField
    {
        Title = "姓名",
        Validator = v => string.IsNullOrWhiteSpace(v) ? ValidationResult.Failure("姓名必填") : ValidationResult.Success,
    })
    .AddField(new NumberField { Title = "年龄", Minimum = 0, Maximum = 150 })
    .AddField(new SingleChoiceField
    {
        Title = "性别",
        Items = new object[] { "男", "女" },
    })
    .AddField(new DateTimeField { Title = "生日" });

var result = await provider.ShowDataEntrySheetDialogAsync(sheet, new DataEntrySheetDialogSettings
{
    Title = "个人信息登记",
});

if (!result.IsCanceled)
{
    var age = ((NumberField)result.Value["年龄"]).Value;
    Console.WriteLine($"年龄：{age}");
}
```

**支持的字段类型**（详见 [SheetField 家族](#sheetfield-家族)）：`TextField`、`PasswordField`、`HexBytesField`、`FilePathField`、`PicturePathField`、`NumberField`、`SingleChoiceField`、`ComboBoxField`、`MultipleChoiceField`、`DateTimeField`。

### 9. 打开文件对话框（OpenFileDialog）

系统文件选择，结果类型 `IReadOnlyList<string>`（文件路径集合）。

```csharp
var result = await provider.ShowOpenFileDialogAsync(new OpenFileDialogSettings
{
    Title = "选择日志文件",
    InitialPath = @"C:\Logs",
    SearchPattern = "*.log",
    MultiSelect = true,
});

if (!result.IsCanceled)
{
    foreach (var path in result.Value)
    {
        Console.WriteLine(path);
    }
}
```

### 10. 打开文件夹对话框（OpenFolderDialog）

系统目录选择，结果类型 `IReadOnlyList<string>`（目录路径集合）。

```csharp
var result = await provider.ShowOpenFolderDialogAsync(new OpenFolderDialogSettings
{
    Title = "选择输出目录",
    MultiSelect = false,
});
```

### 11. 数值输入对话框（NumberInputDialog）

数值输入（类似 WinForms NumericUpDown 控件），结果类型 `decimal`。

```csharp
var result = await provider.ShowNumberInputDialogAsync(new NumberInputDialogSettings
{
    Title = "设置超时",
    Content = "请输入超时秒数：",
    Minimum = 1m,
    Maximum = 3600m,
    Increment = 5m,
    DecimalPlaces = 0,
    DefaultValue = 30m,
});

if (!result.IsCanceled)
{
    Console.WriteLine($"超时：{result.Value} 秒");
}
```

`NumberInputDialogSettings` 额外属性：`Minimum`（默认 0）、`Maximum`（默认 100）、`Increment`（默认 1）、`DecimalPlaces`（默认 0）、`DefaultValue`（默认 0）。

### 12. 单滑块调节对话框（SliderDialog）

实时调节：拖动过程中持续触发 `OnValueChanged` 直接影响外界，确认后返回最终值。

```csharp
// 一次性展示：通过 Settings 委托实时影响外界
var result = await provider.ShowSliderDialogAsync(new SliderDialogSettings
{
    Title = "音量调节",
    Minimum = 0m,
    Maximum = 100m,
    DefaultValue = 50m,
    OnValueChanged = volume => audioPlayer.Volume = (float)volume, // 拖动中实时生效
});

// Create 模式：订阅 ValueChanged 事件
var dialog = provider.CreateSliderDialog(new SliderDialogSettings { Minimum = 0, Maximum = 100 });
dialog.ValueChanged += (_, e) => audioPlayer.Volume = (float)e.NewValue;
DialogResult<decimal> final = await dialog.ShowAsync();
```

> **语义**：调节过程中值已实时推送，取消 Dialog **不会回滚**——如需回滚，由调用方保存初值自行处理。

`SliderDialogSettings` 额外属性：`Minimum`（默认 0）、`Maximum`（默认 100）、`DefaultValue`（默认 50）、`TickFrequency`（默认 1）、`ShowValueLabel`（默认 true）。

### 13. 双滑块范围调节对话框（RangeSliderDialog）

双滑块调节起止范围，结果类型 `ValueRange<decimal>`（`Start` / `End`）。

```csharp
var result = await provider.ShowRangeSliderDialogAsync(new RangeSliderDialogSettings
{
    Title = "筛选价格区间",
    Minimum = 0m,
    Maximum = 10000m,
    DefaultStart = 1000m,
    DefaultEnd = 5000m,
    OnValueChanged = range => priceFilter.Apply(range.Start, range.End), // 实时生效
});

if (!result.IsCanceled)
{
    Console.WriteLine($"区间：{result.Value.Start} ~ {result.Value.End}");
}
```

`RangeSliderDialogSettings` 额外属性：`Minimum`（默认 0）、`Maximum`（默认 100）、`DefaultStart`（默认 25）、`DefaultEnd`（默认 75）、`TickFrequency`（默认 1）、`ShowValueLabel`（默认 true）。

### 14. 进度对话框（ProgressDialog）

显示进度，显示期间由调用方通过返回的 `IProgressDialog` 推进进度；完成返回 `true`，取消返回 `Canceled`。

```csharp
var dialog = provider.CreateProgressDialog(new ProgressDialogSettings
{
    Title = "正在处理",
    IsIndeterminate = false,          // false = 确定进度条
    InitialProgress = 0,
    InitialMessage = "准备中…",
});

var task = dialog.ShowAsync();
for (int i = 1; i <= 10; i++)
{
    await Task.Delay(200);
    dialog.UpdateProgress(i / 10.0, $"处理中 {i * 10}%…"); // progress 范围 0.0 ~ 1.0
}
dialog.UpdateProgress(1.0, "完成");
bool completed = await task; // true = 正常完成
```

`ProgressDialogSettings` 额外属性：`IsIndeterminate`（默认 false）、`InitialProgress`（默认 0，范围 0~1）、`InitialMessage`、`CancelOption`（仅 `Cancelable=true` 时展示）。

### 15. 轻提示对话框（ToastDialog）

非阻塞轻提示，按 `Duration` 自动消失（默认 3 秒）；被点击时立即以确认结果关闭。通常 fire-and-forget：

```csharp
_ = provider.ShowToastDialogAsync(new ToastDialogSettings
{
    Content = "保存成功",
    Duration = TimeSpan.FromSeconds(2), // 覆盖默认 3 秒
    IsClickable = true,                  // 点击后立即关闭
});
```

### 16. 长文本查看对话框（TextDialog）

滚动查看长文本（日志、许可协议、帮助文档等）。`Content` 为简要说明，`Text` 为正文。

```csharp
var result = await provider.ShowTextDialogAsync(new TextDialogSettings
{
    Title = "许可协议",
    Content = "请阅读以下条款",
    Text = licenseText,        // 长文本正文
    WordWrap = true,
    IsSelectable = true,       // 允许复制
});
```

### 17. 颜色选择对话框（ColorDialog）

颜色选择，结果类型 `ApeColor`（标准层自有类型，跨平台）。

```csharp
var result = await provider.ShowColorDialogAsync(new ColorDialogSettings
{
    Title = "选择主题色",
    DefaultColor = new ApeColor(0, 122, 204),
    AllowAlpha = false,
    PresetColors = new[] { new ApeColor(255, 0, 0), new ApeColor(0, 255, 0), new ApeColor(0, 0, 255) },
});

if (!result.IsCanceled)
{
    Console.WriteLine($"已选择：{result.Value.ToArgb():X8}");
}
```

### 18. 保存文件对话框（SaveFileDialog）

保存路径选择，结果类型 `string`（单个保存路径）。

```csharp
var result = await provider.ShowSaveFileDialogAsync(new SaveFileDialogSettings
{
    Title = "导出数据",
    InitialPath = @"C:\Data",
    DefaultFileName = "export.csv",
    SearchPattern = "*.csv",
    OverwritePrompt = true, // 文件已存在时提示覆盖
});

if (!result.IsCanceled)
{
    Console.WriteLine($"保存至：{result.Value}");
}
```

### 19. 可编辑下拉输入对话框（ComboBoxInputDialog）

从预设项中选择或自由输入，结果类型 `string`。

```csharp
var result = await provider.ShowComboBoxInputDialogAsync(new ComboBoxInputDialogSettings
{
    Title = "选择城市",
    Items = new[] { "北京", "上海", "广州", "深圳" },
    DefaultText = "北京",
    IsEditable = true, // false 时只能从预设项中选择
});
```

## API 参考

### DialogResult\<T\>（结果值）

命名空间：`ApeFree.ApeDialogs.Core`。不可变值类型（`readonly record struct`）。

| 成员 | 说明 |
|---|---|
| `T? Value` | 结果数据（取消时为 `default`） |
| `bool IsCanceled` | 是否被取消（用户取消或 token 取消） |
| `static DialogResult<T> Confirmed(T value)` | 构造确认结果 |
| `static DialogResult<T> Canceled` | 构造取消结果 |
| `T RequireValue()` | 取消时抛 `InvalidOperationException`，否则返回 `Value` |
| `TOut Match<TOut>(Func<T,TOut> onConfirmed, Func<TOut> onCanceled)` | 按状态分支处理 |

### ValidationResult（校验结果）

命名空间：`ApeFree.ApeDialogs.Core`。不可变值类型。

| 成员 | 说明 |
|---|---|
| `bool IsSuccess` | 是否通过 |
| `string? ErrorMessage` | 失败信息（成功时为 null） |
| `static ValidationResult Success` | 成功结果 |
| `static ValidationResult Failure(string errorMessage)` | 构造失败结果 |

### IDialog（对话框接口）

命名空间：`ApeFree.ApeDialogs.Core`。

| 成员 | 类型 | 说明 |
|---|---|---|
| `Showing` | `event EventHandler<DialogEventArgs>` | 显示前触发 |
| `Shown` | `event EventHandler<DialogEventArgs>` | 显示完成后触发 |
| `Dismissing` | `event EventHandler<DialogEventArgs>` | 销毁前触发 |
| `Dismissed` | `event EventHandler<DialogEventArgs>` | 销毁完成后触发 |
| `IsShown` | `bool` | 是否正在显示 |

### IDialog\<TResult\> / IDialogController\<TResult\>

```csharp
public interface IDialog<TResult> : IDialog
{
    Task<DialogResult<TResult>> ShowAsync(CancellationToken cancellationToken = default);
}

public interface IDialogController<TResult> : IDialog<TResult> // 实现方使用
{
    void Dismiss(DialogResult<TResult> result);
    void Cancel();
}
```

### IAdjustDialog\<T\>（实时调节）

滑块类调节 Dialog 的消费者接口（`CreateSliderDialog` / `CreateRangeSliderDialog` 返回该类型）。

```csharp
public interface IAdjustDialog<T> : IDialog<T>
{
    event EventHandler<ValueChangedEventArgs<T>>? ValueChanged; // 调节过程中持续触发
    T Value { get; }
}

public class ValueChangedEventArgs<T> : EventArgs // Core 命名空间
{
    public T NewValue { get; }
}
```

实现辅助基类 `BaseAdjustDialog<TView, TOption, TContext, TResult>`：实现方在调节动作中调用 `RaiseValueChanged(newValue)`，同时触发事件与 `Settings.OnValueChanged` 委托。

### IProgressDialog（进度）

```csharp
public interface IProgressDialog : IDialog<bool>
{
    bool IsIndeterminate { get; }
    void UpdateProgress(double progress, string? message = null); // progress: 0.0 ~ 1.0
}
```

### ValueRange\<T\>（值范围）

```csharp
public readonly record struct ValueRange<T>(T Start, T End); // Core 命名空间
```

### DialogSettings（配置基类）

命名空间：`ApeFree.ApeDialogs.Settings`。

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string` | `""` | 标题 |
| `Content` | `string?` | `null` | 提示语内容 |
| `Cancelable` | `bool` | `true` | 是否允许取消（关闭按钮/Esc） |
| `Theme` | `IDialogTheme?` | `null` | 配色主题，为空用 `DefaultTheme` |
| `DialogSize` | `ApeSize?` | `null` | 建议尺寸，为空由实现方决定 |
| `Font` | `DialogFont?` | `null` | 默认字体，为空由实现方决定 |
| `Duration` | `TimeSpan?` | `null` | 自动关闭时长，超时以取消关闭 |
| `OnShown` | `Action<IDialog>?` | `null` | 显示完成后回调 |
| `OnDismissed` | `Action<IDialog>?` | `null` | 销毁完成后回调 |
| `DefaultOptions` | `IReadOnlyList<DialogOption>` | 空 | 默认选项集合（子类覆写） |

```csharp
public abstract class DialogSettings<TResult> : DialogSettings
{
    public Func<TResult, ValidationResult>? Validator { get; init; } // 结果校验规则
    public Action<TResult>? OnValueChanged { get; init; }            // 调节类 Dialog 的实时值变化回调
}
```

> 所有属性均为 `init`，使用对象初始化器配置。

### DialogOption（选项）

命名空间：`ApeFree.ApeDialogs.Settings`。不可变 record，支持 `with` 派生。

```csharp
public sealed record DialogOption(
    string Text,
    DialogOptionTag Tag = DialogOptionTag.Neutral,
    bool IsEnabled = true,
    DialogOptionBehavior Behavior = DialogOptionBehavior.None,
    EventHandler<OptionSelectedEventArgs>? OnSelected = null);

public static DialogOption CreateCancelOption(string text = "Cancel"); // 构造"取消"选项
```

| 成员 | 说明 |
|---|---|
| `Text` | 选项文本 |
| `Tag` | 语义标签（决定配色） |
| `IsEnabled` | 是否可用 |
| `Behavior` | 内置行为：`None` / `Cancel`（取消关闭）/ `Clear`（清空输入）/ `SelectAll`（全选）/ `ReverseSelection`（反选）/ `SetCurrentTime`（设为当前时间） |
| `OnSelected` | 选中回调，参数 `OptionSelectedEventArgs`（含 `Dialog` 与 `Option`） |

`DialogOptionTag` 枚举：`Neutral`、`Positive`、`Negative`、`Functional`、`Special`。

### 主题（Theme）

命名空间：`ApeFree.ApeDialogs.Theme`。

```csharp
public interface IDialogTheme
{
    ApeColor GetColor(DialogOptionTag tag);
    ApeColor ReminderColor { get; } // 提示色（校验失败提示等）
}

public sealed class DefaultTheme : IDialogTheme // 内置默认主题
```

跨平台基础类型（替代 System.Drawing）：

| 类型 | 说明 |
|---|---|
| `ApeColor(byte A, byte R, byte G, byte B)` | 颜色，含 `FromArgb(int)` / `ToArgb()` |
| `ApeSize(int Width, int Height)` | 尺寸 |
| `DialogFont(string? Family, float Size, FontWeight Weight, bool Italic)` | 字体，`FontWeight` 为 `Normal`/`Bold` |

### DataEntrySheet / SheetField 家族

命名空间：`ApeFree.ApeDialogs.Settings`。

**DataEntrySheet**

| 成员 | 说明 |
|---|---|
| `IReadOnlyList<SheetField> Fields` | 字段集合 |
| `SheetField this[int index]` / `this[string title]` | 按索引/标题取值 |
| `DataEntrySheet AddField(SheetField field)` | 追加字段（可链式），标题重复或为空抛异常 |
| `ValidationResult Validate()` | 校验所有字段，返回第一个失败结果 |

**SheetField 基类**

| 成员 | 说明 |
|---|---|
| `FieldType FieldType` | 字段类型（抽象） |
| `string Title` | 标题 |
| `object? GetRawValue()` / `SetRawValue(object?)` | 原始值访问 |
| `ValidationResult Validate()` | 有效性检查 |

**`SheetField<T>`（强类型字段）**

| 成员 | 说明 |
|---|---|
| `T? Value` | 强类型字段值 |
| `Func<T?, ValidationResult>? Validator` | 自定义校验器 |

**字段类型与属性**

| 字段 | 类型参数 | 额外属性 |
|---|---|---|
| `TextField` | `string` | `MaximumLength`、`IsMultiline` |
| `PasswordField` | `string` | `MaximumLength`（默认 128）、`PasswordChar`（默认 ●） |
| `HexBytesField` | `byte[]` | `AllowEmpty`、`MaximumLength`、`MinimumLength`、`IsMultiline` |
| `FilePathField` | `string` | `BrowseButtonText` |
| `PicturePathField` | `string` | 继承 FilePathField |
| `NumberField` | `decimal` | `DecimalPlaces`、`Maximum`（默认 1000000）、`Minimum`（默认 -1000000） |
| `SingleChoiceField` | `object?` | `Items`、`ItemDisplayConverter` |
| `ComboBoxField` | `object?` | 继承 SingleChoiceField |
| `MultipleChoiceField` | `IReadOnlyList<object?>` | `Items`、`ItemDisplayConverter` |
| `DateTimeField` | `DateTime` | `DateTimeFormat`（默认 "yyyy-MM-dd HH:mm:ss"，24 小时制） |

### DialogProvider\<TContext\>

命名空间：`ApeFree.ApeDialogs`（根）。

**抽象方法（实现方必做）**

```csharp
IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, TContext? context = default);
IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, TContext? context = default);
IDialog<string> CreateInputDialog(InputDialogSettings settings, TContext? context = default);
IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, TContext? context = default);
IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default, TContext? context = default);
IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, TContext? context = default);
IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, TContext? context = default);
IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, TContext? context = default);
IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, TContext? context = default);
IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, TContext? context = default);
IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, TContext? context = default);
IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, TContext? context = default);
IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, TContext? context = default);
IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, TContext? context = default);
IDialog<bool> CreateToastDialog(ToastDialogSettings settings, TContext? context = default);
IDialog<bool> CreateTextDialog(TextDialogSettings settings, TContext? context = default);
IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, TContext? context = default);
IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, TContext? context = default);
IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, TContext? context = default);
```

**默认实现方法（基类自动获得，无需覆写）**

对应上述每个类型一个：`ShowMessageDialogAsync` / `ShowPromptDialogAsync` / `ShowInputDialogAsync` / `ShowPasswordDialogAsync` / `ShowSelectionDialogAsync<T>` / `ShowMultipleSelectionDialogAsync<T>` / `ShowDateTimeDialogAsync` / `ShowDataEntrySheetDialogAsync` / `ShowOpenFileDialogAsync` / `ShowOpenFolderDialogAsync` / `ShowNumberInputDialogAsync` / `ShowSliderDialogAsync` / `ShowRangeSliderDialogAsync` / `ShowProgressDialogAsync` / `ShowToastDialogAsync` / `ShowTextDialogAsync` / `ShowColorDialogAsync` / `ShowSaveFileDialogAsync` / `ShowComboBoxInputDialogAsync`。

签名模式：

```csharp
Task<DialogResult<TResult>> ShowXxxDialogAsync(XxxDialogSettings settings,
    TContext? context = default, CancellationToken cancellationToken = default);
```

另提供 `IDialogProvider<TContext>` 接口（便于测试 Mock 与 DI），`Theme` 属性（默认 `DefaultTheme.Instance`）可覆写。

## 实现方指南

实现方只需做两件事。

### 1. 继承 `BaseDialog<TView, TOption, TContext, TResult>`

```csharp
public class MyDialog<TResult> : BaseDialog<Control, Button, Control, TResult>
{
    public MyDialog(DialogSettings<TResult> settings, Control context) : base(settings, context) { }

    public override string Title { get => view.Text; set => view.Text = value; }
    public override string Content { get; set; }

    public override Button AddOption(DialogOption dialogOption)
    {
        var btn = new Button { Text = dialogOption.Text };
        btn.Click += (_, _) =>
        {
            if (dialogOption.Behavior == DialogOptionBehavior.Cancel) { Cancel(); return; } // 取消行为 → 直接关闭
            dialogOption.OnSelected?.Invoke(this, new OptionSelectedEventArgs(this, dialogOption));
        };
        return btn;
    }

    public override void ClearOptions() { /* 清空按钮 */ }

    protected override void ShowHandler() { view.Show(); }       // 显示视图
    protected override void DismissHandler() { view.Close(); }   // 销毁视图
    protected override void OnValidationFailed(ValidationResult result) { /* 抖动/Toast 提示 */ }
}
```

要点：

- 确认提交调用受保护的 `bool TrySubmit(TResult value)`：自动执行 `Settings.Validator`，通过则 `Dismiss(Confirmed)` 并返回 `true`；失败则回调 `OnValidationFailed` 并返回 `false`。
- 视图真正显示后调用 `RaiseShown()`（触发 `Shown` 事件与 `Settings.OnShown`）。
- `Duration` 自动关闭、取消 token 关闭、`IsShown` 状态维护均由基类内置。
- 参考实现：`tests/TestDialog.cs`。

### 2. 继承 `DialogProvider<TContext>`

```csharp
public class MyDialogProvider : DialogProvider<Control>
{
    public override IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, Control? context = default)
        => new MyDialog<bool>(settings, context);

    // …… 其余 18 个 CreateXxxDialog 同理，ShowXxxDialogAsync 由基类自动获得
}
```

参考实现：`tests/TestProvider.cs`。

## 依赖注入

```csharp
using Microsoft.Extensions.DependencyInjection;
using ApeFree.ApeDialogs;

var services = new ServiceCollection();
services.AddApeDialogs(new MyDialogProvider());

// 消费
IDialogProvider<Control> provider = sp.GetRequiredService<IDialogProvider<Control>>();
```

`AddApeDialogs<TContext>(provider)` 同时以具体类型与接口注册，便于替换与测试。

## WPF 实现库（ApeFree.ApeDialogs.WPF）

> 安装：`dotnet add package ApeFree.ApeDialogs.WPF`（WinForms 场景另加 `ApeFree.ApeDialogs.WPF.WinForms`）
> 目标框架：`net461` / `net8.0-windows`，Win11 Fluent 设计风格（圆角/亚克力/深浅色跟随/动画）。

```csharp
using ApeFree.ApeDialogs.WPF;

var provider = new ApeDialogsWpfProvider();

// WPF 宿主：Window 直接作为上下文
DialogResult<string> result = await provider.ShowInputDialogAsync(
    new InputDialogSettings { Title = "输入" }, new WindowOwner(window));
```

**WinForms 宿主**（桥包）：以 Form 作为上下文，其余 API 完全一致。

```csharp
using ApeFree.ApeDialogs.WPF;
using ApeFree.ApeDialogs.WPF.WinForms;

var provider = new ApeDialogsWpfProvider();

// Form 直接作为上下文（内部包装为 FormOwner，模态层级/居中自动处理）
DialogResult<string> result = await provider.ShowInputDialogAsync(
    new InputDialogSettings { Title = "输入" }, this);
```

19 种 Dialog 全部可用（系统文件对话框在 Win11 下使用原生 Fluent 界面）。

已知限制（WPF 平台）：

- `PasswordDialogSettings.PasswordChar` 无效（WPF PasswordBox 不支持自定义掩码字符）。
- `OpenFolderDialogSettings.MultiSelect` 无效（自绘目录浏览器返回当前浏览目录单路径）。
- Calendar 控件沿用 WPF 默认模板（Win11 重样式为后续优化项）。

## v0.x → v1.0 迁移对照

| v0.x | v1.0 |
|---|---|
| `Result<T>` + `UpdateResultData()` | `DialogResult<T>`（不可变 record struct，`Confirmed`/`Canceled`） |
| `IDialog.Show()` / `Dismiss(bool isCancel)` | `IDialog<T>.ShowAsync(ct)`、`Cancel()` / `Dismiss(result)` |
| 自定义 `DialogEventHandler` 委托 | 标准 `EventHandler<DialogEventArgs>` |
| 反射 + 方法名字符串的扩展方法 | 基类默认实现的 `ShowXxxDialogAsync` |
| `FormatCheckResult` / `ValidityCheck()` | `ValidationResult` / `Validate()` |
| `PrecheckResult`、`*Handler`/`*Callback` 混用 | `Validator`、统一 `OnXxx` 命名 |
| `GetDefaultOptionsHandler()` | `DefaultOptions` |
| `SheetField.Data` | `SheetField<T>.Value` |
| `System.Drawing.Color/Font/Size` | `ApeColor` / `DialogFont` / `ApeSize`（`IDialogTheme`） |
| `DialogFactory` 空壳单例 | 删除，改用 DI 注册 |
| 事件监听需持有 dialog 实例 | Settings 内 `OnShown`/`OnDismissed` 委托 + 声明式 `Duration` |
| 多选结果 `IEnumerable<T>` | `IReadOnlyList<T>` |
| 打开文件初始路径走方法参数 | `OpenFileDialogSettings.InitialPath` |

## 项目结构

```
ApeFree.ApeDialogs/                         核心标准库
  Core/                                     IDialog、IDialogController、BaseDialog、
                                            DialogResult、ValidationResult、DialogEventArgs
  Settings/                                 DialogSettings 家族、DialogOption、
                                            DataEntrySheet、SheetField 家族
  Theme/                                    ApeColor、ApeSize、DialogFont、
                                            IDialogTheme、DefaultTheme、DialogOptionTag
  IDialogProvider.cs / DialogProvider.cs
  Polyfills/IsExternalInit.cs               netstandard2.0 语言特性支撑
ApeFree.ApeDialogs.DependencyInjection/     DI 注册扩展（可选包）
tests/                                      xunit 单元测试（含 TestDialog/TestProvider 参考实现）
.github/workflows/ci.yml                    双 OS 构建 + 测试
```

## License

Apache-2.0
