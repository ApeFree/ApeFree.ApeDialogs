using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 进度对话框配置。显示期间由调用方通过 <see cref="Core.IProgressDialog"/> 推进进度。
    /// </summary>
    public class ProgressDialogSettings : DialogSettings<bool>
    {
        /// <summary>
        /// 是否是不确定进度（转圈动画）。
        /// </summary>
        public bool IsIndeterminate { get; init; }

        /// <summary>
        /// 初始进度值（范围 0.0 ~ 1.0）。
        /// </summary>
        public double InitialProgress { get; init; }

        /// <summary>
        /// 初始进度消息。
        /// </summary>
        public string? InitialMessage { get; init; }

        /// <summary>
        /// 取消选项（仅 <see cref="DialogSettings.Cancelable"/> 为 true 时展示）。
        /// </summary>
        public DialogOption CancelOption { get; init; } = DialogOption.CreateCancelOption();

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { CancelOption };
    }
}
