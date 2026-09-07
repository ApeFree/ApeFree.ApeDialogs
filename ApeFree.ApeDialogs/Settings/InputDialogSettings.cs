using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 输入对话框配置。
    /// </summary>
    public class InputDialogSettings : DialogSettings<string>
    {
        /// <summary>
        /// 确认选项。
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; set; } = DialogOption.CreateCancelOption();

        /// <summary>
        /// 清空选项。
        /// </summary>
        public DialogOption ClearOption { get; set; } = new("Clear", DialogOptionTag.Functional);

        /// <summary>
        /// 默认输入文本。
        /// </summary>
        public string DefaultText { get; set; } = string.Empty;

        /// <summary>
        /// 最大输入长度。
        /// </summary>
        public int MaximumLength { get; set; } = int.MaxValue;

        /// <summary>
        /// 是否多行输入。
        /// </summary>
        public bool IsMultiline { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption, ClearOption };
    }
}
