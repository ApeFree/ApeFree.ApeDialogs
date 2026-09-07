using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// Dialog 标准接口（消费者视角）。
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// 显示前触发。
        /// </summary>
        event EventHandler<DialogEventArgs>? Showing;

        /// <summary>
        /// 显示完成后触发。
        /// </summary>
        event EventHandler<DialogEventArgs>? Shown;

        /// <summary>
        /// 销毁前触发。
        /// </summary>
        event EventHandler<DialogEventArgs>? Dismissing;

        /// <summary>
        /// 销毁完成后触发。
        /// </summary>
        event EventHandler<DialogEventArgs>? Dismissed;

        /// <summary>
        /// 是否正在显示。
        /// </summary>
        bool IsShown { get; }

        /// <summary>
        /// 取消/关闭对话框。
        /// </summary>
        void Cancel();
    }

    /// <summary>
    /// 带返回结果的 Dialog 标准接口。
    /// </summary>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public interface IDialog<TResult> : IDialog
    {
        /// <summary>
        /// 异步显示 Dialog 并等待其结束。
        /// 用户取消或 token 取消时返回 <see cref="DialogResult{T}.Canceled"/>，不抛异常。
        /// </summary>
        /// <param name="cancellationToken">取消等待（同时会关闭对话框）</param>
        Task<DialogResult<TResult>> ShowAsync(CancellationToken cancellationToken = default);
    }
}
