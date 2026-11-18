using System;
using ApeFree.ApeDialogs.Maui.Controls;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 颜色选择对话框（MAUI 实现，基于纯自绘调色板）。
    /// </summary>
    public partial class MauiColorDialog : MauiDialogBase<ApeColor>
    {
        private readonly ApeColorPicker _picker;

        public MauiColorDialog(ColorDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _picker = new ApeColorPicker
            {
                AllowAlpha = settings.AllowAlpha,
                SelectedColor = settings.DefaultColor,
            };
            _picker.SetPresetColors(settings.PresetColors);

            Host.SetDialogContent(_picker);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(_picker.SelectedColor)
            };
            AddOption(confirmOption);

            if (settings.CancelOption != null)
            {
                var cancelOption = settings.CancelOption with
                {
                    Action = _ => Cancel()
                };
                AddOption(cancelOption);
            }
        }
    }
}
