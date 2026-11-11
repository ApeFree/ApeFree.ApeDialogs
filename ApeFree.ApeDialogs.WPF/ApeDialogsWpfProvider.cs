using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Dialogs;
using ApeFree.ApeDialogs.WPF.Interop;
using ApeFree.ApeDialogs.WPF.Theme;

namespace ApeFree.ApeDialogs.WPF
{
    /// <summary>
    /// ApeDialogs 的 WPF 实现提供器（Win11 Fluent 设计风格）。
    /// 上下文类型为 <see cref="IWin32Owner"/>：WPF 应用传 <see cref="WindowOwner"/>（或使用便捷扩展），
    /// WinForms 应用传桥包的 FormOwner。
    /// </summary>
    public class ApeDialogsWpfProvider : DialogProvider<IWin32Owner>
    {
        private readonly IDialogTheme _theme;

        /// <summary>
        /// 构造提供器（使用 <see cref="Win11Theme"/>）。
        /// </summary>
        public ApeDialogsWpfProvider()
            : this(Win11Theme.Instance)
        {
        }

        /// <summary>
        /// 构造提供器。
        /// </summary>
        /// <param name="theme">选项标签配色主题</param>
        public ApeDialogsWpfProvider(IDialogTheme theme)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        private static readonly object StaLock = new();
        private static System.Windows.Threading.Dispatcher? _backgroundStaDispatcher;

        /// <summary>
        /// 确保获得可用的 STA Dispatcher 实例。
        /// 1. 若宿主已存在 Application.Current（WPF 宿主），直接使用其 Dispatcher；
        /// 2. 若当前为 STA 线程（如 WinForms UI 线程），在当前线程预初始化 ThemeManager；
        /// 3. 若环境无 Application 且当前为非 STA 工作线程，启动常驻后台 STA 线程驱动 Dispatcher。
        /// </summary>
        public static System.Windows.Threading.Dispatcher EnsureDispatcher()
        {
            if (System.Windows.Application.Current?.Dispatcher is { } currentDispatcher)
            {
                return currentDispatcher;
            }

            lock (StaLock)
            {
                if (System.Windows.Application.Current?.Dispatcher is { } appDispatcher)
                {
                    return appDispatcher;
                }

                if (_backgroundStaDispatcher is not null && !_backgroundStaDispatcher.HasShutdownStarted)
                {
                    return _backgroundStaDispatcher;
                }

                if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                {
                    ThemeManager.EnsureLoaded();
                    if (System.Windows.Application.Current?.Dispatcher is { } staDispatcher)
                    {
                        return staDispatcher;
                    }
                }

                var readyEvent = new ManualResetEventSlim(false);
                var thread = new Thread(() =>
                {
                    try
                    {
                        if (System.Windows.Application.Current is null)
                        {
                            _ = new System.Windows.Application { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
                        }
                        ThemeManager.EnsureLoaded();
                    }
                    catch
                    {
                        // 忽略已初始化冲突
                    }
                    finally
                    {
                        _backgroundStaDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                        readyEvent.Set();
                    }
                    System.Windows.Threading.Dispatcher.Run();
                })
                {
                    IsBackground = true,
                    Name = "ApeDialogs_STA_Host"
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                readyEvent.Wait(5000);
                return _backgroundStaDispatcher ?? System.Windows.Threading.Dispatcher.CurrentDispatcher;
            }
        }

        private static TDialog CreateOnUIThread<TDialog>(Func<TDialog> factory)
        {
            var dispatcher = EnsureDispatcher();
            if (dispatcher.CheckAccess())
            {
                return factory();
            }
            return dispatcher.Invoke(factory);
        }

        /// <inheritdoc/>
        public override IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfMessageDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfPromptDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<string> CreateInputDialog(InputDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfInputDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfPasswordDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<bool> CreateTextDialog(TextDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfTextDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<bool> CreateToastDialog(ToastDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfToastDialog(settings, context, _theme));

        // ========== 以下类型将在后续里程碑实现 ==========

        /// <inheritdoc/>
        public override IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfSelectionDialog<T>(settings, collection, defaultSelectedItem, context, _theme));

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfMultipleSelectionDialog<T>(settings, collection, defaultSelectedItems, context, _theme));

        /// <inheritdoc/>
        public override IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfDateTimeDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfComboBoxInputDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfNumberInputDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfSliderDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfRangeSliderDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfProgressDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfColorDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfDataEntrySheetDialog(sheet, settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfOpenFileDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfOpenFolderDialog(settings, context, _theme));

        /// <inheritdoc/>
        public override IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, IWin32Owner? context = null)
            => CreateOnUIThread(() => new WpfSaveFileDialog(settings, context, _theme));

        /// <inheritdoc/>
        protected override async Task<DialogResult<TResult>> ShowAsync<TResult>(Func<IDialog<TResult>> dialogFactory, CancellationToken cancellationToken)
        {
            var dispatcher = EnsureDispatcher();

            if (dispatcher.CheckAccess())
            {
                return await base.ShowAsync(dialogFactory, cancellationToken).ConfigureAwait(false);
            }

            return await dispatcher.InvokeAsync(async () =>
            {
                return await base.ShowAsync(dialogFactory, cancellationToken).ConfigureAwait(false);
            }).Task.Unwrap().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, CancellationToken cancellationToken)
        {
            var dispatcher = EnsureDispatcher();

            if (dispatcher.CheckAccess())
            {
                return await base.ShowAsync(dialog, cancellationToken).ConfigureAwait(false);
            }

            return await dispatcher.InvokeAsync(async () =>
            {
                return await base.ShowAsync(dialog, cancellationToken).ConfigureAwait(false);
            }).Task.Unwrap().ConfigureAwait(false);
        }
    }
}
