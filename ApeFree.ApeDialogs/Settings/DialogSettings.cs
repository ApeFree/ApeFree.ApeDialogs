using System;
using System.Collections.Generic;
using System.Drawing;

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
        /// 配色风格
        /// </summary>
        public ColorStyle ColorStyle { get; set; }

        /// <summary>
        /// 对话框建议尺寸
        /// </summary>
        public Size? DialogSize { get; set; }

        /// <summary>
        /// 获取对话框的选项信息
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<DialogOption> GetDefaultOptionsHandler();

        public IEnumerable<DialogOption> GetOptions()
        {
            return GetDefaultOptionsHandler().GetValidItems();
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
        public Func<TResult, FormatCheckResult> PrecheckResult { get; set; }

        /// <summary>
        /// 选项标签颜色表
        /// </summary>
        public Dictionary<DialogOptionTag, Color> OptionTagColors { get; internal set; }// = new Dictionary<DialogOptionTag, Color>();

        /// <summary>
        /// 默认的取消选项处理过程
        /// </summary>
        internal static readonly Action<object, OptionSelectedEventArgs> DefaultCancelOptionHandler = (s, e) => e.Dialog.Dismiss(true);
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
