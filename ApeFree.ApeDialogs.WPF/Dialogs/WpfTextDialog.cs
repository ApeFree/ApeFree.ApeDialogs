using System;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 长文本查看对话框（WPF 实现）。
    /// </summary>
    public class WpfTextDialog : WpfDialogBase<bool>
    {
        private readonly TextDialogSettings _settings;

        public WpfTextDialog(TextDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 640;
            Host.Height = 420;

            var body = new TextBox
            {
                Text = settings.Text,
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = settings.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = settings.WordWrap ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto,
                VerticalContentAlignment = VerticalAlignment.Top,
                Padding = new Thickness(12),
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
            };

            if (!settings.IsSelectable)
            {
                body.IsHitTestVisible = false;
            }

            body.SetResourceReference(TextBox.ForegroundProperty, "ApeTextFillPrimary");
            body.SetResourceReference(TextBox.FontFamilyProperty, "ApeDefaultFontFamily");

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(ContentTextBlock, 0);
            Grid.SetRow(body, 1);
            grid.Children.Add(ContentTextBlock);
            grid.Children.Add(body);
            Host.SetDialogContent(grid);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.CloseOption))
            {
                button.Click += (_, _) => TrySubmit(true);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(true);
    }
}
