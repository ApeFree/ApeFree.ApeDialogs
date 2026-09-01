using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 风格颜色选择器：SV 面板 + 色相条 + RGB/Hex 输入 + Alpha 滑块 + 预设色块，多入口双向同步。
    /// </summary>
    public partial class ApeColorPicker : UserControl
    {
        private bool _updating;

        public ApeColorPicker()
        {
            InitializeComponent();

            SVPanel.ColorAreaChanged += (_, _) => ComposeFromSaturationValue(fromHueBar: false);
            HueBar.HueChanged += (_, _) => ComposeFromSaturationValue(fromHueBar: true);

            AlphaSlider.ValueChanged += (_, _) => ComposeFromSaturationValue();
            RedBox.ValueChanged += (_, _) => ComposeFromRgb();
            GreenBox.ValueChanged += (_, _) => ComposeFromRgb();
            BlueBox.ValueChanged += (_, _) => ComposeFromRgb();

            HexBox.KeyDown += OnHexKeyDown;
            HexBox.LostFocus += OnHexLostFocus;

            SyncUiFromColor(SelectedColor);
            UpdatePresets();
        }

        /// <summary>
        /// 当前选中的颜色（双向）。
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(ApeColor), typeof(ApeColorPicker),
                new FrameworkPropertyMetadata(new ApeColor(255, 255, 255), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));

        /// <summary>
        /// 当前选中的颜色（双向）。
        /// </summary>
        public ApeColor SelectedColor
        {
            get => (ApeColor)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// 是否允许调节 Alpha 通道。
        /// </summary>
        public static readonly DependencyProperty AllowAlphaProperty =
            DependencyProperty.Register(nameof(AllowAlpha), typeof(bool), typeof(ApeColorPicker), new PropertyMetadata(false, OnAllowAlphaChanged));

        /// <summary>
        /// 是否允许调节 Alpha 通道。
        /// </summary>
        public bool AllowAlpha
        {
            get => (bool)GetValue(AllowAlphaProperty);
            set => SetValue(AllowAlphaProperty, value);
        }

        /// <summary>
        /// 预设颜色。
        /// </summary>
        public static readonly DependencyProperty PresetColorsProperty =
            DependencyProperty.Register(nameof(PresetColors), typeof(IEnumerable<ApeColor>), typeof(ApeColorPicker), new PropertyMetadata(null, OnPresetColorsChanged));

        /// <summary>
        /// 预设颜色。
        /// </summary>
        public IEnumerable<ApeColor>? PresetColors
        {
            get => (IEnumerable<ApeColor>?)GetValue(PresetColorsProperty);
            set => SetValue(PresetColorsProperty, value);
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (ApeColorPicker)d;
            if (!picker._updating)
            {
                picker.ApplyColor((ApeColor)e.NewValue);
            }
        }

        private static void OnAllowAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (ApeColorPicker)d;
            picker.AlphaRow.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnPresetColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ApeColorPicker)d).UpdatePresets();
        }

        private void UpdatePresets()
        {
            PresetsPanel.Children.Clear();

            var presets = PresetColors;
            if (presets is null)
            {
                return;
            }

            foreach (var preset in presets)
            {
                var border = new Border
                {
                    Width = 24,
                    Height = 24,
                    Margin = new Thickness(0, 0, 8, 0),
                    Background = new SolidColorBrush(Color.FromArgb(preset.A, preset.R, preset.G, preset.B)),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0xE5, 0xE5, 0xE5)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Cursor = Cursors.Hand,
                    ToolTip = string.Format(CultureInfo.InvariantCulture, "#{0:X8}", preset.ToArgb()),
                };

                var color = preset;
                border.MouseLeftButtonDown += (_, _) => ApplyColor(color);

                PresetsPanel.Children.Add(border);
            }
        }

        private void ApplyColor(ApeColor color, bool preserveHue = false)
        {
            if (_updating)
            {
                return;
            }

            try
            {
                _updating = true;
                SetCurrentValue(SelectedColorProperty, color);
                SyncUiFromColor(color, preserveHue);
            }
            finally
            {
                _updating = false;
            }
        }

        private void SyncUiFromColor(ApeColor color, bool preserveHue = false)
        {
            ColorHelper.RgbToHsv(color.R, color.G, color.B, out var hue, out var saturation, out var value);

            if (preserveHue || saturation == 0)
            {
                hue = HueBar.Hue;
            }

            SVPanel.Hue = hue;
            SVPanel.Saturation = saturation;
            SVPanel.Value = value;
            HueBar.Hue = hue;

            RedBox.Value = color.R;
            GreenBox.Value = color.G;
            BlueBox.Value = color.B;
            AlphaSlider.Value = color.A;

            var alpha = AllowAlpha ? color.A : (byte)255;
            var hexValue = color.ToArgb() & (alpha < 255 ? unchecked((int)0xFFFFFFFF) : 0xFFFFFF);
            HexBox.Text = string.Format(CultureInfo.InvariantCulture, alpha < 255 ? "#{0:X8}" : "#{0:X6}", hexValue);

            PreviewSwatch.Background = new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
        }

        private void ComposeFromSaturationValue(bool fromHueBar = false)
        {
            if (_updating)
            {
                return;
            }

            if (fromHueBar)
            {
                SVPanel.Hue = HueBar.Hue;
            }

            var rgb = ColorHelper.HsvToRgb(HueBar.Hue, SVPanel.Saturation, SVPanel.Value);
            var alpha = AllowAlpha ? (byte)Math.Round(AlphaSlider.Value) : (byte)255;
            ApplyColor(new ApeColor(alpha, rgb.R, rgb.G, rgb.B), preserveHue: true);
        }

        private void ComposeFromRgb()
        {
            if (_updating)
            {
                return;
            }

            var alpha = AllowAlpha ? (byte)Math.Round(AlphaSlider.Value) : (byte)255;
            ApplyColor(new ApeColor(alpha, (byte)RedBox.Value, (byte)GreenBox.Value, (byte)BlueBox.Value));
        }

        private void OnHexKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryParseHex();
                e.Handled = true;
            }
        }

        private void OnHexLostFocus(object sender, RoutedEventArgs e) => TryParseHex();

        private void TryParseHex()
        {
            var text = (HexBox.Text ?? string.Empty).Trim().TrimStart('#');
            if (text.Length != 6 && text.Length != 8)
            {
                SyncUiFromColor(SelectedColor);
                return;
            }

            if (int.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var argb))
            {
                var color = ApeColor.FromArgb(text.Length == 8 ? argb : argb | unchecked((int)0xFF000000));
                if (!AllowAlpha && text.Length == 8)
                {
                    color = new ApeColor(255, color.R, color.G, color.B);
                }

                ApplyColor(color);
            }
            else
            {
                SyncUiFromColor(SelectedColor);
            }
        }
    }
}
