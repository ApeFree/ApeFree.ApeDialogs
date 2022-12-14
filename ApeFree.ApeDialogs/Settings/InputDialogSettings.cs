using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class InputDialogSettings : DialogSettings<string>
    {
        /// <summary>
        /// 确认选项文本
        /// </summary>
        public string ConfirmOptionText { get; set; } = "Confirm";

        /// <summary>
        /// 清空选项文本
        /// </summary>
        public string ClearOptionText { get; set; } = "Clear";

        /// <summary>
        /// 允许输入内容为空
        /// </summary>
        public bool AllowEmpty { get; set; }

        /// <summary>
        /// 最大输入长度
        /// </summary>
        public byte MaximumLength { get; set; }

        /// <summary>
        /// 最小输入长度
        /// </summary>
        public byte MinimumLength { get; set; }

        /// <summary>
        /// 是否多行
        /// </summary>
        public bool IsMultiline { get; set; }

        public override IEnumerable<DialogOption> GetOptionsHandler()
        {
            return new List<DialogOption>() {
                new DialogOption(ClearOptionText, DialogOptionType.Functional),
                new DialogOption(ConfirmOptionText, DialogOptionType.Positive),
                new DialogOption(CancelOptionText, DialogOptionType.Negative,Cancelable),
            };
        }
    }
}
