namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// Dialog 配色主题。实现方依据主题渲染对话框与选项样式。
    /// </summary>
    public interface IDialogTheme
    {
        /// <summary>
        /// 依据选项标签获取颜色。
        /// </summary>
        ApeColor GetColor(DialogOptionTag tag);

        /// <summary>
        /// 提示色（如校验失败提示、强调文本）。
        /// </summary>
        ApeColor ReminderColor { get; }
    }
}
