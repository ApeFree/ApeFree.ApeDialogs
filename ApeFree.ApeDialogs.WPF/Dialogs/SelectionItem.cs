namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 列表选择项包装（Item 原始数据 + 显示文本），供选择类 Dialog 的列表展示。
    /// </summary>
    internal sealed class SelectionItem
    {
        public SelectionItem(object? item, string displayText)
        {
            Item = item;
            DisplayText = displayText;
        }

        public object? Item { get; }

        public string DisplayText { get; }
    }
}
