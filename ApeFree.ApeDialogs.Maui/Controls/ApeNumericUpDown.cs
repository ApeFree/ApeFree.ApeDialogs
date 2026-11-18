using System;
using System.Globalization;
using ApeFree.ApeDialogs.Maui.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Controls
{
    /// <summary>
    /// 数值微调输入控件（带有加减步进按钮与范围限制）。
    /// </summary>
    public partial class ApeNumericUpDown : Grid
    {
        private decimal _value;
        private decimal _minimum = 0m;
        private decimal _maximum = 100m;
        private decimal _increment = 1m;
        private int _decimalPlaces = 0;

        private readonly Button _downButton;
        private readonly Entry _entry;
        private readonly Button _upButton;

        public ApeNumericUpDown()
        {
            HeightRequest = 42;
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(42) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(42) },
            };

            _downButton = new Button
            {
                Text = "－",
                FontSize = 16,
                Padding = 0,
                BackgroundColor = MauiThemeManager.ControlBackground,
                TextColor = MauiThemeManager.TextPrimary,
                BorderColor = MauiThemeManager.StrokeDefault,
                BorderWidth = 1,
                CornerRadius = 6,
            };
            _downButton.Clicked += (_, _) => Value -= Increment;
            Children.Add(_downButton);
            Grid.SetColumn((BindableObject)_downButton, 0);

            _entry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
                Margin = new Thickness(6, 0),
            };
            _entry.Unfocused += (_, _) => CommitText();
            _entry.Completed += (_, _) => CommitText();
            Children.Add(_entry);
            Grid.SetColumn((BindableObject)_entry, 1);

            _upButton = new Button
            {
                Text = "＋",
                FontSize = 16,
                Padding = 0,
                BackgroundColor = MauiThemeManager.ControlBackground,
                TextColor = MauiThemeManager.TextPrimary,
                BorderColor = MauiThemeManager.StrokeDefault,
                BorderWidth = 1,
                CornerRadius = 6,
            };
            _upButton.Clicked += (_, _) => Value += Increment;
            Children.Add(_upButton);
            Grid.SetColumn((BindableObject)_upButton, 2);

            UpdateDisplayText();
            UpdateButtonStates();
        }

        public event EventHandler<decimal>? ValueChanged;

        public decimal Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum) Value = _minimum;
                UpdateButtonStates();
            }
        }

        public decimal Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum) Value = _maximum;
                UpdateButtonStates();
            }
        }

        public decimal Increment
        {
            get => _increment;
            set => _increment = value;
        }

        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                _decimalPlaces = Math.Max(0, value);
                UpdateDisplayText();
            }
        }

        public decimal Value
        {
            get => _value;
            set
            {
                var clamped = Math.Max(_minimum, Math.Min(_maximum, value));
                if (_value != clamped)
                {
                    _value = clamped;
                    UpdateDisplayText();
                    UpdateButtonStates();
                    ValueChanged?.Invoke(this, _value);
                }
            }
        }

        private void CommitText()
        {
            if (decimal.TryParse(_entry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
            {
                Value = parsed;
            }
            else
            {
                UpdateDisplayText();
            }
        }

        private void UpdateDisplayText()
        {
            var format = _decimalPlaces > 0 ? $"F{_decimalPlaces}" : "0";
            _entry.Text = _value.ToString(format, CultureInfo.CurrentCulture);
        }

        private void UpdateButtonStates()
        {
            _downButton.IsEnabled = _value > _minimum;
            _upButton.IsEnabled = _value < _maximum;
        }
    }
}
