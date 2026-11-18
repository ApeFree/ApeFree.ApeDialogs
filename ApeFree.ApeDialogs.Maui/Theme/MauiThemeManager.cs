using System;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Theme
{
    /// <summary>
    /// MAUI 对话框主题管理器，负责亮暗色主题适配与设计令牌获取。
    /// </summary>
    public static class MauiThemeManager
    {
        /// <summary>
        /// 判断当前是否处于深色模式。
        /// </summary>
        public static bool IsDarkTheme
        {
            get
            {
                var appTheme = Application.Current?.RequestedTheme ?? AppTheme.Unspecified;
                if (appTheme == AppTheme.Unspecified)
                {
                    return AppInfo.RequestedTheme == AppTheme.Dark;
                }
                return appTheme == AppTheme.Dark;
            }
        }

        public static Color OverlayColor => IsDarkTheme
            ? Color.FromRgba(0, 0, 0, 160)
            : Color.FromRgba(0, 0, 0, 110);

        public static Color CardBackground => IsDarkTheme
            ? Color.FromArgb("#232323")
            : Color.FromArgb("#FFFFFF");

        public static Color TextPrimary => IsDarkTheme
            ? Color.FromArgb("#F3F3F3")
            : Color.FromArgb("#1C1C1E");

        public static Color TextSecondary => IsDarkTheme
            ? Color.FromArgb("#A0A0A0")
            : Color.FromArgb("#636366");

        public static Color StrokeDefault => IsDarkTheme
            ? Color.FromArgb("#38383A")
            : Color.FromArgb("#E5E5EA");

        public static Color AccentColor => IsDarkTheme
            ? Color.FromArgb("#0A84FF")
            : Color.FromArgb("#007AFF");

        public static Color AccentTextColor => Colors.White;

        public static Color ControlBackground => IsDarkTheme
            ? Color.FromArgb("#2C2C2E")
            : Color.FromArgb("#F2F2F7");

        public static Color ControlBorder => IsDarkTheme
            ? Color.FromArgb("#48484A")
            : Color.FromArgb("#D1D1D6");

        public static Color PositiveColor => IsDarkTheme
            ? Color.FromArgb("#30D158")
            : Color.FromArgb("#34C759");

        public static Color NegativeColor => IsDarkTheme
            ? Color.FromArgb("#FF453A")
            : Color.FromArgb("#FF3B30");

        public static Color SpecialColor => IsDarkTheme
            ? Color.FromArgb("#BF5AF2")
            : Color.FromArgb("#AF52DE");
    }
}
