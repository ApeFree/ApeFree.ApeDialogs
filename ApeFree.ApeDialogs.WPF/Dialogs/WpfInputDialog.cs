using System;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 输入对话框（WPF 实现）。
    /// </summary>
    public class WpfInputDialog : WpfDialogBase<string>
    {
        private readonly InputDialogSettings _settings;
        private readonly ApeTextBox _textBox;
        private readonly TextBlock _errorText;

        public WpfInputDialog(InputDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _textBox = new ApeTextBox
            {
                Text = settings.DefaultText,
                MaxLength = settings.MaximumLength,
            };

            if (settings.IsMultiline)
            {
                _textBox.AcceptsReturn = true;
                _textBox.TextWrapping = TextWrapping.Wrap;
                _textBox.VerticalContentAlignment = VerticalAlignment.Top;
                _textBox.Height = 120;
                _textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            _errorText = new TextBlock
            {
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0),
                Visibility = Visibility.Collapsed,
            };
            _errorText.SetResourceReference(TextBlock.ForegroundProperty, "ApeNegativeBrush");

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(_textBox);
            panel.Children.Add(_errorText);
            Host.SetDialogContent(panel);

            AddDefaultOptions();

            Host.Loaded += (_, _) =>
            {
                _textBox.Focus();
                _textBox.SelectAll();
            };
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_textBox.Text);
            }
            else if (ReferenceEquals(dialogOption, _settings.ClearOption))
            {
                button.Click += (_, _) => _textBox.Clear();
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested()
        {
            // 多行输入时回车用于换行，不触发确认
            if (!_settings.IsMultiline)
            {
                TrySubmit(_textBox.Text);
            }
        }

        /// <inheritdoc/>
        protected override void OnValidationFailed(ApeFree.ApeDialogs.Core.ValidationResult result)
        {
            base.OnValidationFailed(result);

            _errorText.Text = result.ErrorMessage ?? string.Empty;
            _errorText.Visibility = string.IsNullOrEmpty(result.ErrorMessage)
                ? Visibility.Collapsed
                : Visibility.Visible;

            _textBox.TextChanged += (_, _) => _errorText.Visibility = Visibility.Collapsed;
        }
    }
}
