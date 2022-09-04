using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class SelectionDialogSettings<T> : DialogSettings<T>
    {
        /// <summary>
        /// 确认选项文本
        /// </summary>
        public string ConfirmOptionText { get; set; } = "Confirm";

        /// <summary>
        /// 选项显示文本转换回调
        /// </summary>
        public Func<T, string> ItemDisplayTextConvertCallback { get; set; } = (item) => item.ToString();

        public override IEnumerable<DialogOption> GetOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(ConfirmOptionText, DialogOptionType.Positive),
                new DialogOption(CancelOptionText, DialogOptionType.Negative,Cancelable),
            };
        }
    }
}
