using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Core
{
    public abstract class BaseDialog<TView, TContext, TResult> : BaseDialog<TView, TContext>
    {
        public new Result<TResult> Result
        {
            get => (Result<TResult>)base.Result;
            protected set => base.Result = value;
        }
    }

    public abstract class BaseDialog<TView, TContext> : IDialog<TView, TContext>
    {
        public event DialogEventHandler Showing;
        public event DialogEventHandler Shown;
        public event DialogEventHandler Dismissing;
        public event DialogEventHandler Dismissed;

        public Result Result { get; protected set; }

        public TContext Context { get; protected set; }

        public TView ContentView { get; set; }

        /// <summary>
        /// 设置选项
        /// </summary>
        /// <param name="options"></param>
        protected abstract void SetOptions(IEnumerable<DialogOption> options);

        public void Show()
        {
            RaiseShowing();
            ShowHandler();
            RaiseShown();
        }

        public void Dismiss()
        {
            RaiseDismissing();
            DismissHandler();
            RaiseDismissed();
        }

        /// <summary>
        /// Dialog显示的实现
        /// </summary>
        protected abstract void ShowHandler();

        /// <summary>
        /// Dialog销毁的实现
        /// </summary>
        protected abstract void DismissHandler();

        protected void RaiseShowing()
        {
            Showing?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseShown()
        {
            Shown?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseDismissing()
        {
            Dismissing?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseDismissed()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
