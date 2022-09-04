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
        public abstract IDialog<bool> CreateMessageDialog(MessageDialogSettings setings, TContext context);

        /// <summary>
        /// 创建提示对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<bool> CreatePromptDialog(PromptDialogSettings setings, TContext context);

        /// <summary>
        /// 创建输入对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<string> CreateInputDialog(InputDialogSettings setings, TContext context);

        /// <summary>
        /// 创建密码对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<string> CreatePasswordDialog(PasswordDialogSettings setings, TContext context);

        /// <summary>
        /// 创建单选对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> setings, IEnumerable<T> collection, T defaultSelectedItem, TContext context);

        /// <summary>
        /// 创建多选对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<IEnumerable<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> setings, IEnumerable<T> collection, IEnumerable<T> defaultSelectedItems, TContext context);

        /// <summary>
        /// 创建日期选择对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings setings, TContext context);
    }
}
