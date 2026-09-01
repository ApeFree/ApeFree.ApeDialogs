using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格进度条（4px 圆角轨道；确定模式按 Value 填充，不确定模式播放滑动动画）。
    /// </summary>
    public class ApeProgressBar : ProgressBar
    {
        static ApeProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeProgressBar), new FrameworkPropertyMetadata(typeof(ApeProgressBar)));
        }
    }
}
