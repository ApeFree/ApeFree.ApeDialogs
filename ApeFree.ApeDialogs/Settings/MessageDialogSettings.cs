using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class MessageDialogSettings : DialogSettings<bool>
    {
        public override IEnumerable<DialogOption> GetOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(CancelOptionText),
            };
        }
    }
}
