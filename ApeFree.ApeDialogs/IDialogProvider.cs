using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// 这里要用扩展方法代替
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IDialogMethods<TContext>
    {
        Result ShowMessageDialog(MessageDialogSettings setings, TContext context);
        Result<bool> ShowPromptDialog(PromptDialogSettings setings, TContext context);
        Result<string> ShowInputDialog(InputDialogSettings setings, TContext context);
        Result<string> ShowPasswordDialog(PasswordDialogSettings setings, TContext context);
        Result<T> ShowSelectionDialog<T>(SelectionDialogSettings setings, IEnumerable<T> collection, T defaultSelectedItem, TContext context);
        Result<IEnumerable<T>> ShowMultipleSelectionDialog<T>(SelectionDialogSettings setings, IEnumerable<T> collection, IEnumerable<T> defaultSelectedItems, TContext context);
    }
}
