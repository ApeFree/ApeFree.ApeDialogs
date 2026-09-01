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
    /// 数值输入对话框（WPF 实现，类似 NumericUpDown）。
    /// </summary>
    public class WpfNumberInputDialog : WpfDialogBase<decimal>
    {
        private readonly NumberInputDialogSettings _settings;
        private readonly ApeNumericUpDown _numeric;
        private readonly TextBlock _errorText;

        public WpfNumberInputDialog(NumberInputDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _numeric = new ApeNumericUpDown
            {
                Minimum = settings.Minimum,
                Maximum = settings.Maximum,
                Increment = settings.Increment,
                DecimalPlaces = settings.DecimalPlaces,
                Value = settings.DefaultValue,
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
            panel.Children.Add(_numeric);
            panel.Children.Add(_errorText);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => Submit();
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => Submit();

        /// <inheritdoc/>
        protected override void OnValidationFailed(ApeFree.ApeDialogs.Core.ValidationResult result)
        {
            base.OnValidationFailed(result);

            _errorText.Text = result.ErrorMessage ?? string.Empty;
            _errorText.Visibility = string.IsNullOrEmpty(result.ErrorMessage)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Submit()
        {
            _numeric.Commit();
            TrySubmit(_numeric.Value);
        }
    }
}
