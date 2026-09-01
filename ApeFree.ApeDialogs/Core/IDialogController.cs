namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// Dialog 控制器接口（实现方视角）。
    /// 用于实现方在用户交互完成后提交结果或关闭对话框。
    /// </summary>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public interface IDialogController<TResult> : IDialog<TResult>
    {
        /// <summary>
        /// 以指定结果关闭对话框。
        /// </summary>
        void Dismiss(DialogResult<TResult> result);

        /// <summary>
        /// 以取消结果关闭对话框。
        /// </summary>
        void Cancel();
    }
}
