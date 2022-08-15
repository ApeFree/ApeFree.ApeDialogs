using ApeFree.ApeDialogs.Settings;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// Dialog提供器接口
    /// </summary>
    public abstract partial class DialogProvider<TDialog, TView, TContext> where TDialog : BaseDialog<TView,TContext>
    {
        public abstract TDialog CreateDialog(TContext context);
    }

    public abstract partial class DialogProvider<TDialog, TView, TContext>
    {
        protected abstract TView GenerateMessageView(MessageDialogSettings setings, TContext context);

        protected abstract TView GeneratePromptView(PromptDialogSettings setings, TContext context);

        protected abstract TView GenerateInputView(InputDialogSettings setings, TContext context);

        protected abstract TView GeneratePasswordView(PasswordDialogSettings setings, TContext context);

        protected abstract TView GenerateSelectionView(SelectionDialogSettings setings, TContext context);

        protected abstract TView GenerateMultipleSelectionView(MultipleSelectionDialogSettings setings, TContext context);

        protected abstract TView GenerateDataTimeView(DateTimeDialogSettings setings, TContext context);
    }

    public abstract partial class DialogProvider<TDialog, TView, TContext>
    {
        //public Result ShowMessageDialog(MessageDialogSettings setings, TContext context)
        //{
        //    var dialog = CreateDialog(context);
        //    dialog.ContentView = GenerateMessageView(setings, context);

        //    return dialog.Result;
        //}
    }
}
