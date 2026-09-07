using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 长文本查看对话框配置（日志、许可协议、帮助文档等）。
    /// <see cref="DialogSettings.Content"/> 为简要说明，<see cref="Text"/> 为正文。
    /// </summary>
    public class TextDialogSettings : DialogSettings<bool>
    {
        /// <summary>
        /// 正文文本（长文本，滚动显示）。
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 是否只读。
        /// </summary>
        public bool IsReadOnly { get; set; } = true;

        /// <summary>
        /// 是否自动换行。
        /// </summary>
        public bool WordWrap { get; set; } = true;

        /// <summary>
        /// 是否允许选择/复制文本。
        /// </summary>
        public bool IsSelectable { get; set; } = true;

        /// <summary>
        /// 关闭选项。
        /// </summary>
        public DialogOption CloseOption { get; set; } = new("Close", DialogOptionTag.Neutral);

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { CloseOption };
    }
}
