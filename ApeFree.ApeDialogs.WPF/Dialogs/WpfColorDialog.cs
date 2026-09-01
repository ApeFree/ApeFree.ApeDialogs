using System;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 颜色选择对话框（WPF 实现）。
    /// </summary>
    public class WpfColorDialog : WpfDialogBase<ApeColor>
    {
        private readonly ColorDialogSettings _settings;
        private readonly ApeColorPicker _picker;

        public WpfColorDialog(ColorDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 460;
            Host.Height = 520;

            _picker = new ApeColorPicker
            {
                SelectedColor = settings.DefaultColor,
                AllowAlpha = settings.AllowAlpha,
                PresetColors = settings.PresetColors,
            };

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(_picker);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_picker.SelectedColor);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(_picker.SelectedColor);
    }
}
