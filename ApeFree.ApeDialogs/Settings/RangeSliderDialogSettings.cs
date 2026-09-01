using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 双滑块范围调节对话框配置。
    /// 拖动过程中持续触发 <see cref="DialogSettings{TResult}.OnValueChanged"/> 实时影响外界；
    /// 确认后 <see cref="Core.DialogResult{T}.Value"/> 为最终范围，取消不自动回滚。
    /// </summary>
    public class RangeSliderDialogSettings : DialogSettings<ValueRange<decimal>>
    {
        /// <summary>
        /// 允许的最小值。
        /// </summary>
        public decimal Minimum { get; init; } = 0m;

        /// <summary>
        /// 允许的最大值。
        /// </summary>
        public decimal Maximum { get; init; } = 100m;

        /// <summary>
        /// 默认起始值。
        /// </summary>
        public decimal DefaultStart { get; init; } = 25m;

        /// <summary>
        /// 默认结束值。
        /// </summary>
        public decimal DefaultEnd { get; init; } = 75m;

        /// <summary>
        /// 刻度步长。
        /// </summary>
        public decimal TickFrequency { get; init; } = 1m;

        /// <summary>
        /// 是否显示当前范围标签。
        /// </summary>
        public bool ShowValueLabel { get; init; } = true;

        /// <summary>
        /// 确认选项。
        /// </summary>
        public DialogOption ConfirmOption { get; init; } = new("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; init; } = DialogOption.CreateCancelOption();

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }
}
