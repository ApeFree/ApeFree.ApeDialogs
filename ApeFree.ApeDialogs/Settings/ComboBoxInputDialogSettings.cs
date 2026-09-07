using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 可编辑下拉输入对话框配置（选择预设项或自由输入）。
    /// </summary>
    public class ComboBoxInputDialogSettings : DialogSettings<string>
    {
        /// <summary>
        /// 预设选项集合。
        /// </summary>
        public IReadOnlyList<string> Items { get; set; } = new string[0];

        /// <summary>
        /// 默认输入文本。
        /// </summary>
        public string DefaultText { get; set; } = string.Empty;

        /// <summary>
        /// 是否允许自由输入（为 false 时仅可从预设项中选择）。
        /// </summary>
        public bool IsEditable { get; set; } = true;

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
