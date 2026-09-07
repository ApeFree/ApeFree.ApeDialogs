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
- **多目标框架支持**：支持 `.NET Standard 2.0`、`.NET Framework 4.5.2+` 及 `.NET 8.0+`。

---

## 📦 安装

通过 NuGet 命令行安装：

```bash
dotnet add package ApeFree.ApeDialogs
```

或在项目文件（`.csproj`）中添加引用：

```xml
<PackageReference Include="ApeFree.ApeDialogs" Version="1.0.2.1-alpha260907" />
```

---

## 🚀 快速上手

### 1. 基础弹窗调用（以接口/实现类为例）

```csharp
using System;
using System.Threading.Tasks;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Settings;

// 假设 provider 是注入或实例化的具体 IDialogProvider（例如 WPF 实现包中的 ApeDialogsWpfProvider）
IDialogProvider dialogProvider = ...;

// 1. 弹出消息框
var result = dialogProvider.ShowMessage("操作已成功完成！", "提示");
if (!result.IsCanceled)
{
    Console.WriteLine("用户确认了提示");
}

// 2. 弹出文本输入框
var inputResult = dialogProvider.ShowPrompt("请输入您的用户名：", "用户登录");
if (inputResult.TryGetSuccessValue(out string username))
{
    Console.WriteLine($"输入的用户名: {username}");
}

// 3. 弹出列表单选框
var fruits = new[] { "苹果", "香蕉", "橙子", "西瓜" };
var selectResult = dialogProvider.ShowSelection(fruits, "请选择喜欢的水果", "水果调查");
if (selectResult.TryGetSuccessValue(out string selectedFruit))
{
    Console.WriteLine($"用户选择了: {selectedFruit}");
}
```

### 2. 高级表单弹窗 (`DataEntrySheet`)

```csharp
var sheet = new DataEntrySheet("用户注册信息");
sheet.AddField(new SheetField("姓名", typeof(string), isRequired: true));
sheet.AddField(new SheetField("年龄", typeof(int), defaultValue: 18));
sheet.AddField(new SheetField("启用通知", typeof(bool), defaultValue: true));

var sheetResult = dialogProvider.ShowDataEntrySheet(sheet, "请填写以下信息", "注册");
if (sheetResult.TryGetSuccessValue(out var values))
{
    Console.WriteLine($"姓名: {sheet.GetFieldValue("姓名")}");
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