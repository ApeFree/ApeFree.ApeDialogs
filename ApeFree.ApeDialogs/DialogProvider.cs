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
    /// Dialog 提供器抽象基类。
    /// 实现方只需实现 10 个 <c>CreateXxxDialog</c> 抽象方法；
    /// <c>ShowXxxDialogAsync</c> 系列由基类通过 Create + ShowAsync 组合提供默认实现。
    /// </summary>
    /// <typeparam name="TContext">上下文类型</typeparam>
    public abstract class DialogProvider<TContext> : IDialogProvider<TContext>
    {
        /// <summary>
        /// 默认主题。
        /// </summary>
        public virtual IDialogTheme Theme => DefaultTheme.Instance;

        /// <inheritdoc/>
        public abstract IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<string> CreateInputDialog(InputDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<bool> CreateToastDialog(ToastDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<bool> CreateTextDialog(TextDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, TContext? context = default);

        /// <inheritdoc/>
        public abstract IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, TContext? context = default);

        /// <summary>
        /// 显示消息对话框并等待结果。
        /// </summary>
        public Task<DialogResult<bool>> ShowMessageDialogAsync(MessageDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateMessageDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示提示对话框并等待结果。
        /// </summary>
        public Task<DialogResult<bool>> ShowPromptDialogAsync(PromptDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreatePromptDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示输入对话框并等待结果。
        /// </summary>
        public Task<DialogResult<string>> ShowInputDialogAsync(InputDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateInputDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示密码对话框并等待结果。
        /// </summary>
        public Task<DialogResult<string>> ShowPasswordDialogAsync(PasswordDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreatePasswordDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示单选对话框并等待结果。
        /// </summary>
        public Task<DialogResult<T>> ShowSelectionDialogAsync<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateSelectionDialog(settings, collection, defaultSelectedItem, context), cancellationToken);

        /// <summary>
        /// 显示多选对话框并等待结果。
        /// </summary>
        public Task<DialogResult<IReadOnlyList<T>>> ShowMultipleSelectionDialogAsync<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateMultipleSelectionDialog(settings, collection, defaultSelectedItems, context), cancellationToken);

        /// <summary>
        /// 显示日期时间对话框并等待结果。
        /// </summary>
        public Task<DialogResult<DateTime>> ShowDateTimeDialogAsync(DateTimeDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateDateTimeDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示多字段表单对话框并等待结果。
        /// </summary>
        public Task<DialogResult<DataEntrySheet>> ShowDataEntrySheetDialogAsync(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateDataEntrySheetDialog(sheet, settings, context), cancellationToken);

        /// <summary>
        /// 显示打开文件对话框并等待结果。
        /// </summary>
        public Task<DialogResult<IReadOnlyList<string>>> ShowOpenFileDialogAsync(OpenFileDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateOpenFileDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示打开文件夹对话框并等待结果。
        /// </summary>
        public Task<DialogResult<IReadOnlyList<string>>> ShowOpenFolderDialogAsync(OpenFolderDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateOpenFolderDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示数值输入对话框并等待结果。
        /// </summary>
        public Task<DialogResult<decimal>> ShowNumberInputDialogAsync(NumberInputDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateNumberInputDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示单滑块调节对话框并等待结果（调节过程经 <see cref="DialogSettings{TResult}.OnValueChanged"/> 实时影响外界）。
        /// </summary>
        public Task<DialogResult<decimal>> ShowSliderDialogAsync(SliderDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateSliderDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示双滑块范围调节对话框并等待结果。
        /// </summary>
        public Task<DialogResult<ValueRange<decimal>>> ShowRangeSliderDialogAsync(RangeSliderDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateRangeSliderDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示进度对话框并等待结果（显示期间通过 <see cref="IProgressDialog"/> 推进进度）。
        /// </summary>
        public Task<DialogResult<bool>> ShowProgressDialogAsync(ProgressDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateProgressDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示轻提示对话框（通常不等待该 Task，按 Duration 自动消失）。
        /// </summary>
        public Task<DialogResult<bool>> ShowToastDialogAsync(ToastDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateToastDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示长文本查看对话框并等待结果。
        /// </summary>
        public Task<DialogResult<bool>> ShowTextDialogAsync(TextDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateTextDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示颜色选择对话框并等待结果。
        /// </summary>
        public Task<DialogResult<ApeColor>> ShowColorDialogAsync(ColorDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateColorDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示保存文件对话框并等待结果。
        /// </summary>
        public Task<DialogResult<string>> ShowSaveFileDialogAsync(SaveFileDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateSaveFileDialog(settings, context), cancellationToken);

        /// <summary>
        /// 显示可编辑下拉输入对话框并等待结果。
        /// </summary>
        public Task<DialogResult<string>> ShowComboBoxInputDialogAsync(ComboBoxInputDialogSettings settings, TContext? context = default, CancellationToken cancellationToken = default)
            => ShowAsync(CreateComboBoxInputDialog(settings, context), cancellationToken);

        private static Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, CancellationToken cancellationToken)
            => dialog.ShowAsync(cancellationToken);
    }
}
