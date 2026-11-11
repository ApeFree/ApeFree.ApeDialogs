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
    /// 密码输入对话框（WPF 实现）。
    /// 注意：WPF 的 PasswordBox 不支持自定义掩码字符，<see cref="PasswordDialogSettings.PasswordChar"/> 在 WPF 下无效（掩码为平台默认 ●）。
    /// </summary>
    public class WpfPasswordDialog : WpfDialogBase<string>
    {
        private readonly PasswordDialogSettings _settings;
        private readonly ApePasswordBox _passwordBox;
        private readonly TextBlock _errorText;

        public WpfPasswordDialog(PasswordDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _passwordBox = new ApePasswordBox
            {
                Password = string.Empty,
                PlaceholderText = string.Empty,
            };

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
            panel.Children.Add(_passwordBox);
            panel.Children.Add(_errorText);
            Host.SetDialogContent(panel);

            AddDefaultOptions();

            Host.Loaded += (_, _) =>
            {
                _passwordBox.Focus();
            };
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_passwordBox.Password);
            }
            else if (ReferenceEquals(dialogOption, _settings.ClearOption))
            {
                button.Click += (_, _) => _passwordBox.Password = string.Empty;
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(_passwordBox.Password);

        /// <inheritdoc/>
        protected override void OnValidationFailed(ApeFree.ApeDialogs.Core.ValidationResult result)
        {
            base.OnValidationFailed(result);

            _errorText.Text = result.ErrorMessage ?? string.Empty;
            _errorText.Visibility = string.IsNullOrEmpty(result.ErrorMessage)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
