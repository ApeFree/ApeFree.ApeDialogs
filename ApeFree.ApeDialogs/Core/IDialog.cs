using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Core
{
    public delegate void DialogEventHandler(IDialog dialog, EventArgs e);

    /// <summary>
    /// 对话框标准接口
    /// </summary>
    public interface IDialog<TResult> : IDialog
    {
        Result<TResult> Result { get; }
    }

    /// <summary>
    /// 对话框标准接口
    /// </summary>
    public interface IDialog
    {
        event DialogEventHandler Showing;
        event DialogEventHandler Shown;
        event DialogEventHandler Dismissing;
        event DialogEventHandler Dismissed;

        /// <summary>
        /// 显示Dialog
        /// </summary>
        void Show();

        /// <summary>
        /// 销毁Dialog
        /// </summary>
        void Dismiss();
    }
}
