using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs
{

    /// <summary>
    /// 对话框标准接口
    /// </summary>
    public interface IDialog
    {
        void Show();
        void Dismiss();
        void Finish();
    }

    public abstract class BaseDialog<TView, TContext, TResult> : BaseDialog<TView, TContext>
    {
        public Result<TResult> Result { get; }
    }

    public abstract class BaseDialog<TView, TContext> : IDialog
    {
        public delegate void DialogEventHandler(IDialog dialog, EventArgs e);

        public event DialogEventHandler Showing;
        public event DialogEventHandler Shown;
        public event DialogEventHandler Dismissing;
        public event DialogEventHandler Dismissed;

        public TContext Context { get; }

        public TView ContentView { get; set; }

        /// <summary>
        /// 设置选项
        /// </summary>
        /// <param name="options"></param>
        protected abstract void SetOptions(IEnumerable<DialogOption> options);

        public abstract void Show();
        public abstract void Dismiss();
        public abstract void Finish();
    }
}
