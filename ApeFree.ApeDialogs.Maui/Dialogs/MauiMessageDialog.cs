using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 消息对话框（MAUI 实现）。
    /// </summary>
    public partial class MauiMessageDialog : MauiDialogBase<bool>
    {
        public MauiMessageDialog(MessageDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(true)
            };

            AddOption(confirmOption);
        }
    }
}
