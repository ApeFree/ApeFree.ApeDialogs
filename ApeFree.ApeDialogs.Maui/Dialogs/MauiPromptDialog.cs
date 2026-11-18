using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 提示对话框（MAUI 实现，二选一确认）。
    /// </summary>
    public partial class MauiPromptDialog : MauiDialogBase<bool>
    {
        public MauiPromptDialog(PromptDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var positiveOption = settings.PositiveOption with
            {
                Action = _ => TrySubmit(true)
            };

            var negativeOption = settings.NegativeOption with
            {
                Action = _ => TrySubmit(false)
            };

            AddOption(positiveOption);
            AddOption(negativeOption);
        }
    }
}
