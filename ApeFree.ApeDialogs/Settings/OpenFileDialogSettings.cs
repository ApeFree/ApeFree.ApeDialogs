using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 打开文件对话框配置。
    /// </summary>
    public class OpenFileDialogSettings : DialogSettings<IReadOnlyList<string>>
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
        /// 初始目录；为空时由实现方决定（如上次目录或默认目录）。
        /// </summary>
        public string? InitialPath { get; init; }

        /// <summary>
        /// 搜索通配符。
        /// </summary>
        public string SearchPattern { get; init; } = "*";

        /// <summary>
        /// 是否支持多选。
        /// </summary>
        public bool MultiSelect { get; init; }

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }

    /// <summary>
    /// 打开文件夹对话框配置。
    /// </summary>
    public class OpenFolderDialogSettings : DialogSettings<IReadOnlyList<string>>
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
        /// 初始目录；为空时由实现方决定（如上次目录或默认目录）。
        /// </summary>
        public string? InitialPath { get; init; }

        /// <summary>
        /// 是否支持多选。
        /// </summary>
        public bool MultiSelect { get; init; }

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }
}
