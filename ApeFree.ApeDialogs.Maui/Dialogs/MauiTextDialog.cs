using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 长文本查看对话框（MAUI 实现，支持滚动与文本复制）。
    /// </summary>
    public partial class MauiTextDialog : MauiDialogBase<bool>
    {
        public MauiTextDialog(TextDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var editor = new Editor
            {
                Text = settings.Content ?? string.Empty,
                IsReadOnly = true,
                HeightRequest = 260,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
                FontSize = 13,
            };

            Host.SetDialogContent(editor);

            var closeOption = settings.CloseOption with
            {
                Action = _ => TrySubmit(true)
            };
            AddOption(closeOption);
        }
    }
}
