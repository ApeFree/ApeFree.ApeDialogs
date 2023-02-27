using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class MessageDialogSettings : DialogSettings<bool>
    {
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(CancelOptionText),
            };
        }
    }
}
