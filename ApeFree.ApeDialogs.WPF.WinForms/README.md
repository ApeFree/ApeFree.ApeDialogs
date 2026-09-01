# ApeFree.ApeDialogs.WPF.WinForms

[![NuGet](https://img.shields.io/nuget/v/ApeFree.ApeDialogs.WPF.WinForms.svg?style=flat-square)](https://www.nuget.org/packages/ApeFree.ApeDialogs.WPF.WinForms)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

**ApeFree.ApeDialogs.WPF.WinForms** 是 `ApeFree.ApeDialogs.WPF` 在 **Windows Forms (WinForms)** 应用程序中的桥接与扩展库。让传统的 WinForms 项目无需改造即可享受到现代 Win11 Fluent 风格的 WPF 对话框体验。

---

## 🌟 核心特性

- 🔗 **窗口句柄桥接 (`FormOwner`)**：自动将 WinForms `Form` / `IWin32Window` 包装为 WPF 模态对话框宿主，确保对话框居中、置顶以及正确的模态阻塞。
- 🪄 **原生体验的扩展方法**：为 `System.Windows.Forms.Form` 提供直观的 `this.ShowMessage(...)`、`this.ShowPrompt(...)` 等快捷方法。

---

## 📦 安装

```bash
dotnet add package ApeFree.ApeDialogs.WPF.WinForms
```

---

## 🚀 快速上手

```csharp
using System.Windows.Forms;
using ApeFree.ApeDialogs.WPF;
using ApeFree.ApeDialogs.WPF.WinForms;

public partial class MainForm : Form
{
    private readonly ApeDialogsWpfProvider _dialogProvider = new();

    public MainForm()
    {
        InitializeComponent();
    }

    private void btnShow_Click(object sender, EventArgs e)
    {
        // 直接使用扩展方法以当前 Form 为宿主弹出现代 Fluent 对话框
        var result = this.ShowMessage("这是一条来自 WinForms 的现代化 Fluent 风格弹窗！", "系统提示");
        
        var inputResult = this.ShowPrompt("请输入设备名称：", "设备初始化");
        if (inputResult.TryGetSuccessValue(out var name))
        {
            MessageBox.Show($"您输入的名称: {name}");
        }
    }
}
```

---

## 📄 开源许可证

本项目基于 [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) 协议开源。