using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格下拉框（圆角触发器、淡入弹层、项悬停/选中样式）。
    /// </summary>
    public class ApeComboBox : ComboBox
    {
        static ApeComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeComboBox), new FrameworkPropertyMetadata(typeof(ApeComboBox)));
        }
    }
}
