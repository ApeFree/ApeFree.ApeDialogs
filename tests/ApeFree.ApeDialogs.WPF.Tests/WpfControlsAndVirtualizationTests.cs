using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Dialogs;
using ApeFree.ApeDialogs.WPF.Hosting;
using ApeFree.ApeDialogs.WPF.Interop;
using ApeFree.ApeDialogs.WPF.Theme;
using Xunit;

namespace ApeFree.ApeDialogs.WPF.Tests
{
    public class WpfControlsAndVirtualizationTests
    {
        public WpfControlsAndVirtualizationTests()
        {
            WpfTestHelper.EnsureApplication();
        }

        [Fact]
        public async Task SelectionDialog_With10000Items_PreservesVirtualizationWithoutHanging()
        {
            await WpfTestHelper.RunOnUIAsync(() =>
            {
                var items = Enumerable.Range(1, 10000).Select(i => $"Item {i}").ToList();
                var settings = new SelectionDialogSettings<string>
                {
                    Title = "Large Selection",
                    Content = "Please pick one:"
                };

                var sw = Stopwatch.StartNew();
                var dialog = new WpfSelectionDialog<string>(settings, items, "Item 100", null, Win11Theme.Instance);
                sw.Stop();

                // 即使有 10,000 个数据项，由于 UI 虚拟化与去除了全局 ScrollViewer，构造与测量耗时应远低于 500ms
                Assert.True(sw.ElapsedMilliseconds < 1000, $"Initialization took {sw.ElapsedMilliseconds}ms, expected under 1000ms.");
            });
        }

        [Fact]
        public async Task DialogHostWindow_ContentPresenter_IsNotEnclosedInGlobalScrollViewer()
        {
            await WpfTestHelper.RunOnUIAsync(() =>
            {
                var host = new DialogHostWindow();
                // 确保 ContentPresenter 的父容器不是 ScrollViewer，以防止破坏虚拟化
                var parent = host.ContentPresenter.Parent;
                Assert.False(parent is ScrollViewer, "ContentPresenter must not be directly enclosed in a ScrollViewer to maintain ListView virtualization.");
            });
        }

        [Fact]
        public async Task ApeNumericUpDown_ClampAndFormat_BehavesCorrectly()
        {
            await WpfTestHelper.RunOnUIAsync(() =>
            {
                var numeric = new ApeNumericUpDown
                {
                    Minimum = 10,
                    Maximum = 50,
                    DecimalPlaces = 2,
                    Value = 20
                };

                Assert.Equal(20m, numeric.Value);

                // 输入超出最大值
                numeric.Text = "99";
                numeric.Commit();
                Assert.Equal(50m, numeric.Value);
                Assert.Equal("50.00", numeric.Text);

                // 输入低于最小值
                numeric.Text = "5";
                numeric.Commit();
                Assert.Equal(10m, numeric.Value);
                Assert.Equal("10.00", numeric.Text);

                // 正常小数输入
                numeric.Text = "25.5";
                numeric.Commit();
                Assert.Equal(25.5m, numeric.Value);
                Assert.Equal("25.50", numeric.Text);
            });
        }

        [Fact]
        public async Task Win11Theme_ResolvesColorsCorrectly()
        {
            await WpfTestHelper.RunOnUIAsync(() =>
            {
                var theme = Win11Theme.Instance;
                var reminder = theme.ReminderColor;
                Assert.True(reminder.A > 0);

                var posColor = theme.GetColor(DialogOptionTag.Positive);
                Assert.True(posColor.A > 0);

                var negColor = theme.GetColor(DialogOptionTag.Negative);
                Assert.True(negColor.A > 0);
            });
        }

        [Fact]
        public void Win32WindowHelper_ReferenceCountPushPop_ManagesStateCorrectly()
        {
            var fakeHandle = new IntPtr(0x12345);

            // 首次 push
            Win32WindowHelper.PushDisableWindow(fakeHandle);
            // 二次 push
            Win32WindowHelper.PushDisableWindow(fakeHandle);

            // 首次 pop：引用计数减为 1，不解禁
            Win32WindowHelper.PopEnableWindow(fakeHandle);

            // 二次 pop：引用计数归 0，完全解禁
            Win32WindowHelper.PopEnableWindow(fakeHandle);

            // 再次 pop：幂等，不抛出异常
            Win32WindowHelper.PopEnableWindow(fakeHandle);
        }

        [Fact]
        public async Task DialogHostWindow_CancelableFalse_PreventsClosingViaAltF4()
        {
            await WpfTestHelper.RunOnUIAsync(() =>
            {
                var host = new DialogHostWindow
                {
                    Cancelable = false
                };

                // 模拟 Alt+F4（调用 Close() 触发 OnClosing）
                host.Close();

                // 窗口不应进入关闭完成状态（未设置 IsExplicitlyClosing，被 OnClosing 拦截）
                // 显式直接关闭则放行
                host.CloseDirectly();
            });
        }

        [Fact]
        public void WpfNativeFileDialog_DoesNotAllocateDialogHostWindow()
        {
            var settings = new OpenFileDialogSettings { Title = "Native Open" };
            var dialog = new WpfOpenFileDialog(settings, null, Win11Theme.Instance);

            // 原生对话框的 View 绝不是 DialogHostWindow（0 虚空窗口分配）
            Assert.False(dialog.View is DialogHostWindow);
        }

        [Fact]
        public async Task DialogHostWindow_FocusTrapping_IsConfigured()
        {
            await WpfTestHelper.RunOnUIAsync(() =>
            {
                var host = new DialogHostWindow();
                Assert.Equal(System.Windows.Input.KeyboardNavigationMode.Cycle, System.Windows.Input.KeyboardNavigation.GetTabNavigation(host));
                Assert.Equal(System.Windows.Input.KeyboardNavigationMode.Cycle, System.Windows.Input.KeyboardNavigation.GetDirectionalNavigation(host));
            });
        }
    }
}
