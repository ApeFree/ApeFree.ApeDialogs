using System;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 轻提示对话框（MAUI 实现，Toast）。
    /// </summary>
    public partial class MauiToastDialog : MauiDialogBase<bool>
    {
        public MauiToastDialog(ToastDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            Host.Cancelable = false;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (_, _) => TrySubmit(true);
            Host.GestureRecognizers.Add(tapGesture);
        }
    }
}
