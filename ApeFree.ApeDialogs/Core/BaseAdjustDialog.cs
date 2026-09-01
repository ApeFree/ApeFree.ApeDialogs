using System;
using ApeFree.ApeDialogs.Settings;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// 实时调节类 Dialog 的实现辅助基类。
    /// 实现方在调节动作中调用 <see cref="RaiseValueChanged"/>，同时触发 <see cref="IAdjustDialog{T}.ValueChanged"/>
    /// 事件与 <see cref="DialogSettings{TResult}.OnValueChanged"/> 委托。
    /// </summary>
    /// <typeparam name="TView">视图类型</typeparam>
    /// <typeparam name="TOption">选项控件类型</typeparam>
    /// <typeparam name="TContext">上下文类型</typeparam>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public abstract class BaseAdjustDialog<TView, TOption, TContext, TResult> : BaseDialog<TView, TOption, TContext, TResult>, IAdjustDialog<TResult>
    {
        /// <inheritdoc/>
        protected BaseAdjustDialog(DialogSettings<TResult> settings, TContext? context = default, TView? view = default)
            : base(settings, context, view)
        {
        }

        /// <inheritdoc/>
        public event EventHandler<ValueChangedEventArgs<TResult>>? ValueChanged;

        /// <inheritdoc/>
        public abstract TResult Value { get; }

        /// <summary>
        /// 触发值变化（同时调用设置中的 <see cref="DialogSettings{TResult}.OnValueChanged"/> 委托）。
        /// </summary>
        protected void RaiseValueChanged(TResult newValue)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs<TResult>(newValue));
            Settings.OnValueChanged?.Invoke(newValue);
        }
    }
}
