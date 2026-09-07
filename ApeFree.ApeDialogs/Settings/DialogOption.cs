using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog 选项信息（不可变）。使用 with 表达式派生变体。
    /// </summary>
    public sealed record DialogOption(
        string Text,
        DialogOptionTag Tag = DialogOptionTag.Neutral,
        bool IsEnabled = true,
        Action<IDialog>? Action = null,
        EventHandler<OptionSelectedEventArgs>? OnSelected = null)
    {
        /// <summary>
        /// 构造一个"取消"选项。
        /// </summary>
        public static DialogOption CreateCancelOption(string text = "Cancel")
            => new(text, DialogOptionTag.Negative, true, Action: dialog => dialog.Cancel());
    }

    /// <summary>
    /// 选项被选中时的事件参数。
    /// </summary>
    public class OptionSelectedEventArgs : EventArgs
    {
        public OptionSelectedEventArgs(IDialog dialog, DialogOption option)
        {
            Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
            Option = option ?? throw new ArgumentNullException(nameof(option));
        }

        /// <summary>
        /// 当前正在操作的 Dialog。
        /// </summary>
        public IDialog Dialog { get; }

        /// <summary>
        /// 选中的选项。
        /// </summary>
        public DialogOption Option { get; }
    }
}
