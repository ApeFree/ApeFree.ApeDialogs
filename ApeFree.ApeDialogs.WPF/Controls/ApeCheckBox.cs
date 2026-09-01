using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格复选框（20px 圆角勾选块、悬停/按下/禁用状态）。
    /// </summary>
    public class ApeCheckBox : CheckBox
    {
        static ApeCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeCheckBox), new FrameworkPropertyMetadata(typeof(ApeCheckBox)));
        }
    }
}
