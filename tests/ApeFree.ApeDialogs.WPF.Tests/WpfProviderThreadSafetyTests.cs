using System;
using System.Threading;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.WPF;
using ApeFree.ApeDialogs.WPF.Dialogs;
using Xunit;

namespace ApeFree.ApeDialogs.WPF.Tests
{
    public class WpfProviderThreadSafetyTests
    {
        public WpfProviderThreadSafetyTests()
        {
            WpfTestHelper.EnsureApplication();
        }

        [Fact]
        public async Task CreateMessageDialog_FromBackgroundThread_CreatesSafelyOnSta()
        {
            // 在非 STA 的工作线程池中直接调用 CreateMessageDialog
            var dialog = await Task.Run(() =>
            {
                var provider = new ApeDialogsWpfProvider();
                return provider.CreateMessageDialog(new MessageDialogSettings { Title = "Test", Content = "Background" });
            });

            Assert.NotNull(dialog);
            var wpfDialog = Assert.IsType<WpfMessageDialog>(dialog);
            Assert.Equal("Test", wpfDialog.Title);
            Assert.Equal("Background", wpfDialog.Content);
        }

        [Fact]
        public async Task CreateInputDialog_FromBackgroundThread_CreatesSafelyOnSta()
        {
            var dialog = await Task.Run(() =>
            {
                var provider = new ApeDialogsWpfProvider();
                return provider.CreateInputDialog(new InputDialogSettings { Title = "Input", DefaultText = "Default" });
            });

            Assert.NotNull(dialog);
            Assert.IsType<WpfInputDialog>(dialog);
        }

        [Fact]
        public async Task CreateToastDialog_FromBackgroundThread_CreatesSafelyOnSta()
        {
            var dialog = await Task.Run(() =>
            {
                var provider = new ApeDialogsWpfProvider();
                return provider.CreateToastDialog(new ToastDialogSettings { Content = "Toast Notice" });
            });

            Assert.NotNull(dialog);
            Assert.IsType<WpfToastDialog>(dialog);
        }

        [Fact]
        public async Task DialogProviderExtensions_ShowMessageDialogAsync_FromBackgroundThread_DispatchesCorrectly()
        {
            // 验证最常用的扩展方法在 Task.Run 下不会抛出 STA 线程异常
            var provider = new ApeDialogsWpfProvider();

            // 我们使用 CTS 立即取消以测试其调度流程且不阻塞测试
            using var cts = new CancellationTokenSource(50);

            var result = await Task.Run(async () =>
            {
                return await provider.ShowMessageDialogAsync(c =>
                {
                    c.Title = "Ext Title";
                    c.Content = "Ext Content";
                }, cancellationToken: cts.Token);
            });

            Assert.True(result.IsCanceled);
        }

        [Fact]
        public async Task DialogProviderExtensions_ShowToastDialogAsync_FromBackgroundThread_DispatchesCorrectly()
        {
            var provider = new ApeDialogsWpfProvider();
            using var cts = new CancellationTokenSource(50);

            var result = await Task.Run(async () =>
            {
                return await provider.ShowToastDialogAsync(c =>
                {
                    c.Content = "Toast in Task.Run";
                    c.Duration = TimeSpan.FromMilliseconds(100);
                }, cancellationToken: cts.Token);
            });

            Assert.True(result.IsCanceled);
        }
    }
}
