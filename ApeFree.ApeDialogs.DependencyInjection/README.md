# ApeFree.ApeDialogs.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/ApeFree.ApeDialogs.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/ApeFree.ApeDialogs.DependencyInjection)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

**ApeFree.ApeDialogs.DependencyInjection** 是 `ApeFree.ApeDialogs` 的 `Microsoft.Extensions.DependencyInjection` (IoC/DI) 集成扩展库。它为注册和注入 `IDialogProvider` 提供了标准化、链式友好的扩展方法。

---

## 📦 安装

```bash
dotnet add package ApeFree.ApeDialogs.DependencyInjection
```

---

## 🚀 快速上手

在应用启动时（如 `Generic Host`、`Prism`、`MVVM Toolkit` 或 `ASP.NET Core` / `WPF App.xaml.cs`）将对话框提供程序注册进 `IServiceCollection`：

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.DependencyInjection;
using ApeFree.ApeDialogs.WPF; // 假设使用 WPF 实现

var services = new ServiceCollection();

// 注册 DialogProvider 单例或瞬态服务
services.AddDialogProvider<ApeDialogsWpfProvider>();

// 或者使用工厂委托注册
services.AddDialogProvider(sp => new ApeDialogsWpfProvider());

var serviceProvider = services.BuildServiceProvider();

// 在 ViewModel 或业务服务中注入使用
public class MainViewModel
{
    private readonly IDialogProvider _dialogProvider;

    public MainViewModel(IDialogProvider dialogProvider)
    {
        _dialogProvider = dialogProvider;
    }

    public void OnButtonClick()
    {
        _dialogProvider.ShowMessage("Hello from DI injected DialogProvider!", "提示");
    }
}
```

---

## 📄 开源许可证

本项目基于 [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) 协议开源。