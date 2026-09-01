namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// 值范围（不可变值类型），如双滑块调节的起止值。
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public readonly record struct ValueRange<T>(T Start, T End)
    {
        public override string ToString() => $"{Start} ~ {End}";
    }
}
