﻿using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog选项
    /// </summary>
    public abstract class DialogSettings
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
        public string CancelOptionText { get; set; }

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
        public delegate bool PrecheckResultHandler<TResult>(TResult result);
    }
}