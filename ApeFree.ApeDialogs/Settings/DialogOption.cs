using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 选项内置行为。实现方依据行为执行对应动作（如取消、清空输入），优先于 <see cref="DialogOption.OnSelected"/> 自定义回调。
    /// </summary>
    public enum DialogOptionBehavior : byte
    {
        /// <summary>
        /// 无内置行为（仅执行 <see cref="DialogOption.OnSelected"/> 回调）。
        /// </summary>
        None,

        /// <summary>
        /// 取消：以取消结果关闭对话框。
        /// </summary>
        Cancel,

        /// <summary>
        /// 清空：清空对话框当前输入内容。
        /// </summary>
        Clear,

        /// <summary>
        /// 全选：选中全部可选项。
        /// </summary>
        SelectAll,

        /// <summary>
        /// 反选：反转当前选择。
        /// </summary>
        ReverseSelection,

        /// <summary>
        /// 设为当前时间：将日期时间选择器设为当前时刻。
        /// </summary>
        SetCurrentTime,
    }

    /// <summary>
    /// Dialog 选项信息（不可变）。使用 with 表达式派生变体。
    /// </summary>
    public sealed record DialogOption(
        string Text,
        DialogOptionTag Tag = DialogOptionTag.Neutral,
        bool IsEnabled = true,
        DialogOptionBehavior Behavior = DialogOptionBehavior.None,
        EventHandler<OptionSelectedEventArgs>? OnSelected = null)
    {
        /// <summary>
        /// 构造一个"取消"选项（<see cref="DialogOptionBehavior.Cancel"/>）。
        /// </summary>
        public static DialogOption CreateCancelOption(string text = "Cancel")
            => new(text, DialogOptionTag.Negative, true, DialogOptionBehavior.Cancel);
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
