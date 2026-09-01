namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// 默认主题（与 ApeForms 历史配色保持一致）。
    /// </summary>
    public sealed class DefaultTheme : IDialogTheme
    {
        public static DefaultTheme Instance { get; } = new DefaultTheme();

        private DefaultTheme() { }

        public ApeColor ReminderColor => new(0, 122, 204);

        public ApeColor GetColor(DialogOptionTag tag)
        {
            return tag switch
            {
                DialogOptionTag.Positive => new(34, 139, 34),    // ForestGreen
                DialogOptionTag.Negative => new(205, 92, 92),    // IndianRed
                DialogOptionTag.Special => new(147, 112, 219),   // MediumPurple
                _ => new(0, 122, 204),                           // Neutral / Functional
            };
        }
    }
}
