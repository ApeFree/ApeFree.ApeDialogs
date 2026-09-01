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
        private readonly IDialogTheme _theme;
        private readonly TextBlock _contentTextBlock;

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
            Host.CloseRequested += (_, _) => Cancel();
            Host.ConfirmRequested += (_, _) => OnConfirmRequested();
            Host.Loaded += (_, _) => RaiseShown();
            Host.Closed += (_, _) => Cancel();

            _contentTextBlock = new TextBlock
            {
                Text = settings.Content ?? string.Empty,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
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
            get => Host.Title;
            set => Host.Title = value;
        }

        /// <inheritdoc/>
        public override string Content
        {
            get => _contentTextBlock.Text;
            set => _contentTextBlock.Text = value;
        }

        /// <summary>
        /// 回车确认动作（默认无操作，由具体 Dialog 覆写）。
        /// </summary>
        protected virtual void OnConfirmRequested()
        {
        }

        /// <summary>
        /// 清空动作（默认无操作，由输入类 Dialog 覆写）。
        /// </summary>
        protected virtual void OnClearRequested()
        {
        }

        /// <summary>
        /// 全选动作（默认无操作，由多选类 Dialog 覆写）。
        /// </summary>
        protected virtual void OnSelectAllRequested()
        {
        }

        /// <summary>
        /// 反选动作（默认无操作，由多选类 Dialog 覆写）。
        /// </summary>
        protected virtual void OnReverseSelectionRequested()
        {
        }

        /// <summary>
        /// 设为当前时间动作（默认无操作，由日期时间 Dialog 覆写）。
        /// </summary>
        protected virtual void OnSetCurrentTimeRequested()
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
                switch (dialogOption.Behavior)
                {
                    case DialogOptionBehavior.Cancel:
                        Cancel();
                        break;

                    case DialogOptionBehavior.Clear:
                        OnClearRequested();
                        break;

                    case DialogOptionBehavior.SelectAll:
                        OnSelectAllRequested();
                        break;

                    case DialogOptionBehavior.ReverseSelection:
                        OnReverseSelectionRequested();
                        break;

                    case DialogOptionBehavior.SetCurrentTime:
                        OnSetCurrentTimeRequested();
                        break;

                    default:
                        dialogOption.OnSelected?.Invoke(this, new OptionSelectedEventArgs(this, dialogOption));
                        break;
                }
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
            var owner = Context;
            if (owner is not null && owner.Handle != IntPtr.Zero)
            {
                new WindowInteropHelper(Host).Owner = owner.Handle;
                Win32WindowHelper.CenterOverOwner(Host, owner.Handle);
                Win32WindowHelper.EnableWindow(owner.Handle, false);
                Host.Closed += (s, e) =>
                {
                    Win32WindowHelper.EnableWindow(owner.Handle, true);
                    Win32WindowHelper.SetActiveWindow(owner.Handle);
                };
            }
            else
            {
                Host.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            Host.Show();
        }

        /// <inheritdoc/>
        protected override void DismissHandler()
        {
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
