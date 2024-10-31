using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 打开文件Dialog设置
    /// </summary>
    public class OpenFileDialogSettings : DialogSettings<string[]>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项
        /// </summary>
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, enabled: true, DefaultCancelOptionHandler);

        /// <summary>
        /// 搜索通配符
        /// </summary>
        public string SearchPattern { get; set; } = "*";

        /// <summary>
        /// 是否支持多选
        /// </summary>
        public bool MultiSelect { get; set; }

        public OpenFileDialogSettings()
        {
            DialogSize = new System.Drawing.Size(600, 500);
        }

        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()
        {
            return new DialogOption[] { ConfirmOption, CancelOption };
        }
    }

    /// <summary>
    /// 打开文件Dialog设置
    /// </summary>
    public class OpenFolderDialogSettings : DialogSettings<string[]>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项
        /// </summary>
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, enabled: true, DefaultCancelOptionHandler);

        /// <summary>
        /// 是否支持多选
        /// </summary>
        public bool MultiSelect { get; set; }

        public OpenFolderDialogSettings()
        {
            DialogSize = new System.Drawing.Size(600, 500);
        }

        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()
        {
            return new DialogOption[] { ConfirmOption, CancelOption };
        }
    }
}
