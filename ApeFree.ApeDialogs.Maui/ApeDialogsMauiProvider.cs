using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Maui
{
    /// <summary>
    /// ApeDialogs 的 .NET MAUI 提供器实现。
    /// </summary>
    public class ApeDialogsMauiProvider : DialogProvider<IMauiDialogContext>
    {
        private readonly IDialogTheme _theme;

        public ApeDialogsMauiProvider(IDialogTheme? theme = null)
        {
            _theme = theme ?? DefaultTheme.Instance;
        }

        public override IDialogTheme Theme => _theme;

        public override IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiMessageDialog(settings, context, _theme);

        public override IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiPromptDialog(settings, context, _theme);

        public override IDialog<string> CreateInputDialog(InputDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiInputDialog(settings, context, _theme);

        public override IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiPasswordDialog(settings, context, _theme);

        public override IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiSelectionDialog<T>(settings, collection, defaultSelectedItem, context, _theme);

        public override IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiMultipleSelectionDialog<T>(settings, collection, defaultSelectedItems, context, _theme);

        public override IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiDateTimeDialog(settings, context, _theme);

        public override IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiDataEntrySheetDialog(sheet, settings, context, _theme);

        public override IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiOpenFileDialog(settings, context, _theme);

        public override IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiOpenFolderDialog(settings, context, _theme);

        public override IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiNumberInputDialog(settings, context, _theme);

        public override IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiSliderDialog(settings, context, _theme);

        public override IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiRangeSliderDialog(settings, context, _theme);

        public override IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiProgressDialog(settings, context, _theme);

        public override IDialog<bool> CreateToastDialog(ToastDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiToastDialog(settings, context, _theme);

        public override IDialog<bool> CreateTextDialog(TextDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiTextDialog(settings, context, _theme);

        public override IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiColorDialog(settings, context, _theme);

        public override IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiSaveFileDialog(settings, context, _theme);

        public override IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, IMauiDialogContext? context = default)
            => new Maui.Dialogs.MauiComboBoxInputDialog(settings, context, _theme);
    }
}
