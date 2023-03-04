namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog选项信息
    /// </summary>
    public class DialogOption
    {
        /// <summary>
        /// 选项文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 选项是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 选项类型
        /// </summary>
        public DialogOptionType OptionType { get; set; }

        public DialogOption(string text = null, DialogOptionType optionType = DialogOptionType.Neutral, bool enabled = true)
        {
            Text = text;
            Enabled = enabled;
            OptionType = optionType;
        }
    }
}
