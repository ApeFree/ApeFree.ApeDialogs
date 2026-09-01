using System.Collections.Generic;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class ProviderTests
    {
        private readonly TestProvider _provider = new TestProvider();

        [Fact]
        public async Task ShowMessageDialogAsync_ComposesCreateAndShow()
        {
            var task = _provider.ShowMessageDialogAsync(new MessageDialogSettings());
            var dialog = _provider.LastCreatedDialog<bool>();
            Assert.True(dialog.ShowHandlerCalled, "创建后应立即显示");

            dialog.SimulateSubmit(true);
            var result = await task;

            Assert.False(result.IsCanceled);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ShowInputDialogAsync_ReturnsSubmittedValue()
        {
            var task = _provider.ShowInputDialogAsync(new InputDialogSettings());
            var dialog = _provider.LastCreatedDialog<string>();

            dialog.SimulateSubmit("hello");

            var result = await task;
            Assert.Equal("hello", result.Value);
        }

        [Fact]
        public async Task ShowPromptDialogAsync_CancelViaDialog()
        {
            var task = _provider.ShowPromptDialogAsync(new PromptDialogSettings());
            var dialog = _provider.LastCreatedDialog<bool>();

            dialog.Cancel();
            var result = await task;

            Assert.True(result.IsCanceled);
        }

        [Fact]
        public async Task ShowMultipleSelectionDialogAsync_EndToEnd()
        {
            var task = _provider.ShowMultipleSelectionDialogAsync(
                new MultipleSelectionDialogSettings<int>(), new[] { 1, 2, 3 }, new[] { 1 });

            var dialog = _provider.LastCreatedDialog<IReadOnlyList<int>>();
            dialog.SimulateSubmit(new[] { 2, 3 });
            var result = await task;

            Assert.Equal(new[] { 2, 3 }, result.Value);
        }

        [Fact]
        public async Task ShowMessageDialogAsync_PassesContext()
        {
            var context = new object();
            var task = _provider.ShowMessageDialogAsync(new MessageDialogSettings(), context);

            Assert.Same(context, _provider.LastContext);

            _provider.LastCreatedDialog<bool>().Cancel();
            await task;
        }

        [Fact]
        public void CreateSelectionDialog_GenericWorks()
        {
            var dialog = _provider.CreateSelectionDialog(new SelectionDialogSettings<int>(), new[] { 1, 2, 3 }, 2);
            Assert.IsType<TestDialog<int>>(dialog);
        }

        [Fact]
        public void CreateOpenFileDialog_ReturnsTypedDialog()
        {
            var dialog = _provider.CreateOpenFileDialog(new OpenFileDialogSettings());
            Assert.IsType<TestDialog<IReadOnlyList<string>>>(dialog);
        }

        [Fact]
        public void Provider_ExposesDefaultTheme()
        {
            Assert.NotNull(_provider.Theme);
        }
    }
}
