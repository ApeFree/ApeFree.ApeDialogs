namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// Dialog 选项类型标签。
    /// 用于约定选项的语义角色，实现方依据标签（结合主题）渲染样式。
    /// </summary>
    public enum DialogOptionTag : byte
    {
        /// <summary>
        /// 中性的
        /// </summary>
        Neutral,

        /// <summary>
        /// 积极的（如确认）
        /// </summary>
        Positive,

        /// <summary>
        /// 消极的（如取消）
        /// </summary>
        Negative,

        /// <summary>
        /// 功能性的（如全选、清空）
        /// </summary>
        Functional,

        /// <summary>
        /// 特殊的
        /// </summary>
        Special,
    }
}
