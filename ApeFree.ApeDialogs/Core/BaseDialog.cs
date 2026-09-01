using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Settings;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// Dialog 实现辅助基类。实现方需实现视图相关的抽象成员。
    /// </summary>
    /// <typeparam name="TView">视图类型</typeparam>
    /// <typeparam name="TOption">选项控件类型</typeparam>
    /// <typeparam name="TContext">上下文类型</typeparam>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public abstract class BaseDialog<TView, TOption, TContext, TResult> : IDialogController<TResult>, IDisposable
    {
#if NET45 || NET451 || NET452
        private readonly TaskCompletionSource<DialogResult<TResult>> _completionSource = new();
#else
        private readonly TaskCompletionSource<DialogResult<TResult>> _completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
#endif
        private readonly CancellationTokenSource _autoDismissCts = new();
        private bool _disposed;

        /// <summary>
        /// 构造 Dialog。
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <param name="view">视图（可由实现方延迟绑定）</param>
        protected BaseDialog(DialogSettings<TResult> settings, TContext? context = default, TView? view = default)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Context = context;
            View = view;
        }

        /// <summary>
        /// 装配设置中的默认选项（过滤文本为空的选项）。
        /// 注意：不得在构造函数中调用虚方法，实现方应在构造函数末尾（实例字段全部就绪后）调用此方法。
        /// </summary>
        protected void AddDefaultOptions()
        {
            foreach (var option in Settings.DefaultOptions.Where(option => !string.IsNullOrWhiteSpace(option.Text)))
            {
                AddOption(option);
            }
        }

        /// <summary>
        /// 配置参数。
        /// </summary>
        protected DialogSettings<TResult> Settings { get; }

        /// <summary>
        /// 上下文。
        /// </summary>
        public TContext? Context { get; protected set; }

        /// <summary>
        /// 视图。
        /// </summary>
        public TView? View { get; protected set; }

        /// <summary>
        /// 标题。
        /// </summary>
        public abstract string Title { get; set; }

        /// <summary>
        /// 内容。
        /// </summary>
        public abstract string Content { get; set; }

        /// <summary>
        /// 是否正在显示。
        /// </summary>
        public bool IsShown { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<DialogEventArgs>? Showing;

        /// <inheritdoc/>
        public event EventHandler<DialogEventArgs>? Shown;

        /// <inheritdoc/>
        public event EventHandler<DialogEventArgs>? Dismissing;

        /// <inheritdoc/>
        public event EventHandler<DialogEventArgs>? Dismissed;

        /// <inheritdoc/>
        public async Task<DialogResult<TResult>> ShowAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return DialogResult<TResult>.Canceled;
            }

            if (IsShown)
            {
                throw new InvalidOperationException("The dialog has already been shown.");
            }

            using (cancellationToken.Register(() => Cancel()))
            {
                RaiseShowing();
                ShowHandler();

                if (Settings.Duration is TimeSpan duration && duration > TimeSpan.Zero)
                {
                    _ = AutoDismissAsync(duration);
                }

                return await _completionSource.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 校验指定值是否符合设置中的校验规则，并尝试提交。
        /// </summary>
        /// <param name="value">待提交的值</param>
        /// <returns>校验通过并提交成功返回 true；否则调用 <see cref="OnValidationFailed"/> 并返回 false</returns>
        protected bool TrySubmit(TResult value)
        {
            var result = Settings.Validator?.Invoke(value) ?? ValidationResult.Success;

            if (result.IsSuccess)
            {
                Dismiss(DialogResult<TResult>.Confirmed(value));
                return true;
            }

            OnValidationFailed(result);
            return false;
        }

        /// <summary>
        /// 校验失败时的回调，实现方在此展示错误提示（如抖动、Toast）。
        /// </summary>
        protected virtual void OnValidationFailed(ValidationResult result)
        {
        }

        /// <inheritdoc/>
        public void Dismiss(DialogResult<TResult> result)
        {
            if (_completionSource.Task.IsCompleted)
            {
                return;
            }

            _completionSource.TrySetResult(result);
            _autoDismissCts.Cancel();

            RaiseDismissing();
            DismissHandler();
            IsShown = false;
            RaiseDismissed();
        }

        /// <inheritdoc/>
        public void Cancel() => Dismiss(DialogResult<TResult>.Canceled);

        /// <summary>
        /// 添加选项。
        /// </summary>
        /// <param name="dialogOption">选项信息</param>
        /// <returns>实现方对应的选项控件</returns>
        public abstract TOption AddOption(DialogOption dialogOption);

        /// <summary>
        /// 清空选项。
        /// </summary>
        public abstract void ClearOptions();

        /// <summary>
        /// 显示视图的实现。
        /// </summary>
        protected abstract void ShowHandler();

        /// <summary>
        /// 销毁视图的实现。
        /// </summary>
        protected abstract void DismissHandler();

        /// <summary>
        /// 触发 <see cref="Showing"/> 事件。由实现方在视图显示后调用。
        /// </summary>
        protected void RaiseShown()
        {
            IsShown = true;
            Shown?.Invoke(this, new DialogEventArgs());
            Settings.OnShown?.Invoke(this);
        }

        private void RaiseShowing()
        {
            Showing?.Invoke(this, new DialogEventArgs());
        }

        private void RaiseDismissing()
        {
            Dismissing?.Invoke(this, new DialogEventArgs());
        }

        private void RaiseDismissed()
        {
            Dismissed?.Invoke(this, new DialogEventArgs());
            Settings.OnDismissed?.Invoke(this);
        }

        private async Task AutoDismissAsync(TimeSpan duration)
        {
            try
            {
                await Task.Delay(duration, _autoDismissCts.Token).ConfigureAwait(false);
                Cancel();
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _autoDismissCts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
