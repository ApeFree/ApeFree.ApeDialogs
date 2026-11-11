using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// 数值输入控件（类似 WinForms NumericUpDown）：文本框 + 上/下步进按钮。
    /// 支持范围限制、步进值、小数位。
    /// </summary>
    public class ApeNumericUpDown : Control
    {
        private TextBox? _textBox;
        private Button? _upButton;
        private Button? _downButton;
        private bool _suppressTextUpdate;
        private bool _isUserTyping;

        static ApeNumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeNumericUpDown), new FrameworkPropertyMetadata(typeof(ApeNumericUpDown)));
        }

        /// <summary>
        /// 当前值（双向）。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(ApeNumericUpDown),
                new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        /// <summary>
        /// 当前值（双向）。
        /// </summary>
        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// 最小值。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(decimal), typeof(ApeNumericUpDown), new PropertyMetadata(0m, OnRangeChanged));

        /// <summary>
        /// 最小值。
        /// </summary>
        public decimal Minimum
        {
            get => (decimal)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// 最大值。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(decimal), typeof(ApeNumericUpDown), new PropertyMetadata(100m, OnRangeChanged));

        /// <summary>
        /// 最大值。
        /// </summary>
        public decimal Maximum
        {
            get => (decimal)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// 步进值。
        /// </summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(decimal), typeof(ApeNumericUpDown), new PropertyMetadata(1m));

        /// <summary>
        /// 步进值。
        /// </summary>
        public decimal Increment
        {
            get => (decimal)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        /// <summary>
        /// 保留小数的位数（显示与格式化）。
        /// </summary>
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(nameof(DecimalPlaces), typeof(int), typeof(ApeNumericUpDown), new PropertyMetadata(0, OnRangeChanged));

        /// <summary>
        /// 保留小数的位数（显示与格式化）。
        /// </summary>
        public int DecimalPlaces
        {
            get => (int)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        /// <summary>
        /// 值变化事件（程序化设置与用户操作均触发）。
        /// </summary>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_textBox is not null)
            {
                _textBox.TextChanged -= OnTextChanged;
                _textBox.LostFocus -= OnTextBoxLostFocus;
            }

            if (_upButton is not null)
            {
                _upButton.Click -= OnUpClick;
            }

            if (_downButton is not null)
            {
                _downButton.Click -= OnDownClick;
            }

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _upButton = GetTemplateChild("PART_UpButton") as Button;
            _downButton = GetTemplateChild("PART_DownButton") as Button;

            if (_textBox is not null)
            {
                _textBox.TextChanged += OnTextChanged;
                _textBox.LostFocus += OnTextBoxLostFocus;
                _textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;
            }

            if (_upButton is not null)
            {
                _upButton.Click += OnUpClick;
            }

            if (_downButton is not null)
            {
                _downButton.Click += OnDownClick;
            }

            UpdateText();
            UpdateButtonStates();
        }

        private string? _unappliedText;

        /// <summary>
        /// 将当前输入文本提交为值（输入不完整/非法时回退到当前值）。
        /// </summary>
        public void Commit()
        {
            var text = _textBox?.Text ?? _unappliedText;
            if (text is not null && decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
            {
                SetCurrentValue(ValueProperty, Clamp(parsed));
                _unappliedText = null;
            }

            UpdateText();
        }

        /// <summary>
        /// 当前文本框输入文本。
        /// </summary>
        public string? Text
        {
            get => _textBox?.Text ?? _unappliedText ?? FormatValue(Value);
            set
            {
                if (_textBox is not null)
                {
                    _textBox.Text = value;
                }
                else
                {
                    _unappliedText = value;
                }
            }
        }

        private static decimal Clamp(decimal value, decimal minimum, decimal maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }

            if (value > maximum)
            {
                return maximum;
            }

            return value;
        }

        private decimal Clamp(decimal value) => Clamp(value, Minimum, Maximum);

        private string FormatValue(decimal value) => value.ToString("F" + Math.Max(0, DecimalPlaces), CultureInfo.CurrentCulture);

        private void UpdateText()
        {
            _unappliedText = null;
            if (_textBox is null)
            {
                return;
            }

            _suppressTextUpdate = true;
            _textBox.Text = FormatValue(Value);
            _textBox.CaretIndex = _textBox.Text.Length;
            _suppressTextUpdate = false;
        }

        private void UpdateButtonStates()
        {
            if (_upButton is not null)
            {
                _upButton.IsEnabled = Value < Maximum;
            }

            if (_downButton is not null)
            {
                _downButton.IsEnabled = Value > Minimum;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextUpdate || _textBox is null)
            {
                return;
            }

            _isUserTyping = true;
            try
            {
                if (decimal.TryParse(_textBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
                {
                    if (parsed >= Minimum && parsed <= Maximum && parsed != Value)
                    {
                        SetCurrentValue(ValueProperty, parsed);
                    }
                }
            }
            finally
            {
                _isUserTyping = false;
            }
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Commit();
                e.Handled = true;
            }
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e) => Commit();

        private void OnUpClick(object sender, RoutedEventArgs e) => SetCurrentValue(ValueProperty, Clamp(Value + Increment));

        private void OnDownClick(object sender, RoutedEventArgs e) => SetCurrentValue(ValueProperty, Clamp(Value - Increment));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ApeNumericUpDown)d;
            if (!control._isUserTyping)
            {
                control.UpdateText();
            }
            control.UpdateButtonStates();
            control.ValueChanged?.Invoke(control, EventArgs.Empty);
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ApeNumericUpDown)d;
            control.SetCurrentValue(ValueProperty, control.Clamp(control.Value));
        }
    }
}
