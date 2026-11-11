using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace ApeFree.ApeDialogs.WPF.Interop
{
    /// <summary>
    /// Win32 窗口辅助（宿主窗口居中定位、模态禁用等）。
    /// </summary>
    internal static class Win32WindowHelper
    {
        private static readonly System.Collections.Generic.Dictionary<IntPtr, int> DisabledWindowCounts = new();
        private static readonly object WindowLock = new();

        /// <summary>
        /// 安全增加宿主窗口禁用计数（仅当首次被禁用时真正调用 Win32 API）。
        /// 避免多层对话框嵌套导致宿主窗口被内层对话框提前解禁。
        /// </summary>
        public static void PushDisableWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return;

            lock (WindowLock)
            {
                DisabledWindowCounts.TryGetValue(hWnd, out int count);
                if (count == 0)
                {
                    EnableWindow(hWnd, false);
                }
                DisabledWindowCounts[hWnd] = count + 1;
            }
        }

        /// <summary>
        /// 安全减少宿主窗口禁用计数（仅当计数归零时真正解禁宿主窗口并激活）。
        /// </summary>
        public static void PopEnableWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return;

            lock (WindowLock)
            {
                if (DisabledWindowCounts.TryGetValue(hWnd, out int count))
                {
                    if (count <= 1)
                    {
                        DisabledWindowCounts.Remove(hWnd);
                        EnableWindow(hWnd, true);
                        SetActiveWindow(hWnd);
                    }
                    else
                    {
                        DisabledWindowCounts[hWnd] = count - 1;
                    }
                }
            }
        }

        /// <summary>
        /// 将窗口居中于宿主句柄之上（无 WPF Owner 依赖，适用于 WinForms 互操作场景）。
        /// </summary>
        public static void CenterOverOwner(Window window, IntPtr ownerHandle)
        {
            if (ownerHandle == IntPtr.Zero)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                return;
            }

            if (!GetWindowRect(ownerHandle, out var rect))
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                return;
            }

            var ownerWidth = rect.Right - rect.Left;
            var ownerHeight = rect.Bottom - rect.Top;

            double dpiScaleX = 1.0;
            double dpiScaleY = 1.0;

            try
            {
                var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
                if (hwnd != IntPtr.Zero)
                {
                    var hwndSource = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
                    if (hwndSource?.CompositionTarget is not null)
                    {
                        dpiScaleX = hwndSource.CompositionTarget.TransformToDevice.M11;
                        dpiScaleY = hwndSource.CompositionTarget.TransformToDevice.M22;
                    }
                }

                if (dpiScaleX <= 1.0 && dpiScaleY <= 1.0)
                {
                    var source = PresentationSource.FromVisual(window);
                    if (source?.CompositionTarget is not null)
                    {
                        dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                        dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
                    }
                }
            }
            catch
            {
                // Fallback to 1.0
            }

            if (dpiScaleX <= 0) dpiScaleX = 1.0;
            if (dpiScaleY <= 0) dpiScaleY = 1.0;

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = (rect.Left / dpiScaleX) + ((ownerWidth / dpiScaleX) - window.Width) / 2.0;
            window.Top = (rect.Top / dpiScaleY) + ((ownerHeight / dpiScaleY) - window.Height) / 2.0;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    }
}
