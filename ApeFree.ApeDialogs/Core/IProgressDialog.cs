using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// 进度 Dialog 接口（消费者视角）。
    /// 显示期间由调用方推进进度，完成后 <see cref="IDialog{T}.ShowAsync"/> 返回是否成功完成。
    /// </summary>
    public interface IProgressDialog : IDialog<bool>
    {
        /// <summary>
        /// 是否是不确定进度（转圈动画）。
        /// </summary>
        bool IsIndeterminate { get; }

        /// <summary>
        /// 更新进度。
        /// </summary>
        /// <param name="progress">进度值，范围 0.0 ~ 1.0</param>
        /// <param name="message">进度消息（如"正在解压…"），null 表示不更新</param>
        void UpdateProgress(double progress, string? message = null);
    }
}
