using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class MessageDialogSettings : DialogSettings<bool>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionType.Neutral);


        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()=>new DialogOption[] { ConfirmOption };
    }
}
