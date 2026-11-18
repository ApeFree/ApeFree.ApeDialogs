using System;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Hosting
{
    /// <summary>
    /// MAUI 对话框上下文接口。
    /// </summary>
    public interface IMauiDialogContext
    {
        /// <summary>
        /// 关联的目标页面。
        /// </summary>
        Page? TargetPage { get; }
    }

    /// <summary>
    /// 基于 Page 的 MAUI 对话框上下文实现。
    /// </summary>
    public class MauiDialogContext : IMauiDialogContext
    {
        public MauiDialogContext(Page? targetPage)
        {
            TargetPage = targetPage;
        }

        public Page? TargetPage { get; }

        public static implicit operator MauiDialogContext(Page page) => new(page);
    }
}
