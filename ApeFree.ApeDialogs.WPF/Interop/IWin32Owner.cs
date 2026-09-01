using System;
using System.Windows;

namespace ApeFree.ApeDialogs.WPF.Interop
{
    /// <summary>
    /// 对话框宿主窗口的抽象（Win32 句柄级别），使 WPF 与 WinForms 应用共用同一套 Provider。
    /// </summary>
    public interface IWin32Owner
    {
        /// <summary>
        /// 宿主窗口句柄。
        /// </summary>
        IntPtr Handle { get; }
    }

    /// <summary>
    /// WPF 窗口宿主适配器。
    /// </summary>
    public sealed class WindowOwner : IWin32Owner
    {
        private readonly Window _window;
        private IntPtr? _cachedHandle;

        /// <summary>
        /// 构造 WPF 窗口宿主。
        /// </summary>
        public WindowOwner(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        /// <inheritdoc/>
        public IntPtr Handle
        {
            get
            {
                if (_cachedHandle is null)
                {
                    _cachedHandle = new System.Windows.Interop.WindowInteropHelper(_window).Handle;
                }

                return _cachedHandle.Value;
            }
        }
    }
}
