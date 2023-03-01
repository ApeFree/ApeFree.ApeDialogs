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
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionType.Negative);

        /// <summary>
        /// 清空选项
        /// </summary>
        public DialogOption ClearOption { get; set; } = new DialogOption("Cancel", DialogOptionType.Functional);

        /// <summary>
        /// 默认内容
        /// </summary>
        public string DefaultContent { get; set; } = string.Empty;

        /// <summary>
        /// 允许输入内容为空
        /// </summary>
        public bool AllowEmpty { get; set; }

        /// <summary>
        /// 最大输入长度
        /// </summary>
        public byte MaximumLength { get; set; }

        /// <summary>
        /// 最小输入长度
        /// </summary>
        public byte MinimumLength { get; set; }

        /// <summary>
        /// 是否多行
        /// </summary>
        public bool IsMultiline { get; set; }

        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { ConfirmOption, CancelOption, ClearOption };

    }
}
