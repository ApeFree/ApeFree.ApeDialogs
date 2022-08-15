namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog选项信息
    /// </summary>
    public class DialogOption
    {
        public string Text { get; set; }
        public bool Enable { get; set; }
        public DialogOptionType OptionType { get; set; }

        public DialogOption(string text = null, DialogOptionType optionType = DialogOptionType.Neutral, bool enable = true)
        {
            Text = text;
            Enable = enable;
            OptionType = optionType;
        }
    }
}
