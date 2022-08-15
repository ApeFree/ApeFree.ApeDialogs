using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class DateTimeDialogSettings : DialogSettings
    {
        /// <summary>
        /// 确认选项文本
        /// </summary>
        public string ConfirmOptionText { get; set; } = "Confirm";

        /// <summary>
        /// 当前时间选项
        /// </summary>
        public string CurrentTimeOptionText { get; set; } = "Now";

        /// <summary>
        /// 日期时间选择精度
        /// </summary>
        public DateTimePrecision Precision { get; set; }

        public override IEnumerable<DialogOption> GetOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(CurrentTimeOptionText, DialogOptionType.Functional),
                new DialogOption(ConfirmOptionText, DialogOptionType.Positive),
                new DialogOption(CancelOptionText, DialogOptionType.Negative,Cancelable),
            };
        }
    }
}
