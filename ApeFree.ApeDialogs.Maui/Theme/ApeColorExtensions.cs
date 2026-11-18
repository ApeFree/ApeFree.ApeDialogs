using System;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Theme
{
    /// <summary>
    /// ApeColor 与 MAUI Color 转换扩展。
    /// </summary>
    public static class ApeColorExtensions
    {
        /// <summary>
        /// 将 ApeColor 转换为 MAUI Color。
        /// </summary>
        public static Color ToMauiColor(this ApeColor color)
        {
            return Color.FromRgba(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// 将 MAUI Color 转换为 ApeColor。
        /// </summary>
        public static ApeColor ToApeColor(this Color color)
        {
            return new ApeColor(
                (byte)(color.Alpha * 255f),
                (byte)(color.Red * 255f),
                (byte)(color.Green * 255f),
                (byte)(color.Blue * 255f));
        }
    }
}
