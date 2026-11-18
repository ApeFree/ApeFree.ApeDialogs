using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Maui.Hosting
{
    /// <summary>
    /// 实时调节类 Dialog 的 MAUI 基类：在 <see cref="MauiDialogBase{TResult}"/> 之上实现 <see cref="IAdjustDialog{T}"/> 契约。
    /// </summary>
    /// <typeparam name="TResult">调节值类型</typeparam>
    public abstract class MauiAdjustDialogBase<TResult> : MauiDialogBase<TResult>, IAdjustDialog<TResult>
    {
        protected MauiAdjustDialogBase(DialogSettings<TResult> settings, IMauiDialogContext? context, IDialogTheme theme)
            : base(settings, context, theme)
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
