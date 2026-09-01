using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格滑块（4px 圆角轨道、20px 圆形滑块、悬停/按下反馈、按 TickFrequency 吸附）。
    /// </summary>
    public class ApeSlider : Slider
    {
        static ApeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeSlider), new FrameworkPropertyMetadata(typeof(ApeSlider)));
        }
    }
}
