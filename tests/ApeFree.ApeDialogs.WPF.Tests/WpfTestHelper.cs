using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ApeFree.ApeDialogs.WPF.Theme;

namespace ApeFree.ApeDialogs.WPF.Tests
{
    public static class WpfTestHelper
    {
        private static readonly object LockObj = new();
        private static bool _initialized;

        /// <summary>
        /// 确保全局 WPF Application 实例在后台 STA 线程中常驻并初始化主题
        /// </summary>
        public static void EnsureApplication()
        {
            if (_initialized) return;

            lock (LockObj)
            {
                if (_initialized) return;

                var readyEvent = new ManualResetEventSlim(false);
                var thread = new Thread(() =>
                {
                    if (Application.Current is null)
                    {
                        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                        ThemeManager.EnsureLoaded(app);
                    }
                    readyEvent.Set();
                    System.Windows.Threading.Dispatcher.Run();
                })
                {
                    IsBackground = true
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                readyEvent.Wait(5000);
                _initialized = true;
            }
        }

        /// <summary>
        /// 在 WPF UI Dispatcher 线程中执行操作
        /// </summary>
        public static async Task RunOnUIAsync(Action action)
        {
            EnsureApplication();
            var dispatcher = Application.Current!.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action();
                return;
            }
            await dispatcher.InvokeAsync(action);
        }

        /// <summary>
        /// 在 WPF UI Dispatcher 线程中执行异步操作
        /// </summary>
        public static async Task RunOnUIAsync(Func<Task> asyncAction)
        {
            EnsureApplication();
            var dispatcher = Application.Current!.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                await asyncAction();
                return;
            }
            await dispatcher.InvokeAsync(asyncAction).Task.Unwrap();
        }
    }
}
