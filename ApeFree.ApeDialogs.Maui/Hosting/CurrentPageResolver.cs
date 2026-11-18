using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Hosting
{
    /// <summary>
    /// 当前活动页面解析器。
    /// </summary>
    public static class CurrentPageResolver
    {
        /// <summary>
        /// 解析当前正在展示的活动页面。
        /// </summary>
        public static Page? ResolveCurrentPage(IMauiDialogContext? context = null)
        {
            if (context?.TargetPage != null)
            {
                return context.TargetPage;
            }

            // 1. 尝试从 Shell.Current 获取
            if (Shell.Current is not null)
            {
                var shellCurrent = Shell.Current.CurrentPage;
                if (shellCurrent != null)
                {
                    return GetTopPage(shellCurrent);
                }
            }

            // 2. 尝试从 Application.Current.Windows 获取
            var currentApp = Application.Current;
            if (currentApp != null && currentApp.Windows.Count > 0)
            {
                var activeWindow = currentApp.Windows[0];
                if (activeWindow.Page != null)
                {
                    return GetTopPage(activeWindow.Page);
                }
            }

            return null;
        }

        private static Page GetTopPage(Page root)
        {
            var current = root;

            while (true)
            {
                // 如果存在 Modal 页面栈，Modal 页面处于最顶层
                if (current.Navigation != null && current.Navigation.ModalStack.Count > 0)
                {
                    current = current.Navigation.ModalStack[current.Navigation.ModalStack.Count - 1];
                    continue;
                }

                if (current is NavigationPage navPage && navPage.CurrentPage != null)
                {
                    current = navPage.CurrentPage;
                    continue;
                }

                if (current is TabbedPage tabbedPage && tabbedPage.CurrentPage != null)
                {
                    current = tabbedPage.CurrentPage;
                    continue;
                }

                if (current is FlyoutPage flyoutPage && flyoutPage.Detail != null)
                {
                    current = flyoutPage.Detail;
                    continue;
                }

                break;
            }

            return current;
        }
    }
}
