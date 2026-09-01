using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 多字段表单对话框（WPF 实现）：按字段类型动态生成编辑器，提交时逐字段校验。
    /// </summary>
    public class WpfDataEntrySheetDialog : WpfDialogBase<DataEntrySheet>
    {
        private readonly DataEntrySheetDialogSettings _settings;
        private readonly DataEntrySheet _sheet;
        private readonly List<FieldEditor> _editors = new List<FieldEditor>();

        public WpfDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;
            _sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));

            Host.Width = 520;
            Host.Height = 480;

            var form = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };

            foreach (var field in sheet.Fields)
            {
                var editor = CreateEditor(field);
                _editors.Add(editor);
                form.Children.Add(editor.BuildRow());
            }

            var scroll = new ScrollViewer
            {
                Content = form,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(ContentTextBlock, 0);
            Grid.SetRow(scroll, 1);
            grid.Children.Add(ContentTextBlock);
            grid.Children.Add(scroll);
            Host.SetDialogContent(grid);

            AddDefaultOptions();
        }

        private static FieldEditor CreateEditor(SheetField field)
        {
            switch (field.FieldType)
            {
                case FieldType.Text:
                    return new TextFieldEditor((TextField)field);
                case FieldType.Password:
                    return new PasswordFieldEditor((PasswordField)field);
                case FieldType.HexBytes:
                    return new HexBytesFieldEditor((HexBytesField)field);
                case FieldType.FilePath:
                    return new FilePathFieldEditor((FilePathField)field, isPicture: false);
                case FieldType.PicturePath:
                    return new FilePathFieldEditor((PicturePathField)field, isPicture: true);
                case FieldType.Number:
                    return new NumberFieldEditor((NumberField)field);
                case FieldType.SingleChoice:
                    return new SingleChoiceFieldEditor((SingleChoiceField)field);
                case FieldType.ComboBox:
                    return new ComboBoxFieldEditor((ComboBoxField)field);
                case FieldType.MultipleChoice:
                    return new MultipleChoiceFieldEditor((MultipleChoiceField)field);
                case FieldType.DateTime:
                    return new DateTimeFieldEditor((DateTimeField)field);
                default:
                    throw new NotSupportedException($"Unsupported field type: {field.FieldType}");
            }
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => Submit();
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => Submit();

        private void Submit()
        {
            // 第一层：UI 解析提交（写回字段值）
            foreach (var editor in _editors)
            {
                if (!editor.Commit())
                {
                    Host.PlayShakeAnimation();
                    return;
                }
            }

            // 第二层：字段校验器
            foreach (var editor in _editors)
            {
                var result = editor.Field.Validate();
                if (!result.IsSuccess)
                {
                    editor.ShowError(result.ErrorMessage);
                    Host.PlayShakeAnimation();
                    return;
                }

                editor.ShowError(null);
            }

            TrySubmit(_sheet);
        }

        // ==================== 字段编辑器 ====================

        private abstract class FieldEditor
        {
            protected FieldEditor(SheetField field)
            {
                Field = field;
            }

            public SheetField Field { get; }

            protected abstract UIElement CreateControl();

            private readonly TextBlock _errorText = CreateErrorText();

            public StackPanel BuildRow()
            {
                var row = new StackPanel { Margin = new Thickness(0, 0, 0, 12) };

                var title = new TextBlock
                {
                    Text = Field.Title,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 0, 4),
                };
                title.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillPrimary");

                row.Children.Add(title);
                row.Children.Add(CreateControl());
                row.Children.Add(_errorText);
                return row;
            }
            public void ShowError(string? message)
            {
                _errorText.Text = message ?? string.Empty;
                _errorText.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            }

            /// <summary>
            /// 将 UI 值写回字段；返回 false 表示解析失败（已显示错误）。
            /// </summary>
            public abstract bool Commit();

            private static TextBlock CreateErrorText()
            {
                var text = new TextBlock
                {
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 3, 0, 0),
                    Visibility = Visibility.Collapsed,
                };
                text.SetResourceReference(TextBlock.ForegroundProperty, "ApeNegativeBrush");
                return text;
            }
        }

        private sealed class TextFieldEditor : FieldEditor
        {
            private readonly TextField _field;
            private readonly ApeTextBox _textBox;

            public TextFieldEditor(TextField field) : base(field)
            {
                _field = field;
                _textBox = new ApeTextBox
                {
                    Text = field.Value ?? string.Empty,
                    MaxLength = field.MaximumLength,
                    AcceptsReturn = field.IsMultiline,
                    TextWrapping = field.IsMultiline ? TextWrapping.Wrap : TextWrapping.NoWrap,
                    Height = field.IsMultiline ? 80 : 32,
                    VerticalContentAlignment = field.IsMultiline ? VerticalAlignment.Top : VerticalAlignment.Center,
                };
            }

            protected override UIElement CreateControl() => _textBox;

            public override bool Commit()
            {
                _field.Value = _textBox.Text;
                return true;
            }
        }

        private sealed class PasswordFieldEditor : FieldEditor
        {
            private readonly PasswordField _field;
            private readonly ApePasswordBox _passwordBox;

            public PasswordFieldEditor(PasswordField field) : base(field)
            {
                _field = field;
                _passwordBox = new ApePasswordBox { Password = field.Value ?? string.Empty };
            }

            protected override UIElement CreateControl() => _passwordBox;

            public override bool Commit()
            {
                _field.Value = _passwordBox.Password;
                return true;
            }
        }

        private sealed class HexBytesFieldEditor : FieldEditor
        {
            private readonly HexBytesField _field;
            private readonly ApeTextBox _textBox;

            public HexBytesFieldEditor(HexBytesField field) : base(field)
            {
                _field = field;
                _textBox = new ApeTextBox
                {
                    Text = field.Value is null ? string.Empty : BitConverter.ToString(field.Value).Replace("-", string.Empty),
                    MaxLength = Math.Min(200000, field.MaximumLength * 2),
                    AcceptsReturn = field.IsMultiline,
                    TextWrapping = field.IsMultiline ? TextWrapping.Wrap : TextWrapping.NoWrap,
                    Height = field.IsMultiline ? 80 : 32,
                    VerticalContentAlignment = field.IsMultiline ? VerticalAlignment.Top : VerticalAlignment.Center,
                };
            }

            protected override UIElement CreateControl() => _textBox;

            public override bool Commit()
            {
                var text = _textBox.Text.Replace(" ", string.Empty).Replace("-", string.Empty);

                if (string.IsNullOrEmpty(text))
                {
                    if (_field.AllowEmpty)
                    {
                        _field.Value = null;
                        return true;
                    }

                    ShowError("该字段不允许为空");
                    return false;
                }

                if (text.Length % 2 != 0)
                {
                    ShowError("十六进制长度必须为偶数");
                    return false;
                }

                try
                {
                    _field.Value = Enumerable.Range(0, text.Length / 2)
                        .Select(i => Convert.ToByte(text.Substring(i * 2, 2), 16))
                        .ToArray();
                    return true;
                }
                catch (FormatException)
                {
                    ShowError("包含无效的十六进制字符");
                    return false;
                }
            }
        }

        private sealed class FilePathFieldEditor : FieldEditor
        {
            private readonly FilePathField _field;
            private readonly ApeTextBox _textBox;

            public FilePathFieldEditor(FilePathField field, bool isPicture) : base(field)
            {
                _field = field;
                _textBox = new ApeTextBox
                {
                    Text = field.Value ?? string.Empty,
                    IsReadOnly = false,
                };

                var browseButton = new ApeButton
                {
                    Content = field.BrowseButtonText,
                    MinWidth = 0,
                    Padding = new Thickness(10, 0, 10, 0),
                    Margin = new Thickness(8, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                browseButton.Click += (_, _) =>
                {
                    var dialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Title = _field.Title,
                        Filter = isPicture ? "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp" : "All files|*.*",
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        _textBox.Text = dialog.FileName;
                    }
                };

                _grid = new Grid();
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                Grid.SetColumn(_textBox, 0);
                Grid.SetColumn(browseButton, 1);
                _grid.Children.Add(_textBox);
                _grid.Children.Add(browseButton);
            }

            private readonly Grid _grid;

            protected override UIElement CreateControl() => _grid;

            public override bool Commit()
            {
                _field.Value = _textBox.Text;
                return true;
            }
        }

        private sealed class NumberFieldEditor : FieldEditor
        {
            private readonly NumberField _field;
            private readonly ApeNumericUpDown _numeric;

            public NumberFieldEditor(NumberField field) : base(field)
            {
                _field = field;
                _numeric = new ApeNumericUpDown
                {
                    Minimum = field.Minimum,
                    Maximum = field.Maximum,
                    DecimalPlaces = field.DecimalPlaces,
                    Value = field.Value,
                };
            }

            protected override UIElement CreateControl() => _numeric;

            public override bool Commit()
            {
                _numeric.Commit();
                _field.Value = _numeric.Value;
                return true;
            }
        }

        private sealed class SingleChoiceFieldEditor : FieldEditor
        {
            private readonly SingleChoiceField _field;
            private readonly List<ApeRadioButton> _radios = new List<ApeRadioButton>();

            public SingleChoiceFieldEditor(SingleChoiceField field) : base(field)
            {
                _field = field;

                foreach (var item in field.Items)
                {
                    var radio = new ApeRadioButton
                    {
                        Content = field.ItemDisplayConverter(item),
                        IsChecked = Equals(item, field.Value),
                        Margin = new Thickness(0, 2, 0, 2),
                    };
                    _radios.Add(radio);
                }
            }

            protected override UIElement CreateControl()
            {
                var panel = new StackPanel();
                foreach (var radio in _radios)
                {
                    panel.Children.Add(radio);
                }

                return panel;
            }

            public override bool Commit()
            {
                var index = _radios.FindIndex(r => r.IsChecked == true);
                if (index >= 0 && index < _field.Items.Count)
                {
                    _field.Value = _field.Items[index];
                }

                return true;
            }
        }

        private sealed class ComboBoxFieldEditor : FieldEditor
        {
            private readonly SingleChoiceField _field;
            private readonly ApeComboBox _comboBox;

            public ComboBoxFieldEditor(SingleChoiceField field) : base(field)
            {
                _field = field;
                _comboBox = new ApeComboBox
                {
                    ItemsSource = field.Items.Select(field.ItemDisplayConverter).ToList(),
                };

                var index = field.Items.ToList().FindIndex(x => Equals(x, field.Value));
                if (index >= 0)
                {
                    _comboBox.SelectedIndex = index;
                }
            }

            protected override UIElement CreateControl() => _comboBox;

            public override bool Commit()
            {
                var index = _comboBox.SelectedIndex;
                if (index >= 0 && index < _field.Items.Count)
                {
                    _field.Value = _field.Items[index];
                }

                return true;
            }
        }

        private sealed class MultipleChoiceFieldEditor : FieldEditor
        {
            private readonly MultipleChoiceField _field;
            private readonly List<(ApeCheckBox CheckBox, object? Item)> _checks = new List<(ApeCheckBox, object?)>();

            public MultipleChoiceFieldEditor(MultipleChoiceField field) : base(field)
            {
                _field = field;

                var selected = field.Value?.ToList() ?? new List<object?>();

                foreach (var item in field.Items)
                {
                    var check = new ApeCheckBox
                    {
                        Content = field.ItemDisplayConverter(item),
                        IsChecked = selected.Any(x => Equals(x, item)),
                        Margin = new Thickness(0, 2, 0, 2),
                    };
                    _checks.Add((check, item));
                }
            }

            protected override UIElement CreateControl()
            {
                var panel = new StackPanel();
                foreach (var (check, _) in _checks)
                {
                    panel.Children.Add(check);
                }

                return panel;
            }

            public override bool Commit()
            {
                _field.Value = _checks.Where(c => c.CheckBox.IsChecked == true).Select(c => c.Item).ToList();
                return true;
            }
        }

        private sealed class DateTimeFieldEditor : FieldEditor
        {
            private readonly DateTimeField _field;
            private readonly ApeTextBox _textBox;

            public DateTimeFieldEditor(DateTimeField field) : base(field)
            {
                _field = field;
                _textBox = new ApeTextBox
                {
                    Text = field.Value.ToString(field.DateTimeFormat, CultureInfo.CurrentCulture),
                    PlaceholderText = field.DateTimeFormat,
                };
            }

            protected override UIElement CreateControl() => _textBox;

            public override bool Commit()
            {
                if (DateTime.TryParseExact(_textBox.Text, _field.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    _field.Value = parsed;
                    return true;
                }

                ShowError($"日期格式应为 {_field.DateTimeFormat}");
                return false;
            }
        }
    }
}
