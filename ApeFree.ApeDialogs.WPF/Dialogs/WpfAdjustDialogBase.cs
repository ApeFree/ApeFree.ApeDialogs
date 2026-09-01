using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 实时调节类 Dialog 的 WPF 基类：在 <see cref="WpfDialogBase{TResult}"/> 之上实现 <see cref="IAdjustDialog{T}"/> 契约。
    /// 实现方在调节动作中调用 <see cref="RaiseValueChanged"/>，同时触发事件与 <see cref="DialogSettings{TResult}.OnValueChanged"/> 委托。
    /// </summary>
    /// <typeparam name="TResult">调节值类型</typeparam>
    public abstract class WpfAdjustDialogBase<TResult> : WpfDialogBase<TResult>, IAdjustDialog<TResult>
    {
        /// <inheritdoc/>
        protected WpfAdjustDialogBase(DialogSettings<TResult> settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
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
