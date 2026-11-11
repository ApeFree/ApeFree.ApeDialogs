using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Hosting;
using ApeFree.ApeDialogs.WPF.Interop;
using ApeFree.ApeDialogs.WPF.Theme;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// WPF Dialog 实现基类：统一宿主、选项装配、快捷键、校验反馈、模态展示。
    /// </summary>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public abstract class WpfDialogBase<TResult> : BaseDialog<DialogHostWindow, ApeButton, IWin32Owner, TResult>
    {
        private static readonly System.Collections.Generic.List<DialogHostWindow> ActiveDialogs = new();
        private static readonly object ActiveLock = new();

        private readonly IDialogTheme _theme;
        private readonly TextBlock _contentTextBlock;
        private int _ownerDisabledState;
        private IWin32Owner? _effectiveOwner;

        /// <summary>
        /// 构造 WPF Dialog。
        /// </summary>
        protected WpfDialogBase(DialogSettings<TResult> settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));

            Host = new DialogHostWindow();
            View = Host;
            Host.Title = settings.Title;
            Host.ShowCloseButton = settings.Cancelable;
            Host.Cancelable = settings.Cancelable;
            Host.CloseRequested += (_, _) => Cancel();
            Host.ConfirmRequested += (_, _) => OnConfirmRequested();
            Host.Loaded += (_, _) => RaiseShown();
            Host.Closed += (_, _) => Cancel();

            if (settings.ReminderColor.HasValue)
            {
                Host.SetReminderColor(settings.ReminderColor.Value);
            }

            if (settings.DialogSize is ApeSize size && size.Width > 0 && size.Height > 0)
            {
                Host.Width = size.Width;
                Host.Height = size.Height;
            }

            double contentFontSize = 14;
            if (settings.Font is DialogFont font)
            {
                if (!string.IsNullOrWhiteSpace(font.Family))
                {
                    Host.FontFamily = new FontFamily(font.Family);
                }
                if (font.Size > 0)
                {
                    Host.FontSize = font.Size;
                    contentFontSize = font.Size;
                }
                if (font.Weight == ApeFree.ApeDialogs.Theme.FontWeight.Bold)
                {
                    Host.FontWeight = FontWeights.Bold;
                }
                if (font.Italic)
                {
                    Host.FontStyle = FontStyles.Italic;
                }
            }

            _contentTextBlock = new TextBlock
            {
                Text = settings.Content ?? string.Empty,
                TextWrapping = TextWrapping.Wrap,
                FontSize = contentFontSize,
                Margin = new Thickness(0, 0, 0, 12),
            };
            _contentTextBlock.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillPrimary");
        }

        /// <summary>
        /// 宿主窗口。
        /// </summary>
        protected DialogHostWindow Host { get; }

        /// <summary>
        /// 内容提示文本块。
        /// </summary>
        protected TextBlock ContentTextBlock => _contentTextBlock;

        /// <inheritdoc/>
        public override string Title
        {
            get
            {
                if (Host.Dispatcher.CheckAccess()) return Host.Title;
                return Host.Dispatcher.Invoke(() => Host.Title);
            }
            set
            {
                if (Host.Dispatcher.CheckAccess()) Host.Title = value;
                else Host.Dispatcher.Invoke(() => Host.Title = value);
            }
        }

        /// <inheritdoc/>
        public override string Content
        {
            get
            {
                if (_contentTextBlock.Dispatcher.CheckAccess()) return _contentTextBlock.Text;
                return _contentTextBlock.Dispatcher.Invoke(() => _contentTextBlock.Text);
            }
            set
            {
                if (_contentTextBlock.Dispatcher.CheckAccess()) _contentTextBlock.Text = value;
                else _contentTextBlock.Dispatcher.Invoke(() => _contentTextBlock.Text = value);
            }
        }

        /// <summary>
        /// 回车确认动作（默认无操作，由具体 Dialog 覆写）。
        /// </summary>
        protected virtual void OnConfirmRequested()
        {
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            if (dialogOption is null)
            {
                throw new ArgumentNullException(nameof(dialogOption));
            }

            var button = new ApeButton
            {
                Content = dialogOption.Text,
                IsEnabled = dialogOption.IsEnabled,
            };
            ApplyOptionTagStyle(button, dialogOption);

            button.Click += (_, _) =>
            {
                dialogOption.Action?.Invoke(this);
                dialogOption.OnSelected?.Invoke(this, new OptionSelectedEventArgs(this, dialogOption));
            };

            Host.AddOptionButton(button);
            return button;
        }

        /// <summary>
        /// 依据选项标签应用 Win11 语义样式（Positive → 强调色按钮；Negative/Special → 语义色文字）。
        /// </summary>
        protected virtual void ApplyOptionTagStyle(ApeButton button, DialogOption dialogOption)
        {
            switch (dialogOption.Tag)
            {
                case DialogOptionTag.Positive:
                    button.IsAccent = true;
                    break;

                case DialogOptionTag.Negative:
                    button.SetResourceReference(Control.ForegroundProperty, "ApeNegativeBrush");
                    break;

                case DialogOptionTag.Special:
                    button.SetResourceReference(Control.ForegroundProperty, "ApeSpecialBrush");
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ClearOptions() => Host.ClearOptionButtons();

        /// <inheritdoc/>
        protected override void ShowHandler()
        {
            if (Host.Dispatcher.CheckAccess())
            {
                ShowHandlerCore();
            }
            else
            {
                Host.Dispatcher.Invoke(ShowHandlerCore);
            }
        }

        private void ShowHandlerCore()
        {
            if (Settings.DialogSize is ApeSize size && size.Width > 0 && size.Height > 0)
            {
                Host.Width = size.Width;
                Host.Height = size.Height;
            }

            _effectiveOwner = Context;
            if ((_effectiveOwner is null || _effectiveOwner.Handle == IntPtr.Zero) &&
                Application.Current?.MainWindow is Window mainWindow &&
                mainWindow != Host &&
                mainWindow.IsVisible)
            {
                var handle = new WindowInteropHelper(mainWindow).Handle;
                if (handle != IntPtr.Zero)
                {
                    _effectiveOwner = new WindowOwner(mainWindow);
                }
            }

            if (_effectiveOwner is not null && _effectiveOwner.Handle != IntPtr.Zero)
            {
                new WindowInteropHelper(Host).Owner = _effectiveOwner.Handle;

                if (System.Threading.Interlocked.CompareExchange(ref _ownerDisabledState, 1, 0) == 0)
                {
                    Win32WindowHelper.PushDisableWindow(_effectiveOwner.Handle);
                }

                Host.SourceInitialized += (_, _) =>
                {
                    Win32WindowHelper.CenterOverOwner(Host, _effectiveOwner.Handle);
                };

                Host.Closed += (s, e) =>
                {
                    RestoreOwnerState();
                };
            }
            else
            {
                Host.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Host.ShowInTaskbar = true;
                Host.Topmost = true;
            }

            lock (ActiveLock)
            {
                int cascadeCount = ActiveDialogs.Count;
                if (cascadeCount > 0)
                {
                    int offset = (cascadeCount % 8) * 24;
                    Host.SourceInitialized += (_, _) =>
                    {
                        Host.Left += offset;
                        Host.Top += offset;
                    };
                }
                ActiveDialogs.Add(Host);
            }

            Host.Closed += (_, _) =>
            {
                lock (ActiveLock)
                {
                    ActiveDialogs.Remove(Host);
                }
            };

            try
            {
                Host.Show();
            }
            catch
            {
                lock (ActiveLock)
                {
                    ActiveDialogs.Remove(Host);
                }
                RestoreOwnerState();
                throw;
            }
        }

        private void RestoreOwnerState()
        {
            var owner = _effectiveOwner ?? Context;
            if (owner is not null && owner.Handle != IntPtr.Zero)
            {
                if (System.Threading.Interlocked.CompareExchange(ref _ownerDisabledState, 0, 1) == 1)
                {
                    Win32WindowHelper.PopEnableWindow(owner.Handle);
                }
            }
        }

        /// <inheritdoc/>
        protected override void DismissHandler()
        {
            RestoreOwnerState();

            if (Host.Dispatcher.CheckAccess())
            {
                Host.PlayCloseAnimation();
            }
            else
            {
                Host.Dispatcher.BeginInvoke(new Action(Host.PlayCloseAnimation));
            }
        }

        /// <inheritdoc/>
        protected override void OnValidationFailed(ApeFree.ApeDialogs.Core.ValidationResult result)
        {
            Host.PlayShakeAnimation();
        }
    }
}
