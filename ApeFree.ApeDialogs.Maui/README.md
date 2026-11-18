# ApeFree.ApeDialogs.Maui

ApeDialogs 的 **.NET MAUI 跨平台实现库**。遵循 ApeDialog 统一规范，为 Android、iOS、macOS (Mac Catalyst) 与 Windows 提供一致、强类型、声明式且无异常抛出的对话框体验。

- **目标平台**：Android、iOS、macOS (Mac Catalyst)、Windows (WinUI 3)
- **目标框架**：`net8.0-android`, `net8.0-windows10.0.19041.0`, `net8.0-ios`, `net8.0-maccatalyst`
- **零外部 UI 依赖**：纯自研轻量级宿主浮层（`MauiDialogHost`）+ `Microsoft.Maui.Graphics` 自绘高级控件（双滑块、调色板）
- **授权协议**：Apache-2.0

---

## 核心特性

- **19 种标准 Dialog 全覆盖**：消息、输入、单选、多选、日期时间、数值微调、双滑块、进度条、调色板、动态多字段表单及系统文件选择。
- **纯自研 Window/Page 遮罩宿主**：无缝融入现有页面树，支持半透明遮罩背景、圆角自适应卡片、阴影、进出场淡入缩放动画与校验失败左右抖动反馈。
- **软键盘与触控友好**：内置可滚动容器防遮挡，标准 40px 高度触控按钮。
- **语义配色与暗黑模式**：根据系统 `AppTheme` 自动切换亮暗调色板；选项按钮根据 `DialogOptionTag` 自动应用 Positive（强调色）、Negative（红色）、Special（紫色）等语义风格。
- **无异常与声明式**：用户取消或 Token 超时一律返回 `DialogResult<T>.Canceled`，不抛异常。

---

## 快速上手

### 1. 注册服务 (`MauiProgram.cs`)

在 `MauiProgram.CreateMauiApp` 中链式调用 `UseApeDialogs()`：

```csharp
using ApeFree.ApeDialogs.Maui.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseApeDialogs(); // 注册 ApeDialogs MAUI 提供器

        return builder.Build();
    }
}
```

### 2. 消费使用

在 ViewModel 或 Page 代码中直接注入并调用：

```csharp
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Settings;

public class DemoViewModel
{
    private readonly IDialogProvider<IMauiDialogContext> _dialogProvider;

    public DemoViewModel(IDialogProvider<IMauiDialogContext> dialogProvider)
    {
        _dialogProvider = dialogProvider;
    }

    public async Task PromptUserNameAsync()
    {
        var result = await _dialogProvider.ShowInputDialogAsync(new InputDialogSettings
        {
            Title = "个人信息",
            Content = "请输入您的昵称：",
            Duration = TimeSpan.FromSeconds(60),
            Validator = name => string.IsNullOrWhiteSpace(name)
                ? ValidationResult.Failure("昵称不能为空")
                : ValidationResult.Success
        });

        result.Match(
            onConfirmed: name => Console.WriteLine($"欢迎, {name}"),
            onCanceled: () => Console.WriteLine("用户取消了输入"));
    }
}
```

---

## 19 种标准 Dialog 支持清单

| # | 对话框 | 结果类型 | 说明 |
|---|---|---|---|
| 1 | 消息对话框 | `bool` | 确定提示，结果恒为 true |
| 2 | 提示对话框 | `bool` | Yes/No 二选一确认 |
| 3 | 输入对话框 | `string` | 支持单行/多行，限制长度与即时校验 |
| 4 | 密码对话框 | `string` | 密码输入，支持明密文小眼睛切换 |
| 5 | 单选对话框 | `T` | 列表单项选择，支持泛型与预选 |
| 6 | 多选对话框 | `IReadOnlyList<T>` | 支持 CheckBox 勾选，内置全选与反选 |
| 7 | 日期时间对话框 | `DateTime` | 支持 Date、Time 与 DateTime 组合 |
| 8 | 多字段表单对话框 | `DataEntrySheet` | 支持 10 种字段组件与双层数据有效性校验 |
| 9 | 打开文件对话框 | `IReadOnlyList<string>` | 原生跨平台 `FilePicker` 选取文件 |
| 10 | 打开文件夹对话框 | `IReadOnlyList<string>` | 目录浏览与常用沙盒目录快捷选择 |
| 11 | 数值微调对话框 | `decimal` | 带加减步进按钮的 `ApeNumericUpDown` |
| 12 | 单滑块调节对话框 | `decimal` | 滑动实时触发 `ValueChanged` 回调 |
| 13 | 双滑块范围调节对话框 | `ValueRange<decimal>` | 基于 `Maui.Graphics` 纯代码自绘双滑块 |
| 14 | 进度对话框 | `bool` | 支持转圈与进度条，跨线程刷新，1.0 自动关闭 |
| 15 | 轻提示对话框 (Toast) | `bool` | 浮动非模态卡片，自动淡出 |
| 16 | 长文本查看对话框 | `bool` | 滚动查看与复制长篇文本内容 |
| 17 | 颜色选择对话框 | `ApeColor` | 基于 `Maui.Graphics` 自绘 SV 调色板与色相条 |
| 18 | 保存文件对话框 | `string` | 文件保存路径与文件名确认 |
| 19 | 可编辑下拉输入对话框 | `string` | 自由输入兼顾预设候选项点选 |

---

## 平台限制与说明

- **文件系统沙盒**：移动端（Android/iOS）由于操作系统严格的沙盒隔离机制，文件夹选择聚焦于应用沙盒目录（`AppDataDirectory` / `CacheDirectory`）与系统授权范围。
- **软键盘交互**：所有复杂对话框与动态表单均默认置于滚动视图中，防止由于软键盘弹出而遮挡确认操作按钮。
