using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// <see cref="IDialogProvider{TContext}"/> 扩展方法集合，提供基于 Lambda 委托的快捷创建与展示支持。
    /// </summary>
    public static class DialogProviderExtensions
    {
        #region 1. MessageDialog (消息对话框)

        /// <summary>
        /// 通过配置委托创建消息对话框。
        /// </summary>
        public static IDialog<bool> CreateMessageDialog<TContext>(this IDialogProvider<TContext> provider, Action<MessageDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new MessageDialogSettings();
            configure(settings);
            return provider.CreateMessageDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示消息对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<bool>> ShowMessageDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<MessageDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new MessageDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowMessageDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateMessageDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 2. PromptDialog (提示/确认对话框)

        /// <summary>
        /// 通过配置委托创建提示对话框。
        /// </summary>
        public static IDialog<bool> CreatePromptDialog<TContext>(this IDialogProvider<TContext> provider, Action<PromptDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new PromptDialogSettings();
            configure(settings);
            return provider.CreatePromptDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示提示对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<bool>> ShowPromptDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<PromptDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new PromptDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowPromptDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreatePromptDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 3. InputDialog (文本输入对话框)

        /// <summary>
        /// 通过配置委托创建输入对话框。
        /// </summary>
        public static IDialog<string> CreateInputDialog<TContext>(this IDialogProvider<TContext> provider, Action<InputDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new InputDialogSettings();
            configure(settings);
            return provider.CreateInputDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示输入对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<string>> ShowInputDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<InputDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new InputDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowInputDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateInputDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 4. PasswordDialog (密码输入对话框)

        /// <summary>
        /// 通过配置委托创建密码对话框。
        /// </summary>
        public static IDialog<string> CreatePasswordDialog<TContext>(this IDialogProvider<TContext> provider, Action<PasswordDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new PasswordDialogSettings();
            configure(settings);
            return provider.CreatePasswordDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示密码对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<string>> ShowPasswordDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<PasswordDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new PasswordDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowPasswordDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreatePasswordDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 5. SelectionDialog (单选对话框)

        /// <summary>
        /// 通过配置委托创建单选对话框。
        /// </summary>
        public static IDialog<T> CreateSelectionDialog<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, T defaultSelectedItem, Action<SelectionDialogSettings<T>>? configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            var settings = new SelectionDialogSettings<T>();
            configure?.Invoke(settings);
            return provider.CreateSelectionDialog(settings, collection, defaultSelectedItem, context);
        }

        /// <summary>
        /// 通过配置委托创建单选对话框。
        /// </summary>
        public static IDialog<T> CreateSelectionDialog<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, Action<SelectionDialogSettings<T>> configure, TContext? context = default)
            => provider.CreateSelectionDialog(collection, default!, configure, context);

        /// <summary>
        /// 通过配置委托显示单选对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<T>> ShowSelectionDialogAsync<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, T defaultSelectedItem, Action<SelectionDialogSettings<T>>? configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            var settings = new SelectionDialogSettings<T>();
            configure?.Invoke(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowSelectionDialogAsync(settings, collection, defaultSelectedItem, context, cancellationToken);
            }
            return provider.CreateSelectionDialog(settings, collection, defaultSelectedItem, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 通过配置委托显示单选对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<T>> ShowSelectionDialogAsync<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, Action<SelectionDialogSettings<T>> configure, TContext? context = default, CancellationToken cancellationToken = default)
            => provider.ShowSelectionDialogAsync(collection, default!, configure, context, cancellationToken);

        #endregion

        #region 6. MultipleSelectionDialog (多选对话框)

        /// <summary>
        /// 通过配置委托创建多选对话框。
        /// </summary>
        public static IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems, Action<MultipleSelectionDialogSettings<T>>? configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            var settings = new MultipleSelectionDialogSettings<T>();
            configure?.Invoke(settings);
            return provider.CreateMultipleSelectionDialog(settings, collection, defaultSelectedItems, context);
        }

        /// <summary>
        /// 通过配置委托创建多选对话框。
        /// </summary>
        public static IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, Action<MultipleSelectionDialogSettings<T>> configure, TContext? context = default)
            => provider.CreateMultipleSelectionDialog(collection, null, configure, context);

        /// <summary>
        /// 通过配置委托显示多选对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<IReadOnlyList<T>>> ShowMultipleSelectionDialogAsync<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems, Action<MultipleSelectionDialogSettings<T>>? configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            var settings = new MultipleSelectionDialogSettings<T>();
            configure?.Invoke(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowMultipleSelectionDialogAsync(settings, collection, defaultSelectedItems, context, cancellationToken);
            }
            return provider.CreateMultipleSelectionDialog(settings, collection, defaultSelectedItems, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 通过配置委托显示多选对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<IReadOnlyList<T>>> ShowMultipleSelectionDialogAsync<T, TContext>(this IDialogProvider<TContext> provider, IEnumerable<T> collection, Action<MultipleSelectionDialogSettings<T>> configure, TContext? context = default, CancellationToken cancellationToken = default)
            => provider.ShowMultipleSelectionDialogAsync(collection, null, configure, context, cancellationToken);

        #endregion

        #region 7. DateTimeDialog (日期时间对话框)

        /// <summary>
        /// 通过配置委托创建日期时间对话框。
        /// </summary>
        public static IDialog<DateTime> CreateDateTimeDialog<TContext>(this IDialogProvider<TContext> provider, Action<DateTimeDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new DateTimeDialogSettings();
            configure(settings);
            return provider.CreateDateTimeDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示日期时间对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<DateTime>> ShowDateTimeDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<DateTimeDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new DateTimeDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowDateTimeDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateDateTimeDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 8. DataEntrySheetDialog (多字段表单对话框)

        /// <summary>
        /// 通过配置委托创建多字段表单对话框。
        /// </summary>
        public static IDialog<DataEntrySheet> CreateDataEntrySheetDialog<TContext>(this IDialogProvider<TContext> provider, DataEntrySheet sheet, Action<DataEntrySheetDialogSettings>? configure = null, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            var settings = new DataEntrySheetDialogSettings();
            configure?.Invoke(settings);
            return provider.CreateDataEntrySheetDialog(sheet, settings, context);
        }

        /// <summary>
        /// 通过配置委托显示多字段表单对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<DataEntrySheet>> ShowDataEntrySheetDialogAsync<TContext>(this IDialogProvider<TContext> provider, DataEntrySheet sheet, Action<DataEntrySheetDialogSettings>? configure = null, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (sheet is null) throw new ArgumentNullException(nameof(sheet));
            var settings = new DataEntrySheetDialogSettings();
            configure?.Invoke(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowDataEntrySheetDialogAsync(sheet, settings, context, cancellationToken);
            }
            return provider.CreateDataEntrySheetDialog(sheet, settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 9. OpenFileDialog (打开文件对话框)

        /// <summary>
        /// 通过配置委托创建打开文件对话框。
        /// </summary>
        public static IDialog<IReadOnlyList<string>> CreateOpenFileDialog<TContext>(this IDialogProvider<TContext> provider, Action<OpenFileDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new OpenFileDialogSettings();
            configure(settings);
            return provider.CreateOpenFileDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示打开文件对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<IReadOnlyList<string>>> ShowOpenFileDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<OpenFileDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new OpenFileDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowOpenFileDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateOpenFileDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 10. OpenFolderDialog (打开文件夹对话框)

        /// <summary>
        /// 通过配置委托创建打开文件夹对话框。
        /// </summary>
        public static IDialog<IReadOnlyList<string>> CreateOpenFolderDialog<TContext>(this IDialogProvider<TContext> provider, Action<OpenFolderDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new OpenFolderDialogSettings();
            configure(settings);
            return provider.CreateOpenFolderDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示打开文件夹对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<IReadOnlyList<string>>> ShowOpenFolderDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<OpenFolderDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new OpenFolderDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowOpenFolderDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateOpenFolderDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 11. NumberInputDialog (数值输入对话框)

        /// <summary>
        /// 通过配置委托创建数值输入对话框。
        /// </summary>
        public static IDialog<decimal> CreateNumberInputDialog<TContext>(this IDialogProvider<TContext> provider, Action<NumberInputDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new NumberInputDialogSettings();
            configure(settings);
            return provider.CreateNumberInputDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示数值输入对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<decimal>> ShowNumberInputDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<NumberInputDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new NumberInputDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowNumberInputDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateNumberInputDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 12. SliderDialog (单滑块调节对话框)

        /// <summary>
        /// 通过配置委托创建单滑块调节对话框。
        /// </summary>
        public static IAdjustDialog<decimal> CreateSliderDialog<TContext>(this IDialogProvider<TContext> provider, Action<SliderDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new SliderDialogSettings();
            configure(settings);
            return provider.CreateSliderDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示单滑块调节对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<decimal>> ShowSliderDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<SliderDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new SliderDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowSliderDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateSliderDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 13. RangeSliderDialog (双滑块范围调节对话框)

        /// <summary>
        /// 通过配置委托创建双滑块范围调节对话框。
        /// </summary>
        public static IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog<TContext>(this IDialogProvider<TContext> provider, Action<RangeSliderDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new RangeSliderDialogSettings();
            configure(settings);
            return provider.CreateRangeSliderDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示双滑块范围调节对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<ValueRange<decimal>>> ShowRangeSliderDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<RangeSliderDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new RangeSliderDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowRangeSliderDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateRangeSliderDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 14. ProgressDialog (进度对话框)

        /// <summary>
        /// 通过配置委托创建进度对话框。
        /// </summary>
        public static IProgressDialog CreateProgressDialog<TContext>(this IDialogProvider<TContext> provider, Action<ProgressDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ProgressDialogSettings();
            configure(settings);
            return provider.CreateProgressDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示进度对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<bool>> ShowProgressDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<ProgressDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ProgressDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowProgressDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateProgressDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 15. ToastDialog (轻提示对话框)

        /// <summary>
        /// 通过配置委托创建轻提示对话框。
        /// </summary>
        public static IDialog<bool> CreateToastDialog<TContext>(this IDialogProvider<TContext> provider, Action<ToastDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ToastDialogSettings();
            configure(settings);
            return provider.CreateToastDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示轻提示对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<bool>> ShowToastDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<ToastDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ToastDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowToastDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateToastDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 16. TextDialog (长文本查看对话框)

        /// <summary>
        /// 通过配置委托创建长文本查看对话框。
        /// </summary>
        public static IDialog<bool> CreateTextDialog<TContext>(this IDialogProvider<TContext> provider, Action<TextDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new TextDialogSettings();
            configure(settings);
            return provider.CreateTextDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示长文本查看对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<bool>> ShowTextDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<TextDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new TextDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowTextDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateTextDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 17. ColorDialog (颜色选择对话框)

        /// <summary>
        /// 通过配置委托创建颜色选择对话框。
        /// </summary>
        public static IDialog<ApeColor> CreateColorDialog<TContext>(this IDialogProvider<TContext> provider, Action<ColorDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ColorDialogSettings();
            configure(settings);
            return provider.CreateColorDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示颜色选择对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<ApeColor>> ShowColorDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<ColorDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ColorDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowColorDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateColorDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 18. SaveFileDialog (保存文件对话框)

        /// <summary>
        /// 通过配置委托创建保存文件对话框。
        /// </summary>
        public static IDialog<string> CreateSaveFileDialog<TContext>(this IDialogProvider<TContext> provider, Action<SaveFileDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new SaveFileDialogSettings();
            configure(settings);
            return provider.CreateSaveFileDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示保存文件对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<string>> ShowSaveFileDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<SaveFileDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new SaveFileDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowSaveFileDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateSaveFileDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region 19. ComboBoxInputDialog (可编辑下拉输入对话框)

        /// <summary>
        /// 通过配置委托创建可编辑下拉输入对话框。
        /// </summary>
        public static IDialog<string> CreateComboBoxInputDialog<TContext>(this IDialogProvider<TContext> provider, Action<ComboBoxInputDialogSettings> configure, TContext? context = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ComboBoxInputDialogSettings();
            configure(settings);
            return provider.CreateComboBoxInputDialog(settings, context);
        }

        /// <summary>
        /// 通过配置委托显示可编辑下拉输入对话框并等待结果。
        /// </summary>
        public static Task<DialogResult<string>> ShowComboBoxInputDialogAsync<TContext>(this IDialogProvider<TContext> provider, Action<ComboBoxInputDialogSettings> configure, TContext? context = default, CancellationToken cancellationToken = default)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var settings = new ComboBoxInputDialogSettings();
            configure(settings);
            if (provider is DialogProvider<TContext> dp)
            {
                return dp.ShowComboBoxInputDialogAsync(settings, context, cancellationToken);
            }
            return provider.CreateComboBoxInputDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion
    }
}
