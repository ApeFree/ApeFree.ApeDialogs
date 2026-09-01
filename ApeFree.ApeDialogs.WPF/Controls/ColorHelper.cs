using System;
using System.Windows.Media;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// HSV 与 RGB 颜色换算（自绘颜色选择器用）。
    /// </summary>
    internal static class ColorHelper
    {
        /// <summary>
        /// HSV → RGB。h 范围 0~360，s/v 范围 0~1。
        /// </summary>
        public static Color HsvToRgb(double h, double s, double v)
        {
            h = ((h % 360) + 360) % 360;
            s = Math.Max(0, Math.Min(1, s));
            v = Math.Max(0, Math.Min(1, v));

            var c = v * s;
            var x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            var m = v - c;

            double r, g, b;
            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return Color.FromRgb((byte)Math.Round((r + m) * 255), (byte)Math.Round((g + m) * 255), (byte)Math.Round((b + m) * 255));
        }

        /// <summary>
        /// RGB → HSV。
        /// </summary>
        public static void RgbToHsv(byte r, byte g, byte b, out double h, out double s, out double v)
        {
            var rd = r / 255.0;
            var gd = g / 255.0;
            var bd = b / 255.0;

            var max = Math.Max(rd, Math.Max(gd, bd));
            var min = Math.Min(rd, Math.Min(gd, bd));
            var delta = max - min;

            v = max;
            s = max == 0 ? 0 : delta / max;

            if (delta == 0)
            {
                h = 0;
            }
            else if (max == rd)
            {
                h = 60 * (((gd - bd) / delta) % 6);
            }
            else if (max == gd)
            {
                h = 60 * (((bd - rd) / delta) + 2);
            }
            else
            {
                h = 60 * (((rd - gd) / delta) + 4);
            }

            if (h < 0)
            {
                h += 360;
            }
        }
    }
}
