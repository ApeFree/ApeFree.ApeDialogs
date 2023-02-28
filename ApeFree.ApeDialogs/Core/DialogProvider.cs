using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs
{
    //public static class DialogProviderExtensions
    //{
    //    public static IDialog<bool> CreateMessageDialog<TContext>(this DialogProvider<TContext> provider, Action<MessageDialogSettings> settingsHandler, TContext context)
    //    {
    //        MessageDialogSettings settings = new MessageDialogSettings();
    //        settingsHandler.Invoke(settings);
    //        return provider.CreateMessageDialog(settings, context);
    //    }
    //}

    /// <summary>
    /// Dialog提供器接口
    /// </summary>
    public abstract partial class DialogProvider<TContext>
    {
        /// <summary>
        /// 创建消息对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, TContext context);

        /// <summary>
        /// 创建提示对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, TContext context);

        /// <summary>
        /// 创建输入对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<string> CreateInputDialog(InputDialogSettings settings, TContext context);

        /// <summary>
        /// 创建密码对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, TContext context);

        /// <summary>
        /// 创建单选对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem, TContext context);

        /// <summary>
        /// 创建多选对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<IEnumerable<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T> defaultSelectedItems, TContext context);

        /// <summary>
        /// 创建日期选择对话框
        /// </summary>
        /// <param name="settings">配置参数</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, TContext context);
    }
}
