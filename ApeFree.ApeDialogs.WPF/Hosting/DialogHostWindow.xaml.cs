using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ApeFree.ApeDialogs.WPF.Theme;

namespace ApeFree.ApeDialogs.WPF.Hosting
{
    /// <summary>
    /// 对话框宿主窗口：Win11 风格统一外壳（圆角、阴影、亚克力背景、进出场动画、标题栏、选项区）。
    /// 19 种 Dialog 均共享该宿主，内容与选项由各 Dialog 实现装配。
    /// </summary>
    public partial class DialogHostWindow : Window
    {
        /// <summary>
        /// 是否显示关闭按钮（对应 Settings.Cancelable）。
        /// </summary>
        public bool ShowCloseButton
        {
            get => CloseButton.Visibility == Visibility.Visible;
            set => CloseButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 是否显示标题栏。
        /// </summary>
        public bool ShowTitleBar
        {
            get => TitleBarGrid.Visibility == Visibility.Visible;
            set => TitleBarGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 关闭按钮被点击（或 Esc）时触发（宿主不自动关闭，由 Dialog 实现方决定取消语义）。
        /// </summary>
        public event EventHandler? CloseRequested;

        /// <summary>
        /// 回车键按下时触发（由 Dialog 实现方决定确认语义）。
        /// </summary>
        public event EventHandler? ConfirmRequested;

        /// <summary>
        /// 内容区被点击时触发（Toast 等无按钮对话框使用）。
        /// </summary>
        public event MouseButtonEventHandler? ContentClicked;

        public DialogHostWindow()
        {
            ThemeManager.EnsureLoaded();

            InitializeComponent();

            Loaded += OnLoaded;
            PreviewKeyDown += OnPreviewKeyDown;
        }

        /// <summary>
        /// 添加选项按钮。
        /// </summary>
        public void AddOptionButton(Button button)
        {
            if (button is null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            button.Margin = new Thickness(8, 0, 0, 0);
            OptionsPanel.Children.Add(button);
            OptionsPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 清空选项按钮。
        /// </summary>
        public void ClearOptionButtons()
        {
            OptionsPanel.Children.Clear();
            OptionsPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 设置对话框内容。
        /// </summary>
        public void SetDialogContent(object content)
        {
            ContentPresenter.Content = content;
            ContentPresenter.MouseLeftButtonUp += (_, e) => ContentClicked?.Invoke(this, e);
        }

        /// <summary>
        /// 播放进场动画（淡入 + 上移，约 180ms）。
        /// </summary>
        public void PlayOpenAnimation()
        {
            RootBorder.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(180)));

            var translate = new TranslateTransform();
            RootBorder.RenderTransform = translate;
            translate.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(8, 0, TimeSpan.FromMilliseconds(180))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            });
        }

        /// <summary>
        /// 立即关闭窗口。
        /// </summary>
        public void PlayCloseAnimation()
        {
            Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Title) && !ShowCloseButton)
            {
                TitleBarGrid.Visibility = Visibility.Collapsed;
            }

            if (OptionsPanel.Children.Count == 0)
            {
                OptionsPanel.Visibility = Visibility.Collapsed;
            }

            PlayOpenAnimation();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && ShowCloseButton)
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                ConfirmRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 播放校验失败抖动动画。
        /// </summary>
        public void PlayShakeAnimation()
        {
            var translate = RootBorder.RenderTransform as TranslateTransform;
            if (translate is null)
            {
                translate = new TranslateTransform();
                RootBorder.RenderTransform = translate;
            }

            var animation = new DoubleAnimationUsingKeyFrames { Duration = TimeSpan.FromMilliseconds(240) };
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(-6, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(60))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(6, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(120))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(-4, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(180))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(240))));
            translate.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
            {
                DragMove();
            }
        }
    }
}
