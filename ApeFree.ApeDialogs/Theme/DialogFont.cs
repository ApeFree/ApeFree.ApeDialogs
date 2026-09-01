namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// 字体粗细
    /// </summary>
    public enum FontWeight : byte
    {
        Normal,
        Bold,
    }

    /// <summary>
    /// 跨平台字体描述。标准层不依赖 System.Drawing。
    /// </summary>
    public readonly record struct DialogFont(string? Family, float Size, FontWeight Weight = FontWeight.Normal, bool Italic = false)
    {
        public static DialogFont Default => new(null, 12f, FontWeight.Normal, false);
    }
}
