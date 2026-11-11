using System;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 原生通用文件/文件夹对话框基类：直接利用 Windows Shell 原生 Common Item Dialog。
    /// 彻底剥离虚空宿主窗口（DialogHostWindow）的分配损耗，
    /// 并通过 Win32 句柄无缝支持 WPF Window 与 WinForms Form 作为模态所有者。
    /// </summary>
    /// <typeparam name="TSettings">配置项类型</typeparam>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public abstract class WpfNativeFileDialogBase<TSettings, TResult> : BaseDialog<object, object, IWin32Owner, TResult>
        where TSettings : DialogSettings<TResult>
    {
        private static readonly MethodInfo? RunDialogMethod = typeof(Microsoft.Win32.CommonDialog).GetMethod(
            "RunDialog",
            BindingFlags.Instance | BindingFlags.NonPublic);

        protected WpfNativeFileDialogBase(TSettings settings, IWin32Owner? owner)
            : base(settings, owner)
        {
            View = this;
        }

        /// <inheritdoc/>
        public override string Title
        {
            get => Settings.Title;
            set => Settings.Title = value;
        }

        /// <inheritdoc/>
        public override string Content
        {
            get => Settings.Content ?? string.Empty;
            set => Settings.Content = value;
        }

        /// <inheritdoc/>
        public override object AddOption(DialogOption dialogOption) => null!;

        /// <inheritdoc/>
        public override void ClearOptions()
        {
        }

        /// <inheritdoc/>
        protected override void ShowHandler()
        {
            var dispatcher = ApeDialogsWpfProvider.EnsureDispatcher();
            if (!dispatcher.CheckAccess())
            {
                dispatcher.InvokeAsync(ShowHandler);
                return;
            }

            RaiseShown();
            ShowNativeDialog();
        }

        /// <summary>
        /// 执行原生对话框展示与结果处理。
        /// </summary>
        protected abstract void ShowNativeDialog();

        /// <inheritdoc/>
        protected override void DismissHandler()
        {
            // Windows 原生对话框由操作系统管理销毁
        }

        /// <summary>
        /// 解析当前有效的父窗口 Win32 句柄（优先使用 Context，次选 MainWindow）。
        /// </summary>
        protected IntPtr ResolveOwnerHwnd()
        {
            if (Context is not null && Context.Handle != IntPtr.Zero)
            {
                return Context.Handle;
            }

            if (Application.Current?.MainWindow is Window mainWindow && mainWindow.IsVisible)
            {
                var handle = new WindowInteropHelper(mainWindow).Handle;
                if (handle != IntPtr.Zero)
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 向 CommonDialog 传入任意 Win32 HWND（支持 WPF 与 WinForms 宿主）并展示。
        /// </summary>
        protected static bool? ShowCommonDialog(Microsoft.Win32.CommonDialog dialog, IntPtr hwndOwner)
        {
            if (dialog is null) throw new ArgumentNullException(nameof(dialog));

            if (hwndOwner != IntPtr.Zero && RunDialogMethod is not null)
            {
                return (bool?)RunDialogMethod.Invoke(dialog, new object[] { hwndOwner });
            }

            return dialog.ShowDialog();
        }
    }
}
