using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.WinForms
{
    /// <summary>
    /// WinForms 宿主扩展：以 Form 作为上下文直接调用 ApeDialogs（内部包装为 <see cref="FormOwner"/>）。
    /// 深层定制（Create 模式）时请显式使用 <c>new FormOwner(form)</c>。
    /// </summary>
    public static class ApeDialogsWpfProviderWinFormsExtensions
    {
        private static FormOwner? OwnerOf(Form? form) => form is null ? null : new FormOwner(form);

        /// <summary>显示消息对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<bool>> ShowMessageDialogAsync(this ApeDialogsWpfProvider provider, MessageDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowMessageDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示提示对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<bool>> ShowPromptDialogAsync(this ApeDialogsWpfProvider provider, PromptDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowPromptDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示输入对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<string>> ShowInputDialogAsync(this ApeDialogsWpfProvider provider, InputDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowInputDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示密码对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<string>> ShowPasswordDialogAsync(this ApeDialogsWpfProvider provider, PasswordDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowPasswordDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示长文本查看对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<bool>> ShowTextDialogAsync(this ApeDialogsWpfProvider provider, TextDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowTextDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示轻提示对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<bool>> ShowToastDialogAsync(this ApeDialogsWpfProvider provider, ToastDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowToastDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示单选对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<T>> ShowSelectionDialogAsync<T>(this ApeDialogsWpfProvider provider, SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowSelectionDialogAsync(settings, collection, defaultSelectedItem, OwnerOf(owner), cancellationToken);

        /// <summary>显示多选对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<IReadOnlyList<T>>> ShowMultipleSelectionDialogAsync<T>(this ApeDialogsWpfProvider provider, MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowMultipleSelectionDialogAsync(settings, collection, defaultSelectedItems, OwnerOf(owner), cancellationToken);

        /// <summary>显示日期时间对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<DateTime>> ShowDateTimeDialogAsync(this ApeDialogsWpfProvider provider, DateTimeDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowDateTimeDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示可编辑下拉输入对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<string>> ShowComboBoxInputDialogAsync(this ApeDialogsWpfProvider provider, ComboBoxInputDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowComboBoxInputDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示数值输入对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<decimal>> ShowNumberInputDialogAsync(this ApeDialogsWpfProvider provider, NumberInputDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowNumberInputDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示单滑块调节对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<decimal>> ShowSliderDialogAsync(this ApeDialogsWpfProvider provider, SliderDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowSliderDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示双滑块范围调节对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<ValueRange<decimal>>> ShowRangeSliderDialogAsync(this ApeDialogsWpfProvider provider, RangeSliderDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowRangeSliderDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示进度对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<bool>> ShowProgressDialogAsync(this ApeDialogsWpfProvider provider, ProgressDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowProgressDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示颜色选择对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<ApeColor>> ShowColorDialogAsync(this ApeDialogsWpfProvider provider, ColorDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowColorDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示多字段表单对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<DataEntrySheet>> ShowDataEntrySheetDialogAsync(this ApeDialogsWpfProvider provider, DataEntrySheet sheet, DataEntrySheetDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowDataEntrySheetDialogAsync(sheet, settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示打开文件对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<IReadOnlyList<string>>> ShowOpenFileDialogAsync(this ApeDialogsWpfProvider provider, OpenFileDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowOpenFileDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示打开文件夹对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<IReadOnlyList<string>>> ShowOpenFolderDialogAsync(this ApeDialogsWpfProvider provider, OpenFolderDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowOpenFolderDialogAsync(settings, OwnerOf(owner), cancellationToken);

        /// <summary>显示保存文件对话框（WinForms 宿主）。</summary>
        public static Task<DialogResult<string>> ShowSaveFileDialogAsync(this ApeDialogsWpfProvider provider, SaveFileDialogSettings settings, Form? owner = null, CancellationToken cancellationToken = default)
            => provider.ShowSaveFileDialogAsync(settings, OwnerOf(owner), cancellationToken);
    }
}
