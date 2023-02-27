using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class PromptDialogSettings : DialogSettings<bool>
    {
        /// <summary>
        /// 积极选项文本
        /// </summary>
        public string PositiveOptionText { get; set; }

        /// <summary>
        /// 消极选项文本（Cancel）
        /// </summary>
        public string NegativeOptionText { get => CancelOptionText; set => CancelOptionText = value; }

        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(PositiveOptionText, DialogOptionType.Positive),
                new DialogOption(NegativeOptionText, DialogOptionType.Negative),
            };
        }
    }
}
