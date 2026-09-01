# ApeFree.ApeDialogs.WPF

[![NuGet](https://img.shields.io/nuget/v/ApeFree.ApeDialogs.WPF.svg?style=flat-square)](https://www.nuget.org/packages/ApeFree.ApeDialogs.WPF)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

**ApeFree.ApeDialogs.WPF** 是 `ApeFree.ApeDialogs` 规范的 WPF 实现库。遵循 **Windows 11 Fluent Design** 视觉规范设计，内置暗黑/明亮主题切换、圆角阴影、亚克力模糊与微动效交互。

---

## 🌟 核心特性

- 🎨 **Win11 Fluent 视觉风格**：精致的排版、动画、主色强调与阴影效果。
- 🌓 **深色/浅色主题支持**：内置 `Brushes.Light` 与 `Brushes.Dark`，支持运行时一键切换或跟随系统。
- 🖼 **跨宿主支持**：原生支持纯 WPF 窗口环境，并可通过 `ApeFree.ApeDialogs.WPF.WinForms` 桥接包无缝运行在 WinForms 应用程序中（使用 `IWin32Window` Owner 自动建立层级与模态等待）。
- 🛠 **完整弹窗覆盖**：支持消息、输入、密码、颜色选取、单选/多选、时间日期、滑动条/区间滑动条、文件选择、长文本及动态表单等全部标准弹窗。

---

## 📦 安装

```bash
dotnet add package ApeFree.ApeDialogs.WPF
```

---

## 🚀 快速上手

### 1. 基础用法

```csharp
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.WPF;

// 创建 WPF DialogProvider 实例
var provider = new ApeDialogsWpfProvider();

// 弹出标准消息提示框
var result = provider.ShowMessage("操作成功！", "系统提示");

// 弹出颜色拾取对话框
var colorResult = provider.ShowColor("请选择背景颜色", "颜色配置");
if (colorResult.TryGetSuccessValue(out var color))
{
    Console.WriteLine($"选择了颜色: R={color.R}, G={color.G}, B={color.B}");
}
```

### 2. 指定父窗口宿主 (Owner)

```csharp
// 指定 WPF Window 为父窗口
provider.ShowPrompt("请输入内容", "标题", owner: this);
```

---

## 📄 开源许可证

本项目基于 [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0) 协议开源。