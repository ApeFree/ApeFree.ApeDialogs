namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// 跨平台尺寸值（逻辑像素）。标准层不依赖 System.Drawing。
    /// </summary>
    public readonly record struct ApeSize(int Width, int Height)
    {
        public static ApeSize Empty => default;

        public bool IsEmpty => Width == 0 && Height == 0;

        public override string ToString() => $"{Width} x {Height}";
    }
}
