using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// Dialog提供器接口
    /// </summary>
    public abstract partial class DialogProvider<TView, TContext>
    {
        /// <summary>
        /// 创建消息对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, object> CreateMessageDialog(MessageDialogSettings setings, TContext context);

        /// <summary>
        /// 创建提示对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, bool> CreatePromptDialog(PromptDialogSettings setings, TContext context);

        /// <summary>
        /// 创建输入对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, string> CreateInputDialog(InputDialogSettings setings, TContext context);
        
        /// <summary>
        /// 创建密码对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, string> CreatePasswordDialog(PasswordDialogSettings setings, TContext context);

        /// <summary>
        /// 创建单选对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, T> CreateSelectionDialog<T>(SelectionDialogSettings setings, IEnumerable<T> collection, T defaultSelectedItem, TContext context);

        /// <summary>
        /// 创建多选对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, IEnumerable<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings setings, IEnumerable<T> collection, IEnumerable<T> defaultSelectedItems, TContext context);

        /// <summary>
        /// 创建日期选择对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract BaseDialog<TView, TContext, DateTime> CreateDateTimeDialog(DateTimeDialogSettings setings, TContext context);
    }
}
