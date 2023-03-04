using ApeFree.ApeDialogs.Core;
using System;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog选项信息
    /// </summary>
    public class DialogOption
    {
        /// <summary>
        /// 选项文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 选项是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 选项类型
        /// </summary>
        public DialogOptionType OptionType { get; set; }

        /// <summary>
        /// 选项被选中时的回调过程 
        /// </summary>
        public Action<object, OptionSelectedEventArgs> OptionSelectedCallback { get; set; }

        public DialogOption(string text = null, DialogOptionType optionType = DialogOptionType.Neutral, bool enabled = true, Action<object, OptionSelectedEventArgs> callback = null)
        {
            Text = text;
            Enabled = enabled;
            OptionType = optionType;
            OptionSelectedCallback = callback;
        }
    }

    /// <summary>
    /// 选择选项事件参数
    /// </summary>
    public class OptionSelectedEventArgs : EventArgs
    {
        public OptionSelectedEventArgs(IDialog dialog, DialogOption option)
        {
            Dialog = dialog;
            Option = option;
        }

        /// <summary>
        /// 当前正在操作的Dialog
        /// </summary>
        public IDialog Dialog { get; set; }

        /// <summary>
        /// 选中的选项对应的选项信息
        /// </summary>
        public DialogOption Option { get; set; }
    }
}
