using System;
using System.Threading;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class BaseDialogTests
    {
        [Fact]
        public async Task ShowAsync_Completes_WhenDismissed()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings { Title = "Input" });
            var task = dialog.ShowAsync();

            dialog.SimulateSubmit("value");

            var result = await task;
            Assert.False(result.IsCanceled);
            Assert.Equal("value", result.Value);
            Assert.False(dialog.IsShown);
        }

        [Fact]
        public async Task Cancel_ReturnsCanceledResult()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings());
            var task = dialog.ShowAsync();
            dialog.Cancel();

            var result = await task;
            Assert.True(result.IsCanceled);
        }

        [Fact]
        public async Task ShowAsync_TokenCanceled_ReturnsCanceled_WithoutShowing()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings());
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await dialog.ShowAsync(cts.Token);

            Assert.True(result.IsCanceled);
            Assert.False(dialog.ShowHandlerCalled);
            Assert.False(dialog.IsShown);
        }

        [Fact]
        public async Task ShowAsync_TokenCanceledWhileWaiting_ClosesDialog()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings());
            using var cts = new CancellationTokenSource();

            var task = dialog.ShowAsync(cts.Token);
            Assert.True(dialog.IsShown);

            cts.Cancel();
            var result = await task;

            Assert.True(result.IsCanceled);
            Assert.True(dialog.DismissHandlerCalled);
        }

        [Fact]
        public async Task Duration_AutoDismissesAsCanceled()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings { Duration = TimeSpan.FromMilliseconds(50) });
            var result = await dialog.ShowAsync();
            Assert.True(result.IsCanceled);
            Assert.True(dialog.DismissHandlerCalled);
        }

        [Fact]
        public async Task LifecycleEvents_And_SettingsDelegates_Fire()
        {
            int showing = 0, shown = 0, dismissing = 0, dismissed = 0;
            int settingsShown = 0, settingsDismissed = 0;

            var dialog = new TestDialog<string>(new InputDialogSettings
            {
                OnShown = d => settingsShown++,
                OnDismissed = d => settingsDismissed++,
            });

            dialog.Showing += (_, _) => showing++;
            dialog.Shown += (_, _) => shown++;
            dialog.Dismissing += (_, _) => dismissing++;
            dialog.Dismissed += (_, _) => dismissed++;

            var task = dialog.ShowAsync();
            dialog.SimulateSubmit("v");
            await task;

            Assert.Equal(1, showing);
            Assert.Equal(1, shown);
            Assert.Equal(1, dismissing);
            Assert.Equal(1, dismissed);
            Assert.Equal(1, settingsShown);
            Assert.Equal(1, settingsDismissed);
        }

        [Fact]
        public async Task TrySubmit_ValidationFailure_KeepsDialogOpen()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings
            {
                Validator = v => v == "ok" ? ValidationResult.Success : ValidationResult.Failure("not ok"),
            });
            var task = dialog.ShowAsync();

            Assert.False(dialog.SimulateSubmit("bad"));
            Assert.False(dialog.DismissHandlerCalled);
            Assert.True(dialog.IsShown);
            Assert.False(await Task.WhenAny(task, Task.Delay(200)) == task, "dialog should not have completed");

            Assert.True(dialog.SimulateSubmit("ok"));
            Assert.False((await task).IsCanceled);
        }

        [Fact]
        public async Task OnValidationFailed_Called_OnFailure()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings
            {
                Validator = _ => ValidationResult.Failure("nope"),
            });
            var task = dialog.ShowAsync();

            dialog.SimulateSubmit("x");

            Assert.NotNull(dialog.LastValidationFailure);
            Assert.Equal("nope", dialog.LastValidationFailure!.Value.ErrorMessage);
            dialog.Cancel();
            await task;
        }

        [Fact]
        public async Task DefaultOptions_WithEmptyText_AreFilteredOut()
        {
            var settings = new MessageDialogSettings
            {
                ConfirmOption = new DialogOption("   "),
            };
            var dialog = new TestDialog<bool>(settings);

            var task = dialog.ShowAsync();
            dialog.Cancel();
            await task;

            Assert.Empty(dialog.Options);
        }

        [Fact]
        public async Task ShowAsync_Twice_Throws()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings());
            var task = dialog.ShowAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(() => dialog.ShowAsync());

            dialog.Cancel();
            await task;
        }

        [Fact]
        public async Task Dismiss_AfterDismiss_IsNoOp()
        {
            var dialog = new TestDialog<string>(new InputDialogSettings());
            var task = dialog.ShowAsync();
            dialog.SimulateSubmit("first");

            dialog.Cancel();
            var result = await task;

            Assert.Equal("first", result.Value);
        }

        [Fact]
        public void Constructor_Throws_OnNullSettings()
        {
            Assert.Throws<ArgumentNullException>(() => new TestDialog<string>(null!));
        }
    }
}
