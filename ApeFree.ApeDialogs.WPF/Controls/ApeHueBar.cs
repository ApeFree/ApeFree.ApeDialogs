using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// 色相选择条（纵向 0~360°，红→黄→绿→青→蓝→品红→红 渐变）。
    /// 纯代码自绘。
    /// </summary>
    public sealed class ApeHueBar : FrameworkElement
    {
        private bool _dragging;

        /// <summary>
        /// 当前色相（0~360）。
        /// </summary>
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(ApeHueBar), new PropertyMetadata(0.0, OnVisualPropertyChanged));

        /// <summary>
        /// 当前色相（0~360）。
        /// </summary>
        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        /// <summary>
        /// 用户在色相条上拖动时触发。
        /// </summary>
        public event EventHandler? HueChanged;

        static ApeHueBar()
        {
            FocusableProperty.OverrideMetadata(typeof(ApeHueBar), new FrameworkPropertyMetadata(true));
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

            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops =
                {
                    new GradientStop(Color.FromRgb(255, 0, 0), 0.00),
                    new GradientStop(Color.FromRgb(255, 255, 0), 0.17),
                    new GradientStop(Color.FromRgb(0, 255, 0), 0.33),
                    new GradientStop(Color.FromRgb(0, 255, 255), 0.50),
                    new GradientStop(Color.FromRgb(0, 0, 255), 0.67),
                    new GradientStop(Color.FromRgb(255, 0, 255), 0.83),
                    new GradientStop(Color.FromRgb(255, 0, 0), 1.00),
                },
            };

            drawingContext.DrawRoundedRectangle(brush, null, new Rect(0, 0, width, height), 4, 4);

            // 指示器（水平白圈）
            var y = (Hue / 360.0) * height;
            var center = new Point(width / 2.0, y);
            drawingContext.DrawEllipse(null, new Pen(Brushes.Black, 1.5), center, 7, 7);
            drawingContext.DrawEllipse(null, new Pen(Brushes.White, 1.5), center, 5, 5);
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
            var height = Math.Max(1, ActualHeight);
            var hue = Math.Max(0, Math.Min(360, position.Y / height * 360));

            SetCurrentValue(HueProperty, hue);
            InvalidateVisual();

            HueChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ApeHueBar)d).InvalidateVisual();
        }
    }
}
