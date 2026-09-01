using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 轻提示（Toast）对话框配置。
    /// 默认按 <see cref="DialogSettings.Duration"/> 自动消失（默认 3 秒）；被点击时以确认结果返回。
    /// 调用方通常不等待该 Task（fire-and-forget）。
    /// </summary>
    public class ToastDialogSettings : DialogSettings<bool>
    {
        public ToastDialogSettings()
        {
            Duration = TimeSpan.FromSeconds(3);
        }

        /// <summary>
        /// 是否响应点击（点击后立即以确认结果关闭）。
        /// </summary>
        public bool IsClickable { get; init; } = true;

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new DialogOption[0];
    }
}
