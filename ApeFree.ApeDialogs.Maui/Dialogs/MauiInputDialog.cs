using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 文本输入对话框（MAUI 实现）。
    /// </summary>
    public partial class MauiInputDialog : MauiDialogBase<string>
    {
        private readonly InputView _inputView;
        private readonly Label _errorLabel;

        public MauiInputDialog(InputDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var panel = new VerticalStackLayout { Spacing = 6 };

            if (settings.IsMultiline)
            {
                var editor = new Editor
                {
                    Text = settings.DefaultText ?? string.Empty,
                    HeightRequest = 100,
                    MaxLength = settings.MaximumLength,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };
                _inputView = editor;
            }
            else
            {
                var entry = new Entry
                {
                    Text = settings.DefaultText ?? string.Empty,
                    MaxLength = settings.MaximumLength,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                    ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
                };
                entry.Completed += (_, _) => TrySubmit(Text);
                _inputView = entry;
            }

            panel.Children.Add(_inputView);

            _errorLabel = new Label
            {
                TextColor = MauiThemeManager.NegativeColor,
                FontSize = 12,
                IsVisible = false,
            };
            panel.Children.Add(_errorLabel);

            Host.SetDialogContent(panel);

            // 选项装配
            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(Text)
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
                    Action = _ => Text = string.Empty
                };
                AddOption(clearOption);
            }
        }

        /// <summary>
        /// 输入框文本。
        /// </summary>
        public string Text
        {
            get => _inputView.Text ?? string.Empty;
            set => _inputView.Text = value;
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
