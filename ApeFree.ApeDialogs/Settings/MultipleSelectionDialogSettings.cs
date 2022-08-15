using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class MultipleSelectionDialogSettings : SelectionDialogSettings
    {
        /// <summary>
        /// 全选选项文本
        /// </summary>
        public string SelectAllOptionText { get; set; } = "All";

        /// <summary>
        /// 反选选项文本
        /// </summary>
        public string ReverseSelectedOptionText { get; set; } = "Reverse";

        public override IEnumerable<DialogOption> GetOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(SelectAllOptionText, DialogOptionType.Functional),
                new DialogOption(ReverseSelectedOptionText, DialogOptionType.Functional),
                new DialogOption(ConfirmOptionText, DialogOptionType.Positive),
                new DialogOption(CancelOptionText, DialogOptionType.Negative,Cancelable),
            };
        }
    }
}
