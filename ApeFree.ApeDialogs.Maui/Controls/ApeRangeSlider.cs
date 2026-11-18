using System;
using ApeFree.ApeDialogs.Maui.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Controls
{
    /// <summary>
    /// 双滑块范围选择控件（基于 Microsoft.Maui.Graphics 跨平台纯自绘）。
    /// </summary>
    public partial class ApeRangeSlider : GraphicsView, IDrawable
    {
        private const float ThumbRadius = 10f;
        private const float TrackHeight = 4f;

        private decimal _minimum = 0m;
        private decimal _maximum = 100m;
        private decimal _start = 25m;
        private decimal _end = 75m;

        private enum DraggingThumb
        {
            None,
            Start,
            End,
        }

        private DraggingThumb _activeThumb = DraggingThumb.None;
        private double _panStartX;

        public ApeRangeSlider()
        {
            HeightRequest = 44;
            Drawable = this;

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnTapped;
            GestureRecognizers.Add(tapGesture);
        }

        public event EventHandler? RangeChanged;

        public decimal Minimum
        {
            get => _minimum;
            set { _minimum = value; ClampValues(); Invalidate(); }
        }

        public decimal Maximum
        {
            get => _maximum;
            set { _maximum = value; ClampValues(); Invalidate(); }
        }

        public decimal Start
        {
            get => _start;
            set { _start = value; ClampValues(); Invalidate(); }
        }

        public decimal End
        {
            get => _end;
            set { _end = value; ClampValues(); Invalidate(); }
        }

        private void ClampValues()
        {
            if (_maximum < _minimum) _maximum = _minimum;
            _start = Math.Max(_minimum, Math.Min(_maximum, _start));
            _end = Math.Max(_start, Math.Min(_maximum, _end));
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0) return;

            var padding = ThumbRadius + 4;
            var trackWidth = dirtyRect.Width - (padding * 2);
            var centerY = dirtyRect.Height / 2f;

            if (trackWidth <= 0) return;

            var range = (float)(_maximum - _minimum);
            if (range <= 0) range = 1f;

            var startRatio = (float)((_start - _minimum) / (_maximum - _minimum));
            var endRatio = (float)((_end - _minimum) / (_maximum - _minimum));

            var startX = padding + (trackWidth * startRatio);
            var endX = padding + (trackWidth * endRatio);

            // 1. 绘制底色完整轨道
            canvas.StrokeColor = MauiThemeManager.StrokeDefault;
            canvas.StrokeSize = TrackHeight;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawLine(padding, centerY, dirtyRect.Width - padding, centerY);

            // 2. 绘制高亮选中区间轨道
            canvas.StrokeColor = MauiThemeManager.AccentColor;
            canvas.StrokeSize = TrackHeight + 1;
            canvas.DrawLine(startX, centerY, endX, centerY);

            // 3. 绘制 Start 滑块
            DrawThumb(canvas, startX, centerY);

            // 4. 绘制 End 滑块
            DrawThumb(canvas, endX, centerY);
        }

        private static void DrawThumb(ICanvas canvas, float cx, float cy)
        {
            // 白色底色
            canvas.FillColor = Colors.White;
            canvas.FillCircle(cx, cy, ThumbRadius);

            // 强调色外圈
            canvas.StrokeColor = MauiThemeManager.AccentColor;
            canvas.StrokeSize = 2.5f;
            canvas.DrawCircle(cx, cy, ThumbRadius);

            // 中心小圆点
            canvas.FillColor = MauiThemeManager.AccentColor;
            canvas.FillCircle(cx, cy, 3.5f);
        }

        private void OnTapped(object? sender, TappedEventArgs e)
        {
            var position = e.GetPosition(this);
            if (position == null) return;

            UpdateClosestThumbPosition(position.Value.X);
        }

        private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            var padding = ThumbRadius + 4;
            var trackWidth = Width - (padding * 2);
            if (trackWidth <= 0) return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    var touchX = _panStartX;
                    var startRatio = (double)((_start - _minimum) / (_maximum - _minimum));
                    var endRatio = (double)((_end - _minimum) / (_maximum - _minimum));
                    var startThumbX = padding + (trackWidth * startRatio);
                    var endThumbX = padding + (trackWidth * endRatio);

                    var distStart = Math.Abs(touchX - startThumbX);
                    var distEnd = Math.Abs(touchX - endThumbX);

                    _activeThumb = distStart <= distEnd ? DraggingThumb.Start : DraggingThumb.End;
                    break;

                case GestureStatus.Running:
                    var currentX = _panStartX + e.TotalX;
                    var ratio = Math.Max(0, Math.Min(1, (currentX - padding) / trackWidth));
                    var targetValue = _minimum + (decimal)ratio * (_maximum - _minimum);

                    if (_activeThumb == DraggingThumb.Start)
                    {
                        Start = Math.Min(targetValue, End);
                    }
                    else if (_activeThumb == DraggingThumb.End)
                    {
                        End = Math.Max(targetValue, Start);
                    }

                    RangeChanged?.Invoke(this, EventArgs.Empty);
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    _activeThumb = DraggingThumb.None;
                    break;
            }
        }

        private void UpdateClosestThumbPosition(double touchX)
        {
            _panStartX = touchX;
            var padding = ThumbRadius + 4;
            var trackWidth = Width - (padding * 2);
            if (trackWidth <= 0) return;

            var ratio = Math.Max(0, Math.Min(1, (touchX - padding) / trackWidth));
            var targetValue = _minimum + (decimal)ratio * (_maximum - _minimum);

            var distStart = Math.Abs(targetValue - Start);
            var distEnd = Math.Abs(targetValue - End);

            if (distStart <= distEnd)
            {
                Start = Math.Min(targetValue, End);
            }
            else
            {
                End = Math.Max(targetValue, Start);
            }

            RangeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
