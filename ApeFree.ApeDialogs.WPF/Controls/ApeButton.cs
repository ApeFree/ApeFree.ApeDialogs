using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格按钮（圆角 4px、悬停/按下/焦点/禁用状态、可选强调色）。
    /// </summary>
    public class ApeButton : Button
    {
        static ApeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeButton), new FrameworkPropertyMetadata(typeof(ApeButton)));
        }

        /// <summary>
        /// 是否使用强调色（主操作按钮）。
        /// </summary>
        public static readonly DependencyProperty IsAccentProperty =
            DependencyProperty.Register(nameof(IsAccent), typeof(bool), typeof(ApeButton), new PropertyMetadata(false));

        /// <summary>
        /// 是否使用强调色（主操作按钮）。
        /// </summary>
        public bool IsAccent
        {
            get => (bool)GetValue(IsAccentProperty);
            set => SetValue(IsAccentProperty, value);
        }
    }
}
