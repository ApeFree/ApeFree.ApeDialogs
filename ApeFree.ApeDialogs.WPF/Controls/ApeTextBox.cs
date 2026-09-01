using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格文本框（圆角、悬停/焦点边框、占位符）。
    /// </summary>
    public class ApeTextBox : TextBox
    {
        static ApeTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeTextBox), new FrameworkPropertyMetadata(typeof(ApeTextBox)));
        }

        /// <summary>
        /// 占位符文本（文本为空且未输入时显示）。
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ApeTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 占位符文本。
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }
    }
}
