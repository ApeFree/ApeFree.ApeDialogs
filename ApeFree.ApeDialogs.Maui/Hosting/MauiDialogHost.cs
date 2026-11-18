using System;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Maui.Controls;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Hosting
{
    /// <summary>
    /// MAUI 对话框全屏宿主容器（含遮罩、居中圆角卡片、阴影、进出场动画与按钮栏）。
    /// </summary>
    public partial class MauiDialogHost : Grid
    {
        private readonly Border _cardBorder;
        private readonly Label _titleLabel;
        private readonly Button _closeButton;
        private readonly Grid _titleBarGrid;
        private readonly Label _contentLabel;
        private readonly ContentView _customContentPresenter;
        private readonly HorizontalStackLayout _optionsLayout;
        private readonly ScrollView _contentScrollView;
        private bool _cancelable = true;

        public MauiDialogHost()
        {
            ZIndex = 99999;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;

            // 1. 半透明遮罩
            var overlay = new BoxView
            {
                Color = MauiThemeManager.OverlayColor,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };

            var overlayTap = new TapGestureRecognizer();
            overlayTap.Tapped += (_, _) =>
            {
                if (_cancelable)
                {
                    CloseRequested?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _ = PlayShakeAnimationAsync();
                }
            };
            overlay.GestureRecognizers.Add(overlayTap);
            Children.Add(overlay);

            // 2. 居中卡片
            _cardBorder = new Border
            {
                Stroke = MauiThemeManager.StrokeDefault,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                BackgroundColor = MauiThemeManager.CardBackground,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                MaximumWidthRequest = 440,
                Margin = new Thickness(20),
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Colors.Black),
                    Offset = new Point(0, 8),
                    Radius = 24,
                    Opacity = 0.25f,
                }
            };

            var cardGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // 标题栏
                    new RowDefinition { Height = GridLength.Auto }, // 正文 & 自定义内容
                    new RowDefinition { Height = GridLength.Auto }, // 底部选项按钮
                },
                Padding = new Thickness(20),
            };

            // 标题栏
            _titleBarGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
                Margin = new Thickness(0, 0, 0, 12),
            };

            _titleLabel = new Label
            {
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = MauiThemeManager.TextPrimary,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation,
            };
            _titleBarGrid.Children.Add(_titleLabel);
            Grid.SetColumn(_titleLabel, 0);

            _closeButton = new Button
            {
                Text = "✕",
                FontSize = 14,
                WidthRequest = 32,
                HeightRequest = 32,
                Padding = 0,
                CornerRadius = 16,
                BackgroundColor = Colors.Transparent,
                TextColor = MauiThemeManager.TextSecondary,
                VerticalOptions = LayoutOptions.Center,
            };
            _closeButton.Clicked += (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty);
            _titleBarGrid.Children.Add(_closeButton);
            Grid.SetColumn(_closeButton, 1);

            cardGrid.Children.Add(_titleBarGrid);
            Grid.SetRow(_titleBarGrid, 0);

            // 内容区
            var contentStack = new VerticalStackLayout { Spacing = 10 };

            _contentLabel = new Label
            {
                FontSize = 14,
                TextColor = MauiThemeManager.TextPrimary,
                LineBreakMode = LineBreakMode.WordWrap,
                IsVisible = false,
            };
            contentStack.Children.Add(_contentLabel);

            _customContentPresenter = new ContentView();
            contentStack.Children.Add(_customContentPresenter);

            _contentScrollView = new ScrollView
            {
                Content = contentStack,
                MaximumHeightRequest = 420,
            };

            cardGrid.Children.Add(_contentScrollView);
            Grid.SetRow(_contentScrollView, 1);

            // 底部选项栏
            _optionsLayout = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 16, 0, 0),
            };
            cardGrid.Children.Add(_optionsLayout);
            Grid.SetRow(_optionsLayout, 2);

            _cardBorder.Content = cardGrid;
            Children.Add(_cardBorder);
        }

        /// <summary>
        /// 关闭请求事件（点击关闭按钮或遮罩）。
        /// </summary>
        public event EventHandler? CloseRequested;

        /// <summary>
        /// 标题文本。
        /// </summary>
        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value;
        }

        /// <summary>
        /// 正文提示文本。
        /// </summary>
        public string ContentText
        {
            get => _contentLabel.Text;
            set
            {
                _contentLabel.Text = value;
                _contentLabel.IsVisible = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// 是否允许取消。
        /// </summary>
        public bool Cancelable
        {
            get => _cancelable;
            set
            {
                _cancelable = value;
                _closeButton.IsVisible = value;
            }
        }

        /// <summary>
        /// 设置提醒色/重点边框色。
        /// </summary>
        public void SetReminderColor(ApeColor color)
        {
            _cardBorder.Stroke = color.ToMauiColor();
            _cardBorder.StrokeThickness = 2;
        }

        /// <summary>
        /// 设置自定义主体控件。
        /// </summary>
        public void SetDialogContent(View content)
        {
            _customContentPresenter.Content = content;
        }

        /// <summary>
        /// 添加选项按钮。
        /// </summary>
        public void AddOptionButton(ApeButton button)
        {
            if (button == null) return;
            _optionsLayout.Children.Add(button);
        }

        /// <summary>
        /// 清空选项按钮。
        /// </summary>
        public void ClearOptionButtons()
        {
            _optionsLayout.Children.Clear();
        }

        /// <summary>
        /// 播放入场动画。
        /// </summary>
        public async Task PlayOpenAnimationAsync()
        {
            Opacity = 0;
            _cardBorder.Scale = 0.85;
            _ = this.FadeTo(1.0, 180, Easing.CubicOut);
            await _cardBorder.ScaleTo(1.0, 180, Easing.CubicOut);
        }

        /// <summary>
        /// 播放退场动画。
        /// </summary>
        public async Task PlayCloseAnimationAsync()
        {
            _ = this.FadeTo(0, 150, Easing.CubicIn);
            await _cardBorder.ScaleTo(0.9, 150, Easing.CubicIn);
        }

        /// <summary>
        /// 播放校验失败抖动动画。
        /// </summary>
        public async Task PlayShakeAnimationAsync()
        {
            await _cardBorder.TranslateTo(-10, 0, 40);
            await _cardBorder.TranslateTo(10, 0, 40);
            await _cardBorder.TranslateTo(-7, 0, 40);
            await _cardBorder.TranslateTo(7, 0, 40);
            await _cardBorder.TranslateTo(-3, 0, 40);
            await _cardBorder.TranslateTo(0, 0, 40);
        }
    }
}
