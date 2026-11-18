using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 密码输入对话框（MAUI 实现，带明密文切换）。
    /// </summary>
    public partial class MauiPasswordDialog : MauiDialogBase<string>
    {
        private readonly Entry _passwordEntry;
        private readonly Label _errorLabel;

        public MauiPasswordDialog(PasswordDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var panel = new VerticalStackLayout { Spacing = 6 };

            var inputGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                }
            };

            _passwordEntry = new Entry
            {
                Text = settings.DefaultText ?? string.Empty,
                IsPassword = true,
                MaxLength = settings.MaximumLength,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
            };
            _passwordEntry.Completed += (_, _) => TrySubmit(Password);
            inputGrid.Children.Add(_passwordEntry);
            Grid.SetColumn(_passwordEntry, 0);

            var toggleButton = new Button
            {
                Text = "👁",
                FontSize = 14,
                WidthRequest = 40,
                HeightRequest = 40,
                Padding = 0,
                BackgroundColor = Colors.Transparent,
                TextColor = MauiThemeManager.TextSecondary,
            };
            toggleButton.Clicked += (_, _) =>
            {
                _passwordEntry.IsPassword = !_passwordEntry.IsPassword;
                toggleButton.TextColor = _passwordEntry.IsPassword ? MauiThemeManager.TextSecondary : MauiThemeManager.AccentColor;
            };
            inputGrid.Children.Add(toggleButton);
            Grid.SetColumn(toggleButton, 1);

            panel.Children.Add(inputGrid);

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
                Action = _ => TrySubmit(Password)
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

            if (settings.ClearOption != null)
            {
                var clearOption = settings.ClearOption with
                {
                    Action = _ => Password = string.Empty
                };
                AddOption(clearOption);
            }
        }

        /// <summary>
        /// 输入的密码。
        /// </summary>
        public string Password
        {
            get => _passwordEntry.Text ?? string.Empty;
            set => _passwordEntry.Text = value;
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
