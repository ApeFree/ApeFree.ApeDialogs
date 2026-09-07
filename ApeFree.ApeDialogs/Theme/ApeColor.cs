using System;

namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// 跨平台颜色值（ARGB 结构）。标准层不依赖 System.Drawing。
    /// </summary>
    public readonly record struct ApeColor(byte A, byte R, byte G, byte B)
    {
        public ApeColor(byte r, byte g, byte b) : this(255, r, g, b) { }

        #region 常量与预设颜色

        public static ApeColor Transparent => new(0, 0, 0, 0);
        public static ApeColor Black => new(0, 0, 0);
        public static ApeColor White => new(255, 255, 255);
        public static ApeColor Gray => new(128, 128, 128);
        public static ApeColor Red => new(255, 0, 0);
        public static ApeColor IndianRed => new(205, 92, 92);
        public static ApeColor Orange => new(255, 165, 0);
        public static ApeColor OrangeRed => new(255, 69, 0);
        public static ApeColor Yellow => new(255, 255, 0);
        public static ApeColor Gold => new(255, 215, 0);
        public static ApeColor Green => new(0, 128, 0);
        public static ApeColor ForestGreen => new(34, 139, 34);
        public static ApeColor Blue => new(0, 0, 255);
        public static ApeColor DodgerBlue => new(30, 144, 255);

        #endregion

        /// <summary>
        /// 从 RGB 分量构造颜色。
        /// </summary>
        public static ApeColor FromRgb(byte r, byte g, byte b) => new(255, r, g, b);

        /// <summary>
        /// 从 ARGB 分量构造颜色。
        /// </summary>
        public static ApeColor FromArgb(byte a, byte r, byte g, byte b) => new(a, r, g, b);

        /// <summary>
        /// 从 32 位 ARGB 整数构造颜色。
        /// </summary>
        public static ApeColor FromArgb(int argb)
        {
            return new ApeColor(
                (byte)((argb >> 24) & 0xFF),
                (byte)((argb >> 16) & 0xFF),
                (byte)((argb >> 8) & 0xFF),
                (byte)(argb & 0xFF));
        }

        /// <summary>
        /// 转换为 32 位 ARGB 整数。
        /// </summary>
        public int ToArgb() => (A << 24) | (R << 16) | (G << 8) | B;

        public override string ToString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";
    }
}
