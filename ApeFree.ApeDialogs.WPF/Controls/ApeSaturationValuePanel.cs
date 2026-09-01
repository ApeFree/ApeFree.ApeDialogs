using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// 饱和度-明度（SV）选择面板：横向 S（0~1，左→右），纵向 V（0~1，下→上）。
    /// 纯代码自绘：白色→纯色横向渐变叠加透明→黑色纵向渐变。
    /// </summary>
    public sealed class ApeSaturationValuePanel : FrameworkElement
    {
        private bool _dragging;

        /// <summary>
        /// 色相（0~360），变化时重绘。
        /// </summary>
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(ApeSaturationValuePanel), new PropertyMetadata(0.0, OnVisualPropertyChanged));

        /// <summary>
        /// 色相（0~360），变化时重绘。
        /// </summary>
        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        /// <summary>
        /// 饱和度（0~1）。
        /// </summary>
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(ApeSaturationValuePanel), new PropertyMetadata(0.0, OnVisualPropertyChanged));

        /// <summary>
        /// 饱和度（0~1）。
        /// </summary>
        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        /// <summary>
        /// 明度（0~1）。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ApeSaturationValuePanel), new PropertyMetadata(1.0, OnVisualPropertyChanged));

        /// <summary>
        /// 明度（0~1）。
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// 用户在面板内拖动调节 S/V 时触发。
        /// </summary>
        public event EventHandler? ColorAreaChanged;

        static ApeSaturationValuePanel()
        {
            FocusableProperty.OverrideMetadata(typeof(ApeSaturationValuePanel), new FrameworkPropertyMetadata(true));
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var width = ActualWidth;
            var height = ActualHeight;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            var rect = new Rect(0, 0, width, height);

            var hueColor = ColorHelper.HsvToRgb(Hue, 1, 1);
            var hueBrush = new LinearGradientBrush(Colors.White, hueColor, new Point(0, 0), new Point(1, 0));
            drawingContext.DrawRectangle(hueBrush, null, rect);

            var valueBrush = new LinearGradientBrush(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 0, 0), new Point(0, 0), new Point(0, 1));
            drawingContext.DrawRectangle(valueBrush, null, rect);

            // 指示器（外黑圈 + 内白圈）
            var x = Saturation * width;
            var y = (1 - Value) * height;
            drawingContext.DrawEllipse(null, new Pen(Brushes.Black, 1.5), new Point(x, y), 6, 6);
            drawingContext.DrawEllipse(null, new Pen(Brushes.White, 1.5), new Point(x, y), 4.5, 4.5);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _dragging = true;
            CaptureMouse();
            UpdateFromPosition(e.GetPosition(this));
            e.Handled = true;
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging)
            {
                UpdateFromPosition(e.GetPosition(this));
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _dragging = false;
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }

        private void UpdateFromPosition(Point position)
        {
            var width = Math.Max(1, ActualWidth);
            var height = Math.Max(1, ActualHeight);

            var saturation = Math.Max(0, Math.Min(1, position.X / width));
            var value = Math.Max(0, Math.Min(1, 1 - position.Y / height));

            SetCurrentValue(SaturationProperty, saturation);
            SetCurrentValue(ValueProperty, value);
            InvalidateVisual();

            ColorAreaChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ApeSaturationValuePanel)d).InvalidateVisual();
        }
    }
}
