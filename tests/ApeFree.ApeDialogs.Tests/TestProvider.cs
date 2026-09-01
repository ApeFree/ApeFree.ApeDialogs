using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Tests
{
    /// <summary>
    /// 测试用 Provider（兼作实现方参考实现）。
    /// </summary>
    public class TestProvider : DialogProvider<object>
    {
        public object? LastContext { get; private set; }
        public int CreatedCount { get; private set; }
        private object? _lastCreatedDialog;

        /// <summary>
        /// 最近一次创建的 Dialog。
        /// </summary>
        public TestDialog<TResult> LastCreatedDialog<TResult>()
            => (TestDialog<TResult>)_lastCreatedDialog!;

        private TestDialog<TResult> Create<TResult>(DialogSettings<TResult> settings, object? context)
        {
            LastContext = context;
            CreatedCount++;
            var dialog = new TestDialog<TResult>(settings);
            _lastCreatedDialog = dialog;
            return dialog;
        }

        public override IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<string> CreateInputDialog(InputDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, object? context = null)
            => Create(settings, context);

        public override IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, object? context = null)
            => Create(settings, context);

        public override IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, object? context = null)
            => CreateAdjust(settings, context);

        public override IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, object? context = null)
            => CreateAdjust(settings, context);

        public override IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, object? context = null)
            => CreateProgress(settings, context);

        public override IDialog<bool> CreateToastDialog(ToastDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<bool> CreateTextDialog(TextDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, object? context = null)
            => Create(settings, context);

        public override IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, object? context = null)
            => Create(settings, context);

        private TestAdjustDialog<TResult> CreateAdjust<TResult>(DialogSettings<TResult> settings, object? context)
        {
            LastContext = context;
            CreatedCount++;
            var dialog = new TestAdjustDialog<TResult>(settings);
            _lastCreatedDialog = dialog;
            return dialog;
        }

        private TestProgressDialog CreateProgress(ProgressDialogSettings settings, object? context)
        {
            LastContext = context;
            CreatedCount++;
            var dialog = new TestProgressDialog(settings);
            _lastCreatedDialog = dialog;
            return dialog;
        }
    }
}
