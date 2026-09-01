using System;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// 调节类 Dialog（如滑块）的实时值变化事件参数。
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public ValueChangedEventArgs(T newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// 变化后的新值。
        /// </summary>
        public T NewValue { get; }
    }

    /// <summary>
    /// 实时调节类 Dialog 接口（消费者视角）。
    /// 显示期间通过 <see cref="ValueChanged"/> 持续观察值变化，最终结果仍由 <see cref="IDialog{T}.ShowAsync"/> 返回。
    /// 注意：调节过程已实时推送给外界，取消 Dialog 并不会回滚值，由消费方决定是否回滚。
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public interface IAdjustDialog<T> : IDialog<T>
    {
        /// <summary>
        /// 值实时变化事件（调节过程中持续触发，实现方按平台惯例节流）。
        /// </summary>
        event EventHandler<ValueChangedEventArgs<T>>? ValueChanged;

        /// <summary>
        /// 当前值。
        /// </summary>
        T Value { get; }
    }
}
