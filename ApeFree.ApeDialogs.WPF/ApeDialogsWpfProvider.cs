using System;
using System.Collections.Generic;
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

        /// <inheritdoc/>
        public override IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, IWin32Owner? context = null)
            => new WpfMessageDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, IWin32Owner? context = null)
            => new WpfPromptDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<string> CreateInputDialog(InputDialogSettings settings, IWin32Owner? context = null)
            => new WpfInputDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, IWin32Owner? context = null)
            => new WpfPasswordDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<bool> CreateTextDialog(TextDialogSettings settings, IWin32Owner? context = null)
            => new WpfTextDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<bool> CreateToastDialog(ToastDialogSettings settings, IWin32Owner? context = null)
            => new WpfToastDialog(settings, context, _theme);

        // ========== 以下类型将在后续里程碑实现 ==========

        /// <inheritdoc/>
        public override IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, IWin32Owner? context = null)
            => new WpfSelectionDialog<T>(settings, collection, defaultSelectedItem, context, _theme);

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, IWin32Owner? context = null)
            => new WpfMultipleSelectionDialog<T>(settings, collection, defaultSelectedItems, context, _theme);

        /// <inheritdoc/>
        public override IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, IWin32Owner? context = null)
            => new WpfDateTimeDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, IWin32Owner? context = null)
            => new WpfComboBoxInputDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, IWin32Owner? context = null)
            => new WpfNumberInputDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, IWin32Owner? context = null)
            => new WpfSliderDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, IWin32Owner? context = null)
            => new WpfRangeSliderDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, IWin32Owner? context = null)
            => new WpfProgressDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, IWin32Owner? context = null)
            => new WpfColorDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, IWin32Owner? context = null)
            => new WpfDataEntrySheetDialog(sheet, settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, IWin32Owner? context = null)
            => new WpfOpenFileDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, IWin32Owner? context = null)
            => new WpfOpenFolderDialog(settings, context, _theme);

        /// <inheritdoc/>
        public override IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, IWin32Owner? context = null)
            => new WpfSaveFileDialog(settings, context, _theme);
    }
}
