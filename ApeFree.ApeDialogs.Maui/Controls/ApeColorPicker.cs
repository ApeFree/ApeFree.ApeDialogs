using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Controls
{
    /// <summary>
    /// 跨平台自绘调色板控件（SV 面板 + 色相条 + Hex 联动 + 预设色）。
    /// </summary>
    public partial class ApeColorPicker : VerticalStackLayout
    {
        private float _hue = 0f;
        private float _saturation = 1f;
        private float _value = 1f;
        private byte _alpha = 255;
        private bool _isInternalUpdating;

        private readonly GraphicsView _svPanel;
        private readonly GraphicsView _hueBar;
        private readonly Slider _alphaSlider;
        private readonly Border _previewBorder;
        private readonly Entry _hexEntry;
        private readonly HorizontalStackLayout _presetLayout;

        public ApeColorPicker()
        {
            Spacing = 10;

            // 1. SV 面板
            _svPanel = new GraphicsView
            {
                HeightRequest = 140,
                Drawable = new SvDrawable(this),
            };
            var svPan = new PanGestureRecognizer();
            svPan.PanUpdated += (s, e) =>
            {
                if (e.StatusType == GestureStatus.Running || e.StatusType == GestureStatus.Started)
                {
                    // 在 MAUI 中，PanGesture 提供了相对位移
                }
            };
            var svTap = new TapGestureRecognizer();
            svTap.Tapped += (s, e) =>
            {
                var pos = e.GetPosition(_svPanel);
                if (pos != null && _svPanel.Width > 0 && _svPanel.Height > 0)
                {
                    _saturation = (float)Math.Max(0, Math.Min(1, pos.Value.X / _svPanel.Width));
                    _value = (float)Math.Max(0, Math.Min(1, 1 - (pos.Value.Y / _svPanel.Height)));
                    OnColorChanged(fromHex: false);
                }
            };
            _svPanel.GestureRecognizers.Add(svTap);
            Children.Add(_svPanel);

            // 2. 色相条 (Hue Bar)
            _hueBar = new GraphicsView
            {
                HeightRequest = 22,
                Drawable = new HueDrawable(this),
            };
            var hueTap = new TapGestureRecognizer();
            hueTap.Tapped += (s, e) =>
            {
                var pos = e.GetPosition(_hueBar);
                if (pos != null && _hueBar.Width > 0)
                {
                    var ratio = Math.Max(0, Math.Min(1, pos.Value.X / _hueBar.Width));
                    _hue = (float)(ratio * 360f);
                    OnColorChanged(fromHex: false);
                }
            };
            _hueBar.GestureRecognizers.Add(hueTap);
            Children.Add(_hueBar);

            // 3. Alpha 滑块
            _alphaSlider = new Slider
            {
                Minimum = 0,
                Maximum = 255,
                Value = 255,
                IsVisible = false,
                MinimumTrackColor = MauiThemeManager.AccentColor,
                ThumbColor = MauiThemeManager.AccentColor,
            };
            _alphaSlider.ValueChanged += (s, e) =>
            {
                _alpha = (byte)Math.Max(0, Math.Min(255, (int)e.NewValue));
                OnColorChanged(fromHex: false);
            };
            Children.Add(_alphaSlider);

            // 4. 预览与 Hex 栏
            var infoGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(40) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                ColumnSpacing = 8,
            };

            _previewBorder = new Border
            {
                WidthRequest = 36,
                HeightRequest = 36,
                Stroke = MauiThemeManager.StrokeDefault,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6) },
            };
            infoGrid.Children.Add(_previewBorder);
            Grid.SetColumn((BindableObject)_previewBorder, 0);

            _hexEntry = new Entry
            {
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
                HeightRequest = 36,
            };
            _hexEntry.Completed += (s, e) => TryParseHex(_hexEntry.Text);
            _hexEntry.Unfocused += (s, e) => TryParseHex(_hexEntry.Text);
            infoGrid.Children.Add(_hexEntry);
            Grid.SetColumn((BindableObject)_hexEntry, 1);

            Children.Add(infoGrid);

            // 5. 预设颜色栏
            _presetLayout = new HorizontalStackLayout
            {
                Spacing = 8,
                IsVisible = false,
            };
            Children.Add(_presetLayout);

            UpdateFromRgb(255, 0, 0, 255);
        }

        public event EventHandler<ApeColor>? SelectedColorChanged;

        public bool AllowAlpha
        {
            get => _alphaSlider.IsVisible;
            set => _alphaSlider.IsVisible = value;
        }

        public ApeColor SelectedColor
        {
            get
            {
                ColorMathHelper.HsvToRgb(_hue, _saturation, _value, out var r, out var g, out var b);
                return new ApeColor(_alpha, r, g, b);
            }
            set
            {
                _alpha = value.A;
                _alphaSlider.Value = _alpha;
                ColorMathHelper.RgbToHsv(value.R, value.G, value.B, out _hue, out _saturation, out _value);
                OnColorChanged(fromHex: false);
            }
        }

        public void SetPresetColors(IReadOnlyList<ApeColor>? presetColors)
        {
            _presetLayout.Children.Clear();
            if (presetColors == null || presetColors.Count == 0)
            {
                _presetLayout.IsVisible = false;
                return;
            }

            _presetLayout.IsVisible = true;
            foreach (var preset in presetColors)
            {
                var dot = new Border
                {
                    WidthRequest = 24,
                    HeightRequest = 24,
                    Stroke = MauiThemeManager.StrokeDefault,
                    StrokeThickness = 1,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                    BackgroundColor = preset.ToMauiColor(),
                };
                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, _) => SelectedColor = preset;
                dot.GestureRecognizers.Add(tap);
                _presetLayout.Children.Add(dot);
            }
        }

        private void OnColorChanged(bool fromHex)
        {
            if (_isInternalUpdating) return;
            _isInternalUpdating = true;

            ColorMathHelper.HsvToRgb(_hue, _saturation, _value, out var r, out var g, out var b);
            var color = new ApeColor(_alpha, r, g, b);

            _previewBorder.BackgroundColor = color.ToMauiColor();

            if (!fromHex)
            {
                _hexEntry.Text = AllowAlpha
                    ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                    : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }

            _svPanel.Invalidate();
            _hueBar.Invalidate();

            _isInternalUpdating = false;
            SelectedColorChanged?.Invoke(this, color);
        }

        private void TryParseHex(string? hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return;

            var clean = hex.Trim().TrimStart('#');
            try
            {
                if (clean.Length == 6)
                {
                    var r = Convert.ToByte(clean.Substring(0, 2), 16);
                    var g = Convert.ToByte(clean.Substring(2, 2), 16);
                    var b = Convert.ToByte(clean.Substring(4, 2), 16);
                    UpdateFromRgb(r, g, b, _alpha);
                }
                else if (clean.Length == 8)
                {
                    var a = Convert.ToByte(clean.Substring(0, 2), 16);
                    var r = Convert.ToByte(clean.Substring(2, 2), 16);
                    var g = Convert.ToByte(clean.Substring(4, 2), 16);
                    var b = Convert.ToByte(clean.Substring(6, 2), 16);
                    UpdateFromRgb(r, g, b, a);
                }
            }
            catch
            {
                // 回退为当前颜色
                OnColorChanged(fromHex: false);
            }
        }

        private void UpdateFromRgb(byte r, byte g, byte b, byte a)
        {
            _alpha = a;
            ColorMathHelper.RgbToHsv(r, g, b, out _hue, out _saturation, out _value);
            OnColorChanged(fromHex: true);
        }

        private sealed class SvDrawable : IDrawable
        {
            private readonly ApeColorPicker _picker;

            public SvDrawable(ApeColorPicker picker) => _picker = picker;

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0) return;

                ColorMathHelper.HsvToRgb(_picker._hue, 1f, 1f, out var r, out var g, out var b);
                var pureHueColor = Color.FromRgb(r, g, b);

                // 1. 横向纯白到纯色渐变
                var horizontalPaint = new LinearGradientPaint
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0),
                    GradientStops = new[]
                    {
                        new PaintGradientStop(0f, Colors.White),
                        new PaintGradientStop(1f, pureHueColor),
                    }
                };
                canvas.SetFillPaint(horizontalPaint, dirtyRect);
                canvas.FillRoundedRectangle(dirtyRect, 6);

                // 2. 纵向透明到纯黑遮罩
                var verticalPaint = new LinearGradientPaint
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops = new[]
                    {
                        new PaintGradientStop(0f, Colors.Transparent),
                        new PaintGradientStop(1f, Colors.Black),
                    }
                };
                canvas.SetFillPaint(verticalPaint, dirtyRect);
                canvas.FillRoundedRectangle(dirtyRect, 6);

                // 3. 绘制选中圆环指示器
                var indicatorX = dirtyRect.Width * _picker._saturation;
                var indicatorY = dirtyRect.Height * (1f - _picker._value);

                canvas.StrokeColor = Colors.White;
                canvas.StrokeSize = 2f;
                canvas.DrawCircle(indicatorX, indicatorY, 6);

                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 1f;
                canvas.DrawCircle(indicatorX, indicatorY, 7);
            }
        }

        private sealed class HueDrawable : IDrawable
        {
            private readonly ApeColorPicker _picker;

            public HueDrawable(ApeColorPicker picker) => _picker = picker;

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0) return;

                var rainbowPaint = new LinearGradientPaint
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0),
                    GradientStops = new[]
                    {
                        new PaintGradientStop(0f, Color.FromRgb(255, 0, 0)),
                        new PaintGradientStop(0.17f, Color.FromRgb(255, 255, 0)),
                        new PaintGradientStop(0.33f, Color.FromRgb(0, 255, 0)),
                        new PaintGradientStop(0.5f, Color.FromRgb(0, 255, 255)),
                        new PaintGradientStop(0.67f, Color.FromRgb(0, 0, 255)),
                        new PaintGradientStop(0.83f, Color.FromRgb(255, 0, 255)),
                        new PaintGradientStop(1f, Color.FromRgb(255, 0, 0)),
                    }
                };
                canvas.SetFillPaint(rainbowPaint, dirtyRect);
                canvas.FillRoundedRectangle(dirtyRect, 4);

                // 绘制当前色相滑块位置
                var ratio = _picker._hue / 360f;
                var indicatorX = dirtyRect.Width * ratio;

                canvas.StrokeColor = Colors.White;
                canvas.StrokeSize = 2.5f;
                canvas.DrawLine(indicatorX, 0, indicatorX, dirtyRect.Height);
            }
        }
    }
}
