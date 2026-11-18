using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 进度对话框（MAUI 实现，支持确定/不确定模式，跨线程更新与自动关闭）。
    /// </summary>
    public partial class MauiProgressDialog : MauiDialogBase<bool>, IProgressDialog
    {
        private readonly ProgressDialogSettings _settings;
        private readonly ProgressBar _progressBar;
        private readonly ActivityIndicator _activityIndicator;
        private readonly Label _messageLabel;

        public MauiProgressDialog(ProgressDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _settings = settings;

            var panel = new VerticalStackLayout { Spacing = 10 };

            _progressBar = new ProgressBar
            {
                Progress = Math.Max(0, Math.Min(1, settings.InitialProgress)),
                ProgressColor = MauiThemeManager.AccentColor,
                IsVisible = !settings.IsIndeterminate,
            };
            panel.Children.Add(_progressBar);

            _activityIndicator = new ActivityIndicator
            {
                IsRunning = settings.IsIndeterminate,
                IsVisible = settings.IsIndeterminate,
                Color = MauiThemeManager.AccentColor,
                HeightRequest = 40,
            };
            panel.Children.Add(_activityIndicator);

            _messageLabel = new Label
            {
                Text = settings.InitialMessage ?? string.Empty,
                TextColor = MauiThemeManager.TextSecondary,
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = !string.IsNullOrEmpty(settings.InitialMessage),
            };
            panel.Children.Add(_messageLabel);

            Host.SetDialogContent(panel);

            if (settings.Cancelable && settings.CancelOption != null)
            {
                var cancelOption = settings.CancelOption with
                {
                    Action = _ => Cancel()
                };
                AddOption(cancelOption);
            }
        }

        public bool IsIndeterminate => _settings.IsIndeterminate;

        public void UpdateProgress(double progress, string? message = null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var clamped = Math.Max(0, Math.Min(1, progress));

                if (!_settings.IsIndeterminate)
                {
                    _progressBar.Progress = clamped;
                }

                if (message != null)
                {
                    _messageLabel.Text = message;
                    _messageLabel.IsVisible = true;
                }

                // 进度完成 → 自动以确认结果关闭
                if (clamped >= 1.0)
                {
                    TrySubmit(true);
                }
            });
        }
    }
}
