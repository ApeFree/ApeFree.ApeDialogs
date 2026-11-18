using System;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Controls
{
    /// <summary>
    /// HSV 与 RGB 颜色模型互转数学辅助。
    /// </summary>
    internal static class ColorMathHelper
    {
        public static void HsvToRgb(float h, float s, float v, out byte r, out byte g, out byte b)
        {
            if (s <= 0.00001f)
            {
                var gray = (byte)(v * 255f);
                r = gray;
                g = gray;
                b = gray;
                return;
            }

            var hueSector = (h % 360f) / 60f;
            if (hueSector < 0) hueSector += 6f;

            var i = (int)hueSector;
            var f = hueSector - i;
            var p = v * (1f - s);
            var q = v * (1f - (s * f));
            var t = v * (1f - (s * (1f - f)));

            float rf, gf, bf;
            switch (i)
            {
                case 0: rf = v; gf = t; bf = p; break;
                case 1: rf = q; gf = v; bf = p; break;
                case 2: rf = p; gf = v; bf = t; break;
                case 3: rf = p; gf = q; bf = v; break;
                case 4: rf = t; gf = p; bf = v; break;
                default: rf = v; gf = p; bf = q; break;
            }

            r = (byte)Math.Max(0, Math.Min(255, (int)(rf * 255f)));
            g = (byte)Math.Max(0, Math.Min(255, (int)(gf * 255f)));
            b = (byte)Math.Max(0, Math.Min(255, (int)(bf * 255f)));
        }

        public static void RgbToHsv(byte r, byte g, byte b, out float h, out float s, out float v)
        {
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;

            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float delta = max - min;

            v = max;
            s = max <= 0.0001f ? 0 : delta / max;

            if (delta <= 0.00001f)
            {
                h = 0;
                return;
            }

            if (rf >= max)
            {
                h = (gf - bf) / delta;
            }
            else if (gf >= max)
            {
                h = 2f + ((bf - rf) / delta);
            }
            else
            {
                h = 4f + ((rf - gf) / delta);
            }

            h *= 60f;
            if (h < 0) h += 360f;
        }
    }
}
