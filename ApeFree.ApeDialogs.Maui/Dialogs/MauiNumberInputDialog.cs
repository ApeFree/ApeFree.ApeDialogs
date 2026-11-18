using System;
using ApeFree.ApeDialogs.Maui.Controls;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 数值输入对话框（MAUI 实现）。
    /// </summary>
    public partial class MauiNumberInputDialog : MauiDialogBase<decimal>
    {
        private readonly ApeNumericUpDown _numericBox;
        private readonly Label _errorLabel;

        public MauiNumberInputDialog(NumberInputDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var panel = new VerticalStackLayout { Spacing = 8 };

            _numericBox = new ApeNumericUpDown
            {
                Minimum = settings.Minimum,
                Maximum = settings.Maximum,
                Increment = settings.Increment,
                DecimalPlaces = settings.DecimalPlaces,
                Value = settings.DefaultValue,
            };
            panel.Children.Add(_numericBox);

            _errorLabel = new Label
            {
                TextColor = MauiThemeManager.NegativeColor,
                FontSize = 12,
                IsVisible = false,
            };
            panel.Children.Add(_errorLabel);

            Host.SetDialogContent(panel);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(_numericBox.Value)
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

        protected override void OnValidationFailed(Core.ValidationResult result)
        {
            base.OnValidationFailed(result);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                _errorLabel.Text = result.ErrorMessage;
                _errorLabel.IsVisible = true;
            }
        }
    }
}
