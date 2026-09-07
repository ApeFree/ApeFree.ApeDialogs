using System;

namespace ApeFree.ApeDialogs.Theme
{
    /// <summary>
    /// 可自定义的 Dialog 配色主题。
    /// </summary>
    public class DialogTheme : IDialogTheme
    {
        /// <summary>
        /// 创建默认主题实例。
        /// </summary>
        public DialogTheme()
        {
            ReminderColor = new ApeColor(0, 122, 204);
            PositiveColor = new ApeColor(34, 139, 34);
            NegativeColor = new ApeColor(205, 92, 92);
            SpecialColor = new ApeColor(147, 112, 219);
            FunctionalColor = new ApeColor(0, 122, 204);
            NeutralColor = new ApeColor(0, 122, 204);
        }

        /// <summary>
        /// 使用指定提示色创建主题实例。
        /// </summary>
        public DialogTheme(ApeColor reminderColor) : this()
        {
            ReminderColor = reminderColor;
        }

        /// <summary>
        /// 提示色（如窗体强调色、校验失败提示）。
        /// </summary>
        public ApeColor ReminderColor { get; set; }

        /// <summary>
        /// 积极/肯定选项颜色（Confirm, Yes 等）。
        /// </summary>
        public ApeColor PositiveColor { get; set; }

        /// <summary>
        /// 消极/否定选项颜色（Cancel, No 等）。
        /// </summary>
        public ApeColor NegativeColor { get; set; }

        /// <summary>
        /// 特殊选项颜色。
        /// </summary>
        public ApeColor SpecialColor { get; set; }

        /// <summary>
        /// 功能选项颜色。
        /// </summary>
        public ApeColor FunctionalColor { get; set; }

        /// <summary>
        /// 中性选项颜色。
        /// </summary>
        public ApeColor NeutralColor { get; set; }

        /// <inheritdoc/>
        public virtual ApeColor GetColor(DialogOptionTag tag)
        {
            return tag switch
            {
                DialogOptionTag.Positive => PositiveColor,
                DialogOptionTag.Negative => NegativeColor,
                DialogOptionTag.Special => SpecialColor,
                DialogOptionTag.Functional => FunctionalColor,
                _ => NeutralColor,
            };
        }

        /// <summary>
        /// 从指定提示色快捷创建主题。
        /// </summary>
        public static DialogTheme FromReminderColor(ApeColor reminderColor) => new(reminderColor);
    }
}
