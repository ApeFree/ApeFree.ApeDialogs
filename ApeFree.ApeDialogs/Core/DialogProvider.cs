using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// Dialog提供器接口
    /// </summary>
    public abstract partial class DialogProvider<TDialog, TView, TContext> where TDialog : IDialog<TView, TContext>
    {
        public abstract BaseDialog<TView, TContext, TResult> CreateDialog<TResult>(TContext context, TView view = default(TView));
    }

    public abstract partial class DialogProvider<TDialog, TView, TContext>
    {
        protected abstract TView GenerateMessageView(MessageDialogSettings setings, TContext context);

        protected abstract TView GeneratePromptView(PromptDialogSettings setings, TContext context);

        protected abstract TView GenerateInputView(InputDialogSettings setings, TContext context);

        protected abstract TView GeneratePasswordView(PasswordDialogSettings setings, TContext context);

        protected abstract TView GenerateSelectionView(SelectionDialogSettings setings, TContext context);

        protected abstract TView GenerateMultipleSelectionView(MultipleSelectionDialogSettings setings, TContext context);

        protected abstract TView GenerateDateTimeView(DateTimeDialogSettings setings, TContext context);
    }

    public abstract partial class DialogProvider<TDialog, TView, TContext>
    {
        /// <summary>
        /// 创建消息对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, object> CreateMessageDialog(MessageDialogSettings setings, TContext context)
        {
            return CreateDialog<object>(context, GenerateMessageView(setings, context));
        }

        /// <summary>
        /// 创建提示对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, bool> CreatePromptDialog(PromptDialogSettings setings, TContext context)
        {
            return CreateDialog<bool>(context, GeneratePromptView(setings, context));
        }

        /// <summary>
        /// 创建输入对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, string> CreateInputDialog(InputDialogSettings setings, TContext context)
        {
            return CreateDialog<string>(context, GenerateInputView(setings, context));
        }

        /// <summary>
        /// 创建密码对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, string> CreatePasswordDialog(PasswordDialogSettings setings, TContext context)
        {
            return CreateDialog<string>(context, GeneratePasswordView(setings, context));
        }

        /// <summary>
        /// 创建单选对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, T> CreateSelectionDialog<T>(SelectionDialogSettings setings, IEnumerable<T> collection, T defaultSelectedItem, TContext context)
        {
            return CreateDialog<T>(context, GenerateSelectionView(setings, context));
        }

        /// <summary>
        /// 创建多选对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, IEnumerable<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings setings, IEnumerable<T> collection, IEnumerable<T> defaultSelectedItems, TContext context)
        {
            return CreateDialog<IEnumerable<T>>(context, GenerateMultipleSelectionView(setings, context));
        }

        /// <summary>
        /// 创建日期选择对话框
        /// </summary>
        /// <param name="setings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public BaseDialog<TView, TContext, DateTime> CreateDateTimeDialog(DateTimeDialogSettings setings, TContext context)
        {
            return CreateDialog<DateTime>(context, GenerateDateTimeView(setings, context));
        }


    }
}
