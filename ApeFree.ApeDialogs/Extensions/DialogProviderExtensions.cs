using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApeFree.ApeDialogs
{
    public static class DialogProviderExtensions
    {

        public static IDialog<bool> CreateMessageDialog<TContext>(this DialogProvider<TContext> provider, Action<MessageDialogSettings> settingsHandler, Action<IDialog<bool>> dialogHandler = null, TContext context = default)
        {
            return provider.CreateDialog("CreateMessageDialog", null, settingsHandler, dialogHandler, context);
        }

        public static IDialog<bool> CreatePromptDialog<TContext>(this DialogProvider<TContext> provider, Action<PromptDialogSettings> settingsHandler, Action<IDialog<bool>> dialogHandler = null, TContext context = default)
        {
            return provider.CreateDialog("CreatePromptDialog", null, settingsHandler, dialogHandler, context);
        }

        public static IDialog<string> CreateInputDialog<TContext>(this DialogProvider<TContext> provider, Action<InputDialogSettings> settingsHandler, Action<IDialog<string>> dialogHandler = null, TContext context = default)
        {
            return provider.CreateDialog("CreateInputDialog", null, settingsHandler, dialogHandler, context);
        }

        public static IDialog<string> CreatePasswordDialog<TContext>(this DialogProvider<TContext> provider, Action<PasswordDialogSettings> settingsHandler, Action<IDialog<string>> dialogHandler = null, TContext context = default)
        {
            return provider.CreateDialog("CreatePasswordDialog", null, settingsHandler, dialogHandler, context);
        }

        public static IDialog<TItem> CreateSelectionDialog<TItem, TContext>(this DialogProvider<TContext> provider, IEnumerable<TItem> collection, TItem defaultSelectedItem, Action<SelectionDialogSettings<TItem>> settingsHandler, Action<IDialog<TItem>> dialogHandler = null, TContext context = default)
        {
            var settings = new SelectionDialogSettings<TItem>();
            // 通过回调交由业务层完善参数
            settingsHandler?.Invoke(settings);
            var dialog = provider.CreateSelectionDialog(settings, collection, defaultSelectedItem, context);
            dialogHandler?.Invoke(dialog);
            return dialog;
        }
        public static IDialog<IEnumerable<TItem>> CreateMultipleSelectionDialog<TItem, TContext>(this DialogProvider<TContext> provider, IEnumerable<TItem> collection, IEnumerable<TItem> defaultSelectedItems, Action<MultipleSelectionDialogSettings<TItem>> settingsHandler, Action<IDialog<IEnumerable<TItem>>> dialogHandler = null, TContext context = default)
        {
            var settings = new MultipleSelectionDialogSettings<TItem>();
            // 通过回调交由业务层完善参数
            settingsHandler?.Invoke(settings);
            var dialog = provider.CreateMultipleSelectionDialog(settings, collection, defaultSelectedItems, context);
            dialogHandler?.Invoke(dialog);
            return dialog;
        }

        public static IDialog<bool> CreateDateTimeDialog<TContext>(this DialogProvider<TContext> provider, Action<MessageDialogSettings> settingsHandler, Action<IDialog<bool>> dialogHandler = null, TContext context = default)
        {
            return provider.CreateDialog("CreateDateTimeDialog", null, settingsHandler, dialogHandler, context);
        }

        internal static IDialog<TResult> CreateDialog<TSettings, TResult, TContext>(this DialogProvider<TContext> provider, string methodName, IEnumerable<object> attachedParams, Action<TSettings> settingsHandler, Action<IDialog<TResult>> dialogHandler = null, TContext context = default) where TSettings : DialogSettings<TResult>
        {
            // 构造Dialog Settings对象
            TSettings settings = Activator.CreateInstance<TSettings>();
            settings.OptionTagColors = provider.OptionTagColors;
            // 通过回调交由业务层完善参数
            settingsHandler?.Invoke(settings);
            // 按照传入的参数名称查找方法
            var mi = provider.GetType().GetMethod(methodName);
            // 获取方法所需参数的类型集合
            var methodParamTypes = mi.GetParameters().Select(pi => pi.ParameterType);
            // 整理可提供参数列表
            var paramList = new List<object>(attachedParams ?? new object[0]) { settings, context }.Where(item => item != null);
            // 对齐参数
            var orderedParams = methodParamTypes.Select(pt => paramList.FirstOrDefault(p => p.GetType().IsAssignableFrom(pt))).ToArray();

            // 利用反射执行方法
            IDialog<TResult> dialog = (IDialog<TResult>)mi.Invoke(provider, orderedParams);
            // 通过回调交由业务层完善参数
            dialogHandler?.Invoke(dialog);

            return dialog;
        }
    }
}
