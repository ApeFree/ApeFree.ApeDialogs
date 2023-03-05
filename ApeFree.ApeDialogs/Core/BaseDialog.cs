﻿using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Core
{
    public abstract class BaseDialog<TView, TOption, TContext, TResult> : IDialog<TResult>
    {
        protected readonly DialogSettings<TResult> Settings;

        protected BaseDialog(DialogSettings<TResult> settings)
        {
            Settings = settings;
        }

        public event DialogEventHandler Showing;
        public event DialogEventHandler Shown;
        public event DialogEventHandler Dismissing;
        public event DialogEventHandler Dismissed;

        public Result<TResult> Result { get; protected set; } = new Result<TResult>();
        public TContext Context { get; protected set; }
        public TView ContentView { get; set; }
        public abstract string Title { get; set; }
        public abstract string Content { get; set; }

        public virtual bool PerformPrecheck()
        {
            var result = Settings.PrecheckResult?.Invoke(Result.Data) ?? true;
            if (!result)
            {
                PrecheckFailsCallback();
                Result = new Result<TResult>();
            }
            return result;
        }

        /// <summary>
        /// 预处理未通过时执行的回调
        /// </summary>
        protected abstract void PrecheckFailsCallback();

        /// <summary>
        /// 设置选项
        /// </summary>
        /// <param name="text">选项名</param>
        /// <param name="onClick">单击动作</param>
        /// <returns></returns>
        public abstract TOption AddOption(DialogOption option, Action<IDialog, TOption> onClick = null);


        protected abstract TOption CreateOptionHandler(DialogOption option);

        /// <summary>
        /// 清空选项
        /// </summary>
        public abstract void ClearOptions();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Show()
        {
            RaiseShowing();
            ShowHandler();
            RaiseShown();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dismiss(bool isCancel)
        {
            Result.IsCancel = isCancel;

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
