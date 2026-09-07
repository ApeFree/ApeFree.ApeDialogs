using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class DialogSettingsTests
    {
        [Fact]
        public void MessageDialog_DefaultOptions()
        {
            var settings = new MessageDialogSettings();
            Assert.Single(settings.DefaultOptions);
            Assert.Equal("Confirm", settings.DefaultOptions[0].Text);
            Assert.Equal(DialogOptionTag.Positive, settings.DefaultOptions[0].Tag);
        }

        [Fact]
        public void PromptDialog_DefaultOptions()
        {
            var settings = new PromptDialogSettings();
            Assert.Equal(2, settings.DefaultOptions.Count);
            Assert.Equal("Yes", settings.DefaultOptions[0].Text);
            Assert.Equal("No", settings.DefaultOptions[1].Text);
        }

        [Fact]
        public void InputDialog_CancelOption_CancelsDialog()
        {
            var settings = new InputDialogSettings();
            Assert.NotNull(settings.CancelOption.Action);
            var dialog = new TestDialog<string>(settings);
            settings.CancelOption.Action(dialog);
            Assert.True(dialog.DismissHandlerCalled);
        }

        [Fact]
        public void PasswordDialog_InheritsInputDefaults()
        {
            var settings = new PasswordDialogSettings();
            Assert.NotNull(settings.CancelOption);
            Assert.Equal('●', settings.PasswordChar);
        }

        [Fact]
        public void DateTimeDialog_FormatIs24Hour()
        {
            var settings = new DateTimeDialogSettings();
            Assert.Contains("HH", settings.DateTimeFormat);
            Assert.DoesNotContain("hh", settings.DateTimeFormat);
        }

        [Fact]
        public void OpenFileDialog_HasDefaultSearchPattern()
        {
            var settings = new OpenFileDialogSettings();
            Assert.Equal("*", settings.SearchPattern);
        }

        [Fact]
        public void SelectionDialog_UsesItemDisplayConverter()
        {
            var settings = new SelectionDialogSettings<int>();
            Assert.Equal("123", settings.ItemDisplayConverter(123));
        }

        [Fact]
        public void CancelOption_Factory()
        {
            var option = DialogOption.CreateCancelOption("Cancel");
            Assert.NotNull(option.Action);
            Assert.Equal(DialogOptionTag.Negative, option.Tag);
            Assert.True(option.IsEnabled);
        }

        [Fact]
        public void DialogOption_RecordSemantics()
        {
            var a = new DialogOption("OK");
            var b = new DialogOption("OK");
            var c = a with { Text = "Cancel" };

            Assert.Equal(a, b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void OptionSelectedEventArgs_CarriesDialog()
        {
            var settings = new MessageDialogSettings();
            var dialog = new TestDialog<bool>(settings);
            var option = settings.ConfirmOption;
            var args = new OptionSelectedEventArgs(dialog, option);

            Assert.Same(dialog, args.Dialog);
            Assert.Same(option, args.Option);
        }

        [Fact]
        public void Settings_Defaults()
        {
            var settings = new MessageDialogSettings { Title = "T", Content = "C" };
            Assert.Equal("T", settings.Title);
            Assert.Equal("C", settings.Content);
            Assert.True(settings.Cancelable);
            Assert.Null(settings.Theme);
        }
    }
}
