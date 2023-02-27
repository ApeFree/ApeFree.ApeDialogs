using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class MultipleSelectionDialogSettings<T> : DialogSettings<IEnumerable<T>>
    {
        /// <summary>
        /// 全选选项文本
        /// </summary>
        public string SelectAllOptionText { get; set; } = "All";

        /// <summary>
        /// 反选选项文本
        /// </summary>
        public string ReverseSelectedOptionText { get; set; } = "Reverse";

        /// <summary>
        /// 确认选项文本
        /// </summary>
        public string ConfirmOptionText { get; set; } = "Confirm";

        /// <summary>
        /// 选项显示文本转换回调
        /// </summary>
        public Func<T, string> ItemDisplayTextConvertCallback { get; set; } = (item) => item.ToString();

        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()
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
