using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格单选按钮（20px 圆形、悬停/按下/禁用状态）。
    /// </summary>
    public class ApeRadioButton : RadioButton
    {
        static ApeRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeRadioButton), new FrameworkPropertyMetadata(typeof(ApeRadioButton)));
        }
    }
}
