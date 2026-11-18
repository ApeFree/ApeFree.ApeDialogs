using System;
using System.Collections.Generic;
using System.Linq;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 可编辑下拉输入对话框（MAUI 实现，支持自由输入与预设候选项选取）。
    /// </summary>
    public partial class MauiComboBoxInputDialog : MauiDialogBase<string>
    {
        private readonly Entry _entry;
        private readonly Picker _picker;

        public MauiComboBoxInputDialog(ComboBoxInputDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var panel = new VerticalStackLayout { Spacing = 8 };

            _entry = new Entry
            {
                Text = settings.DefaultText ?? string.Empty,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
                Placeholder = "请输入或从下方选取预选项",
            };
            panel.Children.Add(_entry);

            _picker = new Picker
            {
                Title = "点击选择常用预设项...",
                TextColor = MauiThemeManager.TextSecondary,
                BackgroundColor = MauiThemeManager.ControlBackground,
            };

            if (settings.Items != null)
            {
                foreach (var item in settings.Items)
                {
                    _picker.Items.Add(item);
                }
            }

            _picker.SelectedIndexChanged += (_, _) =>
            {
                if (_picker.SelectedIndex >= 0 && _picker.SelectedIndex < _picker.Items.Count)
                {
                    _entry.Text = _picker.Items[_picker.SelectedIndex];
                }
            };
            panel.Children.Add(_picker);

            Host.SetDialogContent(panel);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(_entry.Text ?? string.Empty)
            };
            AddOption(confirmOption);

            if (settings.CancelOption != null)
            {
                var cancelOption = settings.CancelOption with
                {
                    Action = _ => Cancel()
                };
                AddOption(cancelOption);
            }
        }
    }
}
