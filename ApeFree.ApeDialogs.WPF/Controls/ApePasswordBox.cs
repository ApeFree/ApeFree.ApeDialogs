using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格密码框（PasswordBox 为密封类，故采用包装控件方案：
    /// 模板内持有 PasswordBox，通过 <see cref="Password"/> 依赖属性与占位符对外暴露）。
    /// </summary>
    public class ApePasswordBox : Control
    {
        private PasswordBox? _innerPasswordBox;

        static ApePasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApePasswordBox), new FrameworkPropertyMetadata(typeof(ApePasswordBox)));
        }

        /// <summary>
        /// 密码内容（双向同步内部 PasswordBox）。
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password), typeof(string), typeof(ApePasswordBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));

        /// <summary>
        /// 密码内容（双向同步内部 PasswordBox）。
        /// </summary>
        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        /// <summary>
        /// 占位符文本（密码为空且未输入时显示）。
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ApePasswordBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 占位符文本。
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        /// <summary>
        /// 光标（插入符）颜色。
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register(nameof(CaretBrush), typeof(System.Windows.Media.Brush), typeof(ApePasswordBox), new PropertyMetadata(null));

        /// <summary>
        /// 光标（插入符）颜色。
        /// </summary>
        public System.Windows.Media.Brush CaretBrush
        {
            get => (System.Windows.Media.Brush)GetValue(CaretBrushProperty);
            set => SetValue(CaretBrushProperty, value);
        }

        /// <summary>
        /// 选中文本高亮颜色。
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(nameof(SelectionBrush), typeof(System.Windows.Media.Brush), typeof(ApePasswordBox), new PropertyMetadata(null));

        /// <summary>
        /// 选中文本高亮颜色。
        /// </summary>
        public System.Windows.Media.Brush SelectionBrush
        {
            get => (System.Windows.Media.Brush)GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_innerPasswordBox is not null)
            {
                _innerPasswordBox.PasswordChanged -= OnInnerPasswordChanged;
            }

            _innerPasswordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;

            if (_innerPasswordBox is not null)
            {
                _innerPasswordBox.Password = Password;
                _innerPasswordBox.PasswordChanged += OnInnerPasswordChanged;
            }
        }

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (ApePasswordBox)d;
            if (box._innerPasswordBox is not null && box._innerPasswordBox.Password != (string)e.NewValue)
            {
                box._innerPasswordBox.Password = (string)e.NewValue;
            }
        }

        private void OnInnerPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_innerPasswordBox is not null && _innerPasswordBox.Password != Password)
            {
                SetCurrentValue(PasswordProperty, _innerPasswordBox.Password);
            }
        }
    }
}
