using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 单选对话框配置。
    /// </summary>
    /// <typeparam name="T">选项数据类型</typeparam>
    public class SelectionDialogSettings<T> : DialogSettings<T>
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
        /// 选项显示文本转换器。
        /// </summary>
        public Func<T, string> ItemDisplayConverter { get; set; } = item => item?.ToString() ?? string.Empty;

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }
}
