using System.Collections.Generic;
using System.Drawing;

namespace ApeFree.ApeDialogs.Settings
{
    public class InputDialogSettings : DialogSettings<string>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionType.Positive);

        /// <summary>
        /// 取消选项
        /// </summary>
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionType.Negative, callback: DefaultCancelOptionHandler);

        /// <summary>
        /// 清空选项
        /// </summary>
        public DialogOption ClearOption { get; set; } = new DialogOption("Clear", DialogOptionType.Functional);

        /// <summary>
        /// 默认输入文本
        /// </summary>
        public string DefaultText { get; set; } = string.Empty;

        /// <summary>
        /// 允许输入内容为空
        /// </summary>
        public bool AllowEmpty { get; set; }

        /// <summary>
        /// 最大输入长度
        /// </summary>
        public int MaximumLength { get; set; } = 0;

        /// <summary>
        /// 最小输入长度
        /// </summary>
        public int MinimumLength { get; set; } = int.MaxValue;

        /// <summary>
        /// 是否多行
        /// </summary>
        public bool IsMultiline { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Dialog默认选项集合</returns>
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { ConfirmOption, CancelOption, ClearOption };

    }
}
