using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class PromptDialogSettings : DialogSettings<bool>
    {
        /// <summary>
        /// 积极选项
        /// </summary>
        public DialogOption PositiveOption { get; set; } = new DialogOption("Yes", DialogOptionType.Positive);

        /// <summary>
        /// 消极选项
        /// </summary>
        public DialogOption NegativeOption { get; set; } = new DialogOption("No", DialogOptionType.Negative);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Dialog默认选项集合</returns>
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { PositiveOption, NegativeOption };
    }
}
