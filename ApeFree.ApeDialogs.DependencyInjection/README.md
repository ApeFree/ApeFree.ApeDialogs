# ApeFree.ApeDialogs.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/ApeFree.ApeDialogs.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/ApeFree.ApeDialogs.DependencyInjection)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

**ApeFree.ApeDialogs.DependencyInjection** 是 `ApeFree.ApeDialogs` 的 `Microsoft.Extensions.DependencyInjection` (IoC/DI) 集成扩展库。它为注册和注入 `IDialogProvider<TContext>` / `DialogProvider<TContext>` 提供了标准化、链式友好的扩展方法。

- **目标框架**：`netstandard2.0`、`net8.0`、`net10.0`

---

## 📦 安装

```bash
dotnet add package ApeFree.ApeDialogs.DependencyInjection
```

或在项目文件（`.csproj`）中添加引用：

```xml
<PackageReference Include="ApeFree.ApeDialogs.DependencyInjection" Version="1.1.0.0-alpha61112" />
```

---

## 🚀 快速上手

在应用启动时（如 `Generic Host`、`Prism`、`MVVM Toolkit` 或 `WPF` / `WinForms` 应用入口）将对话框提供程序注册进 `IServiceCollection`：

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.WPF;        // 假设使用 WPF 实现
using ApeFree.ApeDialogs.WPF.Interop;

var services = new ServiceCollection();

// 方式一：按类型自动注册（由 DI 容器解析构造函数依赖，同时注册具体类型、基类与接口）
services.AddApeDialogs<IWin32Owner, ApeDialogsWpfProvider>();

// 方式二：直接注册已有实例
// services.AddApeDialogs(new ApeDialogsWpfProvider());

// 方式三：使用工厂委托注册
// services.AddApeDialogs<IWin32Owner, ApeDialogsWpfProvider>(sp => new ApeDialogsWpfProvider());

var serviceProvider = services.BuildServiceProvider();

// 在 ViewModel 或业务服务中注入使用
public class MainViewModel
{
    private readonly IDialogProvider<IWin32Owner> _dialogProvider;

    public MainViewModel(IDialogProvider<IWin32Owner> dialogProvider)
    {
        _dialogProvider = dialogProvider;
    }

    public async Task OnButtonClickAsync()
    {
        await _dialogProvider.ShowMessageDialogAsync(new MessageDialogSettings
        {
            Title = "提示",
            Content = "Hello from DI injected DialogProvider!",
        });
    }
}
```

---

## 📄 开源许可证

本项目基于 [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) 协议开源。