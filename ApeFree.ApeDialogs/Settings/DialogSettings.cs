using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog选项
    /// </summary>
    public abstract class DialogSettings<TResult>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 提示语内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否允许取消
        /// </summary>
        public bool Cancelable { get; set; }

        /// <summary>
        /// 取消选项文本
        /// </summary>
        public string CancelOptionText { get; set; } = "Cancel";

        /// <summary>
        /// 配色风格
        /// </summary>
        public ColorStyle ColorStyle { get; set; }

        /// <summary>
        /// 获取对话框的选项信息
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<DialogOption> GetOptionsHandler();

        public IEnumerable<DialogOption> GetOptions()
        {
            return GetOptionsHandler().GetValidItems();
        }

        /// <summary>
        /// 返回结果预校验委托
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="result">待检查值</param>
        /// <returns></returns>
        // public delegate bool PrecheckResultHandler<TResult>(TResult result);

        /// <summary>
        /// 返回结果预校验
        /// </summary>
        public Func<TResult, bool> PrecheckResult { get; set; }
    }

    /// <summary>
    /// 配色风格
    /// </summary>
    public enum ColorStyle
    {
        /// <summary>
        /// 明亮模式
        /// </summary>
        Bright,

        /// <summary>
        /// 夜间模式
        /// </summary>
        Night,

        /// <summary>
        /// 柔光模式
        /// </summary>
        // SoftLight,
    }
}
