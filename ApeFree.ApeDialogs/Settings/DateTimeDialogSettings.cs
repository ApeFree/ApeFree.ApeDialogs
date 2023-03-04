using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class DateTimeDialogSettings : DialogSettings<DateTime>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionType.Positive);

        /// <summary>
        /// 取消选项
        /// </summary>
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionType.Negative);

        /// <summary>
        /// 当前时间选项
        /// </summary>
        public DialogOption CurrentTimeOption { get; set; } = new DialogOption("Now", DialogOptionType.Functional);

        /// <summary>
        /// 日期时间选择精度
        /// </summary>
        public DateTimePrecision Precision { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Dialog默认选项集合</returns>
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { ConfirmOption, CancelOption, CurrentTimeOption };
    }
}
