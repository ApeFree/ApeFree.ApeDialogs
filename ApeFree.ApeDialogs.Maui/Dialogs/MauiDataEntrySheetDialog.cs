using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Maui.Controls;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 多字段表单对话框（MAUI 实现）：按字段类型动态生成跨平台编辑器，双层校验保障数据有效性。
    /// </summary>
    public partial class MauiDataEntrySheetDialog : MauiDialogBase<DataEntrySheet>
    {
        private readonly DataEntrySheet _sheet;
        private readonly List<FieldEditor> _editors = new();

        public MauiDataEntrySheetDialog(
            DataEntrySheet sheet,
            DataEntrySheetDialogSettings settings,
            IMauiDialogContext? context = default,
            IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));

            var formPanel = new VerticalStackLayout { Spacing = 14 };

            foreach (var field in sheet.Fields)
            {
                var editor = CreateEditor(field);
                _editors.Add(editor);
                formPanel.Children.Add(editor.BuildView());
            }

            var scroll = new ScrollView
            {
                Content = formPanel,
                MaximumHeightRequest = 360,
            };

            Host.SetDialogContent(scroll);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => Submit()
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

        private void Submit()
        {
            var allValid = true;

            foreach (var editor in _editors)
            {
                if (!editor.TryCommit())
                {
                    allValid = false;
                }
            }

            if (allValid)
            {
                TrySubmit(_sheet);
            }
            else
            {
                _ = Host.PlayShakeAnimationAsync();
            }
        }

        private static FieldEditor CreateEditor(SheetField field)
        {
            return field.FieldType switch
            {
                FieldType.Text => new TextFieldEditor((TextField)field),
                FieldType.Password => new PasswordFieldEditor((PasswordField)field),
                FieldType.HexBytes => new HexBytesFieldEditor((HexBytesField)field),
                FieldType.FilePath => new PathFieldEditor((FilePathField)field, isPicture: false),
                FieldType.PicturePath => new PathFieldEditor((PicturePathField)field, isPicture: true),
                FieldType.Number => new NumberFieldEditor((NumberField)field),
                FieldType.SingleChoice => new SingleChoiceFieldEditor((SingleChoiceField)field),
                FieldType.ComboBox => new ComboBoxFieldEditor((ComboBoxField)field),
                FieldType.MultipleChoice => new MultipleChoiceFieldEditor((MultipleChoiceField)field),
                FieldType.DateTime => new DateTimeFieldEditor((DateTimeField)field),
                _ => throw new NotSupportedException($"Unsupported field type: {field.FieldType}"),
            };
        }

        #region Editors

        private abstract class FieldEditor
        {
            protected readonly Label ErrorLabel;

            protected FieldEditor(string title)
            {
                Title = title;
                ErrorLabel = new Label
                {
                    TextColor = MauiThemeManager.NegativeColor,
                    FontSize = 11,
                    IsVisible = false,
                };
            }

            public string Title { get; }

            public View BuildView()
            {
                var stack = new VerticalStackLayout { Spacing = 4 };

                var titleLabel = new Label
                {
                    Text = Title,
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = MauiThemeManager.TextPrimary,
                };
                stack.Children.Add(titleLabel);

                var editorView = CreateEditorView();
                stack.Children.Add(editorView);

                stack.Children.Add(ErrorLabel);

                return stack;
            }

            protected abstract View CreateEditorView();

            public abstract bool TryCommit();

            protected void SetError(string? message)
            {
                if (string.IsNullOrEmpty(message))
                {
                    ErrorLabel.IsVisible = false;
                    ErrorLabel.Text = string.Empty;
                }
                else
                {
                    ErrorLabel.Text = message;
                    ErrorLabel.IsVisible = true;
                }
            }
        }

        private sealed class TextFieldEditor : FieldEditor
        {
            private readonly TextField _field;
            private readonly InputView _input;

            public TextFieldEditor(TextField field) : base(field.Title)
            {
                _field = field;
                if (field.IsMultiline)
                {
                    _input = new Editor
                    {
                        Text = field.Value ?? string.Empty,
                        HeightRequest = 70,
                        MaxLength = field.MaximumLength,
                        TextColor = MauiThemeManager.TextPrimary,
                        BackgroundColor = MauiThemeManager.ControlBackground,
                    };
                }
                else
                {
                    _input = new Entry
                    {
                        Text = field.Value ?? string.Empty,
                        MaxLength = field.MaximumLength,
                        TextColor = MauiThemeManager.TextPrimary,
                        BackgroundColor = MauiThemeManager.ControlBackground,
                    };
                }
            }

            protected override View CreateEditorView() => _input;

            public override bool TryCommit()
            {
                _field.Value = _input.Text;
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class PasswordFieldEditor : FieldEditor
        {
            private readonly PasswordField _field;
            private readonly Entry _entry;

            public PasswordFieldEditor(PasswordField field) : base(field.Title)
            {
                _field = field;
                _entry = new Entry
                {
                    Text = field.Value ?? string.Empty,
                    IsPassword = true,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };
            }

            protected override View CreateEditorView() => _entry;

            public override bool TryCommit()
            {
                _field.Value = _entry.Text;
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class HexBytesFieldEditor : FieldEditor
        {
            private readonly HexBytesField _field;
            private readonly Entry _entry;

            public HexBytesFieldEditor(HexBytesField field) : base(field.Title)
            {
                _field = field;
                _entry = new Entry
                {
                    Text = field.Value != null ? BitConverter.ToString(field.Value).Replace("-", " ") : string.Empty,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                    Placeholder = "例: 0A 1B 2C",
                };
            }

            protected override View CreateEditorView() => _entry;

            public override bool TryCommit()
            {
                var text = _entry.Text?.Replace(" ", "").Replace("-", "") ?? string.Empty;
                if (text.Length % 2 != 0)
                {
                    SetError("十六进制字符数必须为偶数");
                    return false;
                }

                try
                {
                    var bytes = new byte[text.Length / 2];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
                    }
                    _field.Value = bytes;
                }
                catch
                {
                    SetError("无法解析为十六进制数据");
                    return false;
                }

                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class PathFieldEditor : FieldEditor
        {
            private readonly SheetField<string> _field;
            private readonly Entry _entry;
            private readonly bool _isPicture;

            public PathFieldEditor(SheetField<string> field, bool isPicture) : base(field.Title)
            {
                _field = field;
                _isPicture = isPicture;
                _entry = new Entry
                {
                    Text = field.Value ?? string.Empty,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };
            }

            protected override View CreateEditorView()
            {
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto },
                    },
                    ColumnSpacing = 6,
                };

                grid.Children.Add(_entry);
                Grid.SetColumn(_entry, 0);

                var browseBtn = new Button
                {
                    Text = "浏览...",
                    HeightRequest = 40,
                    Padding = new Thickness(12, 0),
                    BackgroundColor = MauiThemeManager.ControlBackground,
                    TextColor = MauiThemeManager.TextPrimary,
                    BorderColor = MauiThemeManager.StrokeDefault,
                    BorderWidth = 1,
                    CornerRadius = 6,
                };
                browseBtn.Clicked += async (_, _) =>
                {
                    try
                    {
                        var options = _isPicture ? PickOptions.Images : PickOptions.Default;
                        var result = await FilePicker.Default.PickAsync(options);
                        if (result != null)
                        {
                            _entry.Text = result.FullPath;
                        }
                    }
                    catch
                    {
                        // 忽略权限被取消
                    }
                };
                grid.Children.Add(browseBtn);
                Grid.SetColumn(browseBtn, 1);

                return grid;
            }

            public override bool TryCommit()
            {
                _field.Value = _entry.Text;
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class NumberFieldEditor : FieldEditor
        {
            private readonly NumberField _field;
            private readonly ApeNumericUpDown _numericUpDown;

            public NumberFieldEditor(NumberField field) : base(field.Title)
            {
                _field = field;
                _numericUpDown = new ApeNumericUpDown
                {
                    Minimum = field.Minimum,
                    Maximum = field.Maximum,
                    Increment = 1m,
                    DecimalPlaces = field.DecimalPlaces,
                    Value = field.Value,
                };
            }

            protected override View CreateEditorView() => _numericUpDown;

            public override bool TryCommit()
            {
                _field.Value = _numericUpDown.Value;
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class SingleChoiceFieldEditor : FieldEditor
        {
            private readonly SingleChoiceField _field;
            private readonly Picker _picker;

            public SingleChoiceFieldEditor(SingleChoiceField field) : base(field.Title)
            {
                _field = field;
                _picker = new Picker
                {
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };

                if (field.Items != null)
                {
                    foreach (var opt in field.Items)
                    {
                        _picker.Items.Add(field.ItemDisplayConverter(opt));
                    }
                    if (field.Value != null)
                    {
                        var idx = field.Items.ToList().IndexOf(field.Value);
                        if (idx >= 0) _picker.SelectedIndex = idx;
                    }
                }
            }

            protected override View CreateEditorView() => _picker;

            public override bool TryCommit()
            {
                if (_picker.SelectedIndex >= 0 && _field.Items != null && _picker.SelectedIndex < _field.Items.Count)
                {
                    _field.Value = _field.Items[_picker.SelectedIndex];
                }

                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class ComboBoxFieldEditor : FieldEditor
        {
            private readonly ComboBoxField _field;
            private readonly Entry _entry;
            private readonly Picker _picker;

            public ComboBoxFieldEditor(ComboBoxField field) : base(field.Title)
            {
                _field = field;
                _entry = new Entry
                {
                    Text = field.Value?.ToString() ?? string.Empty,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };

                _picker = new Picker
                {
                    Title = "选择候选项...",
                    TextColor = MauiThemeManager.TextSecondary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };

                if (field.Items != null)
                {
                    foreach (var opt in field.Items)
                    {
                        _picker.Items.Add(field.ItemDisplayConverter(opt));
                    }
                }

                _picker.SelectedIndexChanged += (_, _) =>
                {
                    if (_picker.SelectedIndex >= 0 && _field.Items != null && _picker.SelectedIndex < _field.Items.Count)
                    {
                        _entry.Text = _field.ItemDisplayConverter(_field.Items[_picker.SelectedIndex]);
                    }
                };
            }

            protected override View CreateEditorView()
            {
                var stack = new VerticalStackLayout { Spacing = 4 };
                stack.Children.Add(_entry);
                stack.Children.Add(_picker);
                return stack;
            }

            public override bool TryCommit()
            {
                _field.Value = _entry.Text;
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class MultipleChoiceFieldEditor : FieldEditor
        {
            private readonly MultipleChoiceField _field;
            private readonly List<CheckBox> _checkBoxes = new();

            public MultipleChoiceFieldEditor(MultipleChoiceField field) : base(field.Title)
            {
                _field = field;
            }

            protected override View CreateEditorView()
            {
                var stack = new VerticalStackLayout { Spacing = 6 };
                var currentSelection = new HashSet<object?>(_field.Value ?? Enumerable.Empty<object?>());

                if (_field.Items != null)
                {
                    foreach (var opt in _field.Items)
                    {
                        var row = new HorizontalStackLayout { Spacing = 8 };
                        var cb = new CheckBox
                        {
                            IsChecked = currentSelection.Contains(opt),
                            Color = MauiThemeManager.AccentColor,
                        };
                        _checkBoxes.Add(cb);
                        row.Children.Add(cb);
                        row.Children.Add(new Label
                        {
                            Text = _field.ItemDisplayConverter(opt),
                            VerticalOptions = LayoutOptions.Center,
                            TextColor = MauiThemeManager.TextPrimary,
                        });
                        stack.Children.Add(row);
                    }
                }

                return stack;
            }

            public override bool TryCommit()
            {
                var selected = new List<object?>();
                if (_field.Items != null)
                {
                    for (int i = 0; i < _checkBoxes.Count && i < _field.Items.Count; i++)
                    {
                        if (_checkBoxes[i].IsChecked)
                        {
                            selected.Add(_field.Items[i]);
                        }
                    }
                }

                _field.Value = selected;
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        private sealed class DateTimeFieldEditor : FieldEditor
        {
            private readonly DateTimeField _field;
            private readonly DatePicker _datePicker;
            private readonly TimePicker _timePicker;

            public DateTimeFieldEditor(DateTimeField field) : base(field.Title)
            {
                _field = field;
                var initial = field.Value;
                _datePicker = new DatePicker
                {
                    Date = initial.Date,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };
                _timePicker = new TimePicker
                {
                    Time = initial.TimeOfDay,
                    TextColor = MauiThemeManager.TextPrimary,
                    BackgroundColor = MauiThemeManager.ControlBackground,
                };
            }

            protected override View CreateEditorView()
            {
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star },
                    },
                    ColumnSpacing = 8,
                };
                grid.Children.Add(_datePicker);
                Grid.SetColumn(_datePicker, 0);
                grid.Children.Add(_timePicker);
                Grid.SetColumn(_timePicker, 1);
                return grid;
            }

            public override bool TryCommit()
            {
                _field.Value = _datePicker.Date.Add(_timePicker.Time);
                var res = _field.Validate();
                if (!res.IsSuccess)
                {
                    SetError(res.ErrorMessage);
                    return false;
                }
                SetError(null);
                return true;
            }
        }

        #endregion
    }
}
