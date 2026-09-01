using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ApeFree.ApeDialogs.WPF.Theme;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// 双滑块范围选择控件（纯代码自绘：4px 圆角轨道 + 中间高亮 + 两个圆形滑块）。
    /// 支持：拖拽任一端滑块、点击轨道将最近滑块移动到点击处（MoveToPoint）。
    /// </summary>
    public sealed class ApeRangeSlider : FrameworkElement
    {
        private const double DefaultThumbRadius = 8.0;
        private enum DragThumb
        {
            None,
            Start,
            End,
        }

        private DragThumb _dragThumb = DragThumb.None;

        /// <summary>
        /// 允许的最小值。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(decimal), typeof(ApeRangeSlider), new PropertyMetadata(0m, OnValuePropertyChanged));

        /// <summary>
        /// 允许的最小值。
        /// </summary>
        public decimal Minimum
        {
            get => (decimal)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// 允许的最大值。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(decimal), typeof(ApeRangeSlider), new PropertyMetadata(100m, OnValuePropertyChanged));

        /// <summary>
        /// 允许的最大值。
        /// </summary>
        public decimal Maximum
        {
            get => (decimal)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// 起始值。
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(nameof(Start), typeof(decimal), typeof(ApeRangeSlider), new PropertyMetadata(25m, OnValuePropertyChanged));

        /// <summary>
        /// 起始值。
        /// </summary>
        public decimal Start
        {
            get => (decimal)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }

        /// <summary>
        /// 结束值。
        /// </summary>
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register(nameof(End), typeof(decimal), typeof(ApeRangeSlider), new PropertyMetadata(75m, OnValuePropertyChanged));

        /// <summary>
        /// 结束值。
        /// </summary>
        public decimal End
        {
            get => (decimal)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }

        /// <summary>
        /// 用户拖动调节范围时触发（拖拽过程中持续触发）。
        /// </summary>
        public event EventHandler? RangeChanged;

        static ApeRangeSlider()
        {
            FocusableProperty.OverrideMetadata(typeof(ApeRangeSlider), new FrameworkPropertyMetadata(true));
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

            // 透明背景确保整块区域接收鼠标事件
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, width, height));

            var thumbRadius = DefaultThumbRadius;
            var trackTop = (height - 4) / 2.0;
            var trackRect = new Rect(thumbRadius, trackTop, Math.Max(0, width - thumbRadius * 2), 4);

            var trackBrush = GetBrush("ApeControlFillTertiary", new SolidColorBrush(Color.FromRgb(0xE5, 0xE5, 0xE5)));
            var accentBrush = GetBrush("ApeAccentFillDefault", new SolidColorBrush(Color.FromRgb(0x00, 0x67, 0xC0)));
            var thumbFill = GetBrush("ApeWindowBackground", Brushes.White);
            var thumbStroke = GetBrush("ApeAccentFillDefault", new SolidColorBrush(Color.FromRgb(0x00, 0x67, 0xC0)));

            // 轨道背景
            drawingContext.DrawRoundedRectangle(trackBrush, null, trackRect, 2, 2);

            // 中间高亮选区
            var startX = ValueToX(Start);
            var endX = ValueToX(End);
            var selectionRect = new Rect(startX, trackTop, Math.Max(0, endX - startX), 4);
            drawingContext.DrawRoundedRectangle(accentBrush, null, selectionRect, 2, 2);

            // 两个滑块
            var centerY = trackTop + 2;
            var strokePen = new Pen(thumbStroke, 4);
            drawingContext.DrawEllipse(thumbFill, strokePen, new Point(startX, centerY), thumbRadius, thumbRadius);
            drawingContext.DrawEllipse(thumbFill, strokePen, new Point(endX, centerY), thumbRadius, thumbRadius);
        }

        private static Brush GetBrush(string key, Brush fallback)
        {
            return ThemeManager.GetResource(key) as Brush ?? fallback;
        }

        private double ValueToX(decimal value)
        {
            var range = Maximum - Minimum;
            if (range <= 0)
            {
                return DefaultThumbRadius;
            }

            var ratio = (double)((value - Minimum) / range);
            ratio = Math.Max(0.0, Math.Min(1.0, ratio));
            return DefaultThumbRadius + Math.Max(0, ActualWidth - DefaultThumbRadius * 2) * ratio;
        }

        private decimal XToValue(double x)
        {
            var usableWidth = Math.Max(1, ActualWidth - DefaultThumbRadius * 2);
            var ratio = (x - DefaultThumbRadius) / usableWidth;
            ratio = Math.Max(0, Math.Min(1, ratio));
            return Minimum + (Maximum - Minimum) * (decimal)ratio;
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var position = e.GetPosition(this);
            var startX = ValueToX(Start);
            var endX = ValueToX(End);
            var hitRadius = DefaultThumbRadius * 2.2;

            if (Math.Abs(position.X - startX) <= hitRadius && Math.Abs(position.X - startX) <= Math.Abs(position.X - endX))
            {
                _dragThumb = DragThumb.Start;
            }
            else if (Math.Abs(position.X - endX) <= hitRadius)
            {
                _dragThumb = DragThumb.End;
            }
            else
            {
                // MoveToPoint：将最近的滑块移动到点击处
                var value = XToValue(position.X);
                if (Math.Abs(value - Start) <= Math.Abs(value - End))
                {
                    SetCurrentValue(StartProperty, Math.Min(value, End));
                    _dragThumb = DragThumb.Start;
                }
                else
                {
                    SetCurrentValue(EndProperty, Math.Max(value, Start));
                    _dragThumb = DragThumb.End;
                }

                InvalidateVisual();
                RangeChanged?.Invoke(this, EventArgs.Empty);
            }

            if (_dragThumb != DragThumb.None)
            {
                CaptureMouse();
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragThumb == DragThumb.None)
            {
                return;
            }

            var value = XToValue(e.GetPosition(this).X);

            if (_dragThumb == DragThumb.Start)
            {
                SetCurrentValue(StartProperty, Math.Min(value, End));
            }
            else
            {
                SetCurrentValue(EndProperty, Math.Max(value, Start));
            }

            InvalidateVisual();
            RangeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _dragThumb = DragThumb.None;
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (ApeRangeSlider)d;

            // 维护 Start <= End 约束（程序化设置时也生效）
            if (e.Property == StartProperty && slider.Start > slider.End)
            {
                slider.SetCurrentValue(EndProperty, slider.Start);
            }
            else if (e.Property == EndProperty && slider.End < slider.Start)
            {
                slider.SetCurrentValue(StartProperty, slider.End);
            }

            slider.InvalidateVisual();
        }
    }
}
