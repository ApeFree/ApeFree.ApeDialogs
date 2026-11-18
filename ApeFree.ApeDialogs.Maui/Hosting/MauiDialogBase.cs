using System;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Maui.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Hosting
{
    /// <summary>
    /// MAUI 对话框基类，统一处理页面树挂载、选项装配、进出场动画与校验反馈。
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    public abstract class MauiDialogBase<TResult> : BaseDialog<MauiDialogHost, ApeButton, IMauiDialogContext, TResult>
    {
        private readonly IDialogTheme _theme;
        private Page? _attachedPage;
        private Grid? _injectedWrapperGrid;
        private View? _originalPageContent;

        protected MauiDialogBase(DialogSettings<TResult> settings, IMauiDialogContext? context, IDialogTheme theme)
            : base(settings, context)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));

            Host = new MauiDialogHost();
            View = Host;

            Host.Title = settings.Title ?? string.Empty;
            Host.ContentText = settings.Content ?? string.Empty;
            Host.Cancelable = settings.Cancelable;
            Host.CloseRequested += (_, _) => Cancel();

            if (settings.ReminderColor.HasValue)
            {
                Host.SetReminderColor(settings.ReminderColor.Value);
            }
        }

        /// <summary>
        /// 宿主控件。
        /// </summary>
        protected MauiDialogHost Host { get; }

        /// <inheritdoc/>
        public override string Title
        {
            get => Host.Title;
            set => Host.Title = value;
        }

        /// <inheritdoc/>
        public override string Content
        {
            get => Host.ContentText;
            set => Host.ContentText = value;
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
                Text = dialogOption.Text,
                Tag = dialogOption.Tag,
                IsEnabled = dialogOption.IsEnabled,
            };

            button.Clicked += (_, _) =>
            {
                dialogOption.Action?.Invoke(this);
                dialogOption.OnSelected?.Invoke(this, new OptionSelectedEventArgs(this, dialogOption));
            };

            Host.AddOptionButton(button);
            return button;
        }

        /// <inheritdoc/>
        public override void ClearOptions() => Host.ClearOptionButtons();

        /// <inheritdoc/>
        protected override void ShowHandler()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                _attachedPage = CurrentPageResolver.ResolveCurrentPage(Context);
                if (_attachedPage is not ContentPage contentPage)
                {
                    // 无法找到目标 ContentPage 时直接取消
                    Cancel();
                    return;
                }

                if (contentPage.Content is Grid existingGrid)
                {
                    existingGrid.Children.Add(Host);
                    Grid.SetRowSpan(Host, Math.Max(1, existingGrid.RowDefinitions.Count));
                    Grid.SetColumnSpan(Host, Math.Max(1, existingGrid.ColumnDefinitions.Count));
                }
                else
                {
                    _originalPageContent = contentPage.Content;
                    _injectedWrapperGrid = new Grid();
                    contentPage.Content = null;

                    if (_originalPageContent != null)
                    {
                        _injectedWrapperGrid.Children.Add(_originalPageContent);
                    }
                    _injectedWrapperGrid.Children.Add(Host);
                    contentPage.Content = _injectedWrapperGrid;
                }

                await Host.PlayOpenAnimationAsync();
                RaiseShown();
            });
        }

        /// <inheritdoc/>
        protected override void DismissHandler()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await Host.PlayCloseAnimationAsync();
                }
                catch
                {
                    // 忽略退场动画异常
                }

                if (_attachedPage is ContentPage contentPage)
                {
                    if (_injectedWrapperGrid != null)
                    {
                        contentPage.Content = null;
                        _injectedWrapperGrid.Children.Clear();
                        contentPage.Content = _originalPageContent;
                        _injectedWrapperGrid = null;
                        _originalPageContent = null;
                    }
                    else if (contentPage.Content is Grid existingGrid)
                    {
                        existingGrid.Children.Remove(Host);
                    }
                }
            });
        }

        /// <inheritdoc/>
        protected override void OnValidationFailed(ValidationResult result)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Host.PlayShakeAnimationAsync();
            });
        }
    }
}
