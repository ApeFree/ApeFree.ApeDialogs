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
        public DialogOption ConfirmOption { get; init; } = new("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; init; } = DialogOption.CreateCancelOption();

        /// <summary>
        /// 清空选项。
        /// </summary>
        public DialogOption ClearOption { get; init; } = new("Clear", DialogOptionTag.Functional);

        /// <summary>
        /// 默认输入文本。
        /// </summary>
        public string DefaultText { get; init; } = string.Empty;

        /// <summary>
        /// 最大输入长度。
        /// </summary>
        public int MaximumLength { get; init; } = int.MaxValue;

        /// <summary>
        /// 是否多行输入。
        /// </summary>
        public bool IsMultiline { get; init; }

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption, ClearOption };
    }
}
