using System.Collections.Generic;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 颜色选择对话框配置。
    /// </summary>
    public class ColorDialogSettings : DialogSettings<ApeColor>
    {
        /// <summary>
        /// 默认颜色。
        /// </summary>
        public ApeColor DefaultColor { get; set; } = new(255, 255, 255);

        /// <summary>
        /// 是否允许调节 Alpha 通道。
        /// </summary>
        public bool AllowAlpha { get; set; }

        /// <summary>
        /// 预设颜色（快捷选择）。
        /// </summary>
        public IReadOnlyList<ApeColor> PresetColors { get; set; } = new ApeColor[0];

        /// <summary>
        /// 确认选项。
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项。
        /// </summary>
        public DialogOption CancelOption { get; set; } = DialogOption.CreateCancelOption();

        /// <inheritdoc/>
        public override IReadOnlyList<DialogOption> DefaultOptions => new[] { ConfirmOption, CancelOption };
    }
}
