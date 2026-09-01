using System;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 提示对话框（Yes/No，WPF 实现）。
    /// </summary>
    public class WpfPromptDialog : WpfDialogBase<bool>
    {
        private readonly PromptDialogSettings _settings;

        public WpfPromptDialog(PromptDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.PositiveOption))
            {
                button.Click += (_, _) => TrySubmit(true);
            }
            else if (ReferenceEquals(dialogOption, _settings.NegativeOption))
            {
                button.Click += (_, _) => TrySubmit(false);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(true);
    }
}
