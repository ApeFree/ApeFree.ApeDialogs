# ApeFree.ApeDialogs

[![NuGet](https://img.shields.io/nuget/v/ApeFree.ApeDialogs.svg?style=flat-square)](https://www.nuget.org/packages/ApeFree.ApeDialogs)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

**ApeFree.ApeDialogs** 是一套跨平台的通用对话框标准抽象库（非具体 UI 实现）。它定义了对话框交互的统一契约——包括弹窗展示、结果返回、通用参数配置以及 `IDialogProvider` 抽象，让桌面、Web、移动端乃至命令行应用中的“弹窗”遵循同一套 API 语义，业务层无需与具体 UI 框架深度耦合。

---

## 🌟 核心特性

- **接口与实现解耦**：业务代码仅依赖统一的 `IDialogProvider` 接口，UI 展现层可自由替换（如 WPF、WinForms、MAUI 等）。
- **零 UI 框架强绑定**：标准核心层不依赖 `System.Drawing` 或特定 UI 控件库，使用自有轻量数据类型（如 `ApeColor`、`ApeSize`、`DialogFont`）。
- **丰富标准的对话框类型**：
  - 信息提示：`MessageDialog`、`ToastDialog`
  - 用户输入：`PromptDialog`、`InputDialog`、`NumberInputDialog`、`PasswordDialog`、`ComboBoxInputDialog`
  - 选择操作：`SelectionDialog`、`MultipleSelectionDialog`、`DateTimeDialog`、`ColorDialog`、`SliderDialog`、`RangeSliderDialog`
  - 文件/文件夹：`OpenFileDialog`、`SaveFileDialog`、`OpenFolderDialog`
  - 复合数据/进度：`DataEntrySheetDialog`（表单录入）、`ProgressDialog`（进度通知）、`TextDialog`（长文本展示）
- **多目标框架支持**：支持 `.NET Framework 4.5.2`、`.NET Standard 2.0`、`.NET 6.0`、`.NET 8.0` 及 `.NET 10.0`。

---

## 📦 安装

通过 NuGet 命令行安装：

```bash
dotnet add package ApeFree.ApeDialogs
```

或在项目文件（`.csproj`）中添加引用：

```xml
<PackageReference Include="ApeFree.ApeDialogs" Version="1.1.0.0-alpha61112" />
```

---

## 🚀 快速上手

### 1. 基础弹窗调用（以 `IDialogProvider<TContext>` 为例）

```csharp
using System;
using System.Threading.Tasks;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Settings;

// 假设 dialogProvider 是注入或实例化的具体提供器（如 WPF 实现包中的 ApeDialogsWpfProvider）
IDialogProvider<object> dialogProvider = ...;

// 1. 弹出消息框（支持配置委托扩展方法）
var result = await dialogProvider.ShowMessageDialogAsync(s =>
{
    s.Title = "提示";
    s.Content = "操作已成功完成！";
});
if (result.IsConfirmed)
{
    Console.WriteLine("用户确认了提示");
}

// 2. 弹出文本输入框
var inputResult = await dialogProvider.ShowInputDialogAsync(new InputDialogSettings
{
    Title = "用户登录",
    Content = "请输入您的用户名：",
});
if (inputResult.IsConfirmed)
{
    Console.WriteLine($"输入的用户名: {inputResult.Value}");
}

// 3. 弹出列表单选框
var fruits = new[] { "苹果", "香蕉", "橙子", "西瓜" };
var selectResult = await dialogProvider.ShowSelectionDialogAsync(
    new SelectionDialogSettings<string>
    {
        Title = "水果调查",
        Content = "请选择喜欢的水果",
    },
    collection: fruits,
    defaultSelectedItem: "苹果");
if (selectResult.IsConfirmed)
{
    Console.WriteLine($"用户选择了: {selectResult.Value}");
}
```

### 2. 高级表单弹窗 (`DataEntrySheet`)

```csharp
var sheet = new DataEntrySheet()
    .AddTextField("姓名")
    .AddNumberField("年龄", value: 18, minimum: 0, maximum: 150, decimalPlaces: 0);

var sheetResult = await dialogProvider.ShowDataEntrySheetDialogAsync(
    sheet,
    new DataEntrySheetDialogSettings
    {
        Title = "注册",
        Content = "请填写以下信息",
    });

if (sheetResult.IsConfirmed)
{
    var name = ((TextField)sheetResult.Value!["姓名"]).Value;
    var age = ((NumberField)sheetResult.Value["年龄"]).Value;
    Console.WriteLine($"姓名: {name}, 年龄: {age}");
}
```

---

## 🧩 扩展生态推荐

| 扩展包 | 描述 |
| :--- | :--- |
| **`ApeFree.ApeDialogs.WPF`** | 基于 Win11 Fluent 设计风格的 WPF 具体实现库（支持 WPF 与 WinForms） |
| **`ApeFree.ApeDialogs.WPF.WinForms`** | WinForms 应用宿主桥接包与 Form 扩展方法 |
| **`ApeFree.ApeDialogs.DependencyInjection`** | `Microsoft.Extensions.DependencyInjection` 服务注册集成包 |

---

## 📄 开源许可证

本项目基于 [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) 协议开源。