using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 提示对话框配置（Yes / No）。
    /// </summary>
    public class PromptDialogSettings : DialogSettings<bool>
    {
        /// <summary>
        /// 积极选项（Yes）。
        /// </summary>
        public DialogOption PositiveOption { get; init; } = new("Yes", DialogOptionTag.Positive);

        /// <summary>
        /// 消极选项（No）。
        /// </summary>
        public DialogOption NegativeOption { get; init; } = new("No", DialogOptionTag.Negative);

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { PositiveOption, NegativeOption };
    }
}
