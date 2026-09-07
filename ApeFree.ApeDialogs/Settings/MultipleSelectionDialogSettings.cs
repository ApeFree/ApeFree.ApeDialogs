using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 多选对话框配置。
    /// </summary>
    /// <typeparam name="T">选项数据类型</typeparam>
    public class MultipleSelectionDialogSettings<T> : DialogSettings<IReadOnlyList<T>>
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
        /// 全选选项。
        /// </summary>
        public DialogOption SelectAllOption { get; set; } = new("All", DialogOptionTag.Functional);

        /// <summary>
        /// 反选选项。
        /// </summary>
        public DialogOption ReverseSelectedOption { get; set; } = new("Reverse", DialogOptionTag.Functional);

        /// <summary>
        /// 选项显示文本转换器。
        /// </summary>
        public Func<T, string> ItemDisplayConverter { get; set; } = item => item?.ToString() ?? string.Empty;

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption, SelectAllOption, ReverseSelectedOption };
    }
}
