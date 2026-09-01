using System;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 轻提示（Toast）对话框（WPF 实现）。
    /// 非模态展示：显示于工作区右下角、不抢焦点；按 <see cref="ApeFree.ApeDialogs.Settings.DialogSettings.Duration"/> 自动消失，
    /// 可点击时点击立即以确认结果关闭。
    /// </summary>
    public class WpfToastDialog : WpfDialogBase<bool>
    {
        private readonly ToastDialogSettings _settings;

        public WpfToastDialog(ToastDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 320;
            Host.Height = 84;
            Host.ShowCloseButton = false;
            Host.ShowTitleBar = false;
            Host.ShowActivated = false;
            Host.Topmost = true;

            var text = new TextBlock
            {
                Text = settings.Content ?? string.Empty,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                FontSize = 14,
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
            };
            text.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillPrimary");

            Host.SetDialogContent(text);

            if (settings.IsClickable)
            {
                Host.ContentClicked += OnContentClicked;
            }

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        protected override void ShowHandler()
        {
            // 非模态：定位到工作区右下角后直接显示，不阻塞调用线程
            var workArea = SystemParameters.WorkArea;
            Host.WindowStartupLocation = WindowStartupLocation.Manual;
            Host.Left = workArea.Right - Host.Width - 16;
            Host.Top = workArea.Bottom - Host.Height - 16;
            Host.Show();
        }

        private void OnContentClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TrySubmit(true);
        }
    }
}
