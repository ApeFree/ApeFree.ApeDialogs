using System;
using System.Linq;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 可编辑下拉输入对话框（WPF 实现）。
    /// </summary>
    public class WpfComboBoxInputDialog : WpfDialogBase<string>
    {
        private readonly ComboBoxInputDialogSettings _settings;
        private readonly ApeComboBox _comboBox;

        public WpfComboBoxInputDialog(ComboBoxInputDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _comboBox = new ApeComboBox
            {
                IsEditable = settings.IsEditable,
                ItemsSource = settings.Items,
            };

            if (settings.IsEditable)
            {
                _comboBox.Text = settings.DefaultText;
            }
            else
            {
                var match = settings.Items.FirstOrDefault(item => item == settings.DefaultText);
                if (match is not null)
                {
                    _comboBox.SelectedItem = match;
                }
            }

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(_comboBox);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_comboBox.Text);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(_comboBox.Text);
    }
}
