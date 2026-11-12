# ApeFree.ApeDialogs.WPF.WinForms

[![NuGet](https://img.shields.io/nuget/v/ApeFree.ApeDialogs.WPF.WinForms.svg?style=flat-square)](https://www.nuget.org/packages/ApeFree.ApeDialogs.WPF.WinForms)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

**ApeFree.ApeDialogs.WPF.WinForms** 是 `ApeFree.ApeDialogs.WPF` 在 **Windows Forms (WinForms)** 应用程序中的桥接与扩展库。让传统的 WinForms 项目无需改造即可享受到现代 Win11 Fluent 风格的 WPF 对话框体验。

- **目标框架**：`net452`、`net461`、`net472`、`net6.0-windows`、`net8.0-windows`、`net10.0-windows`

---

## 🌟 核心特性

- 🔗 **窗口句柄桥接 (`FormOwner`)**：自动将 WinForms `Form` 包装为 WPF 模态对话框宿主（`IWin32Owner`），确保对话框居中、置顶以及正确的模态阻塞。
- 🪄 **原生体验的扩展方法**：为 `ApeDialogsWpfProvider` 提供接收 `Form? owner` 参数的 19 种 `ShowXxxDialogAsync` 扩展方法，免去手动包装 `new FormOwner(this)`。

---

## 📦 安装

```bash
dotnet add package ApeFree.ApeDialogs.WPF.WinForms
```

或在项目文件（`.csproj`）中添加引用：

```xml
<PackageReference Include="ApeFree.ApeDialogs.WPF.WinForms" Version="1.1.0.0-alpha61112" />
```

---

## 🚀 快速上手

```csharp
using System;
using System.Windows.Forms;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.WPF;
using ApeFree.ApeDialogs.WPF.WinForms;

public partial class MainForm : Form
{
    private readonly ApeDialogsWpfProvider _dialogProvider = new();

    public MainForm()
    {
        InitializeComponent();
    }

    private async void btnShow_Click(object sender, EventArgs e)
    {
        // 直接传入当前 Form 作为宿主，弹出现代 Fluent 对话框
        await _dialogProvider.ShowMessageDialogAsync(new MessageDialogSettings
        {
            Title = "系统提示",
            Content = "这是一条来自 WinForms 的现代化 Fluent 风格弹窗！",
        }, this);

        var inputResult = await _dialogProvider.ShowInputDialogAsync(new InputDialogSettings
        {
            Title = "设备初始化",
            Content = "请输入设备名称：",
        }, this);

        if (inputResult.IsConfirmed)
        {
            MessageBox.Show(this, $"您输入的名称: {inputResult.Value}");
        }
    }
}
```

---

## 📄 开源许可证

本项目基于 [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) 协议开源。