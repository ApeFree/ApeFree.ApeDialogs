using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 保存文件对话框配置。结果为单个保存路径（string）。
    /// </summary>
    public class SaveFileDialogSettings : DialogSettings<string>
    {
        /// <summary>
        /// 初始目录；为空时由实现方决定。
        /// </summary>
        public string? InitialPath { get; set; }

        /// <summary>
        /// 默认文件名。
        /// </summary>
        public string DefaultFileName { get; set; } = string.Empty;

        /// <summary>
        /// 搜索通配符（文件类型过滤）。
        /// </summary>
        public string SearchPattern { get; set; } = "*";

        /// <summary>
        /// 目标文件已存在时是否提示覆盖。
        /// </summary>
        public bool OverwritePrompt { get; set; } = true;

        /// <summary>
        /// 确认选项。
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new("Save", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; set; } = DialogOption.CreateCancelOption();

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }
}
