using System;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// Dialog 的返回结果（不可变值类型）。
    /// 用户取消或 CancellationToken 取消时，均返回 <see cref="Canceled"/>，不会抛异常。
    /// </summary>
    /// <typeparam name="T">结果数据类型</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "工厂方法提升可读性，且因 T 有默认取消实例（Canceled）无需静态字段缓存。")]
    public readonly record struct DialogResult<T>(T? Value, bool IsCanceled)
    {
        /// <summary>
        /// 构造一个确认结果。
        /// </summary>
        public static DialogResult<T> Confirmed(T value) => new(value, false);

        /// <summary>
        /// 构造一个取消结果。
        /// </summary>
        public static DialogResult<T> Canceled => new(default, true);

        /// <summary>
        /// 是否已确认（即未被取消）。
        /// </summary>
        public bool IsConfirmed => !IsCanceled;

        /// <summary>
        /// 取消时抛出 <see cref="InvalidOperationException"/>，否则返回结果值。
        /// </summary>
        public T RequireValue()
        {
            if (IsCanceled)
            {
                throw new InvalidOperationException("The dialog result is canceled, no value available.");
            }

            return Value!;
        }

        /// <summary>
        /// 按结果状态分支处理。
        /// </summary>
        public TOut Match<TOut>(Func<T, TOut> onConfirmed, Func<TOut> onCanceled)
        {
            if (onConfirmed is null) { throw new ArgumentNullException(nameof(onConfirmed)); }
            if (onCanceled is null) { throw new ArgumentNullException(nameof(onCanceled)); }

            return IsCanceled ? onCanceled() : onConfirmed(Value!);
        }

        /// <summary>
        /// 按结果状态分支执行无返回值动作。
        /// </summary>
        public void Match(Action<T> onConfirmed, Action onCanceled)
        {
            if (onConfirmed is null) { throw new ArgumentNullException(nameof(onConfirmed)); }
            if (onCanceled is null) { throw new ArgumentNullException(nameof(onCanceled)); }

            if (IsCanceled)
            {
                onCanceled();
            }
            else
            {
                onConfirmed(Value!);
            }
        }
    }
}
