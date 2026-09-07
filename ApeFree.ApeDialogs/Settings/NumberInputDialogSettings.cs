using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 数值输入对话框配置（类似 NumericUpDown 控件）。
    /// </summary>
    public class NumberInputDialogSettings : DialogSettings<decimal>
    {
        /// <summary>
        /// 最小值。
        /// </summary>
        public decimal Minimum { get; set; } = 0m;

        /// <summary>
        /// 最大值。
        /// </summary>
        public decimal Maximum { get; set; } = 100m;

        /// <summary>
        /// 步进值（点击上下按钮时的增减量）。
        /// </summary>
        public decimal Increment { get; set; } = 1m;

        /// <summary>
        /// 保留小数的位数。
        /// </summary>
        public int DecimalPlaces { get; set; }

        /// <summary>
        /// 默认值。
        /// </summary>
        public decimal DefaultValue { get; set; }

        /// <summary>
        /// 确认选项。
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; set; } = DialogOption.CreateCancelOption();

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }
}
