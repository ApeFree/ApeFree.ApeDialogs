using System;

namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// 跨平台颜色值（ARGB 结构）。标准层不依赖 System.Drawing。
    /// </summary>
    public readonly record struct ApeColor(byte A, byte R, byte G, byte B)
    {
        public ApeColor(byte r, byte g, byte b) : this(255, r, g, b) { }

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
