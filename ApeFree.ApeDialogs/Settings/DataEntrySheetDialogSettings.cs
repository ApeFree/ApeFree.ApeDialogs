using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 多字段表单Dialog设置
    /// </summary>
    public class DataEntrySheetDialogSettings : DialogSettings<DataEntrySheet>
    {
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, enabled: true, DefaultCancelOptionHandler);
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()
        {
            return new DialogOption[] { ConfirmOption, CancelOption };
        }
    }
}
