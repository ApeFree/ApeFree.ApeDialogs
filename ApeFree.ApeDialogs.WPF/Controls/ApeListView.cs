using System.Windows;
using System.Windows.Controls;

namespace ApeFree.ApeDialogs.WPF.Controls
{
    /// <summary>
    /// Win11 Fluent 风格列表（透明边框、项悬停/选中样式，供单选/多选 Dialog 使用）。
    /// </summary>
    public class ApeListView : ListView
    {
        static ApeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApeListView), new FrameworkPropertyMetadata(typeof(ApeListView)));
        }
    }
}
