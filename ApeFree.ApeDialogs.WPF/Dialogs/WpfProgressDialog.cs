using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 进度对话框（WPF 实现）。
    /// 调用方通过 <see cref="IProgressDialog.UpdateProgress"/> 推进进度；进度达到 1.0 时自动以确认结果关闭。
    /// </summary>
    public class WpfProgressDialog : WpfDialogBase<bool>, IProgressDialog
    {
        private readonly ProgressDialogSettings _settings;
        private readonly ApeProgressBar _progressBar;
        private readonly TextBlock _messageText;

        public WpfProgressDialog(ProgressDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _progressBar = new ApeProgressBar
            {
                IsIndeterminate = settings.IsIndeterminate,
                Value = Math.Max(0, Math.Min(100, settings.InitialProgress * 100)),
                Margin = new Thickness(0, 12, 0, 0),
            };

            _messageText = new TextBlock
            {
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 0),
                Text = settings.InitialMessage ?? string.Empty,
            };
            _messageText.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillSecondary");

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(_progressBar);
            panel.Children.Add(_messageText);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public bool IsIndeterminate => _settings.IsIndeterminate;

        /// <inheritdoc/>
        public void UpdateProgress(double progress, string? message = null)
        {
            if (!Host.Dispatcher.CheckAccess())
            {
                Host.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => UpdateProgress(progress, message)));
                return;
            }

            var clamped = Math.Max(0, Math.Min(1, progress));

            if (!_progressBar.IsIndeterminate)
            {
                _progressBar.Value = clamped * 100;
            }

            if (message is not null)
            {
                _messageText.Text = message;
            }

            // 进度完成 → 自动以确认结果关闭
            if (clamped >= 1.0)
            {
                TrySubmit(true);
            }
        }
    }
}
