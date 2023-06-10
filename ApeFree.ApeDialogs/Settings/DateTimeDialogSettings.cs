using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class DateTimeDialogSettings : DialogSettings<DateTime>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项
        /// </summary>
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, callback: DefaultCancelOptionHandler);

        /// <summary>
        /// 当前时间选项
        /// </summary>
        public DialogOption CurrentTimeOption { get; set; } = new DialogOption("Now", DialogOptionTag.Functional);

        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd hh:mm:ss";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Dialog默认选项集合</returns>
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { ConfirmOption, CancelOption, CurrentTimeOption };
    }
}
