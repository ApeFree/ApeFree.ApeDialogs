using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 打开文件Dialog设置
    /// </summary>
    public class OpenFileDialogSettings : DialogSettings<string[]>
    {
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, enabled: true, DefaultCancelOptionHandler);
        public string SearchPattern { get; set; } = "*";
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
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, enabled: true, DefaultCancelOptionHandler);
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
