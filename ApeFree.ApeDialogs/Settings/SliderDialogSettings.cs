using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 单滑块调节对话框配置。
    /// 拖动过程中持续触发 <see cref="DialogSettings{TResult}.OnValueChanged"/> 实时影响外界；
    /// 确认后 <see cref="Core.DialogResult{T}.Value"/> 为最终值，取消不自动回滚。
    /// </summary>
    public class SliderDialogSettings : DialogSettings<decimal>
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
        /// 默认值。
        /// </summary>
        public decimal DefaultValue { get; set; } = 50m;

        /// <summary>
        /// 刻度步长。
        /// </summary>
        public decimal TickFrequency { get; set; } = 1m;

        /// <summary>
        /// 是否显示当前值标签。
        /// </summary>
        public bool ShowValueLabel { get; set; } = true;

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
