using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 时间精度。
    /// </summary>
    public enum DateTimePrecision : byte
    {
        DateTime,
        Date,
        Time,
    }

    /// <summary>
    /// 日期时间对话框配置。
    /// </summary>
    public class DateTimeDialogSettings : DialogSettings<DateTime>
    {
        /// <summary>
        /// 确认选项。
        /// </summary>
        public DialogOption ConfirmOption { get; init; } = new("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; init; } = DialogOption.CreateCancelOption();

        /// <summary>
        /// 当前时间选项。
        /// </summary>
        public DialogOption CurrentTimeOption { get; init; } = new("Now", DialogOptionTag.Functional);

        /// <summary>
        /// 日期时间格式（24 小时制）。
        /// </summary>
        public string DateTimeFormat { get; init; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 默认时间。
        /// </summary>
        public DateTime DefaultDateTime { get; init; } = new(2000, 1, 1);

        /// <summary>
        /// 时间精度。
        /// </summary>
        public DateTimePrecision Precision { get; init; } = DateTimePrecision.DateTime;

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption, CurrentTimeOption };
    }
}
