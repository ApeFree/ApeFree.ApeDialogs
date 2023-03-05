using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    public class MultipleSelectionDialogSettings<T> : DialogSettings<IEnumerable<T>>
    {
        /// <summary>
        /// 确认选项
        /// </summary>
        public DialogOption ConfirmOption { get; set; } = new DialogOption("Confirm", DialogOptionTag.Positive);

        /// <summary>
        /// 取消选项
        /// </summary>
        public DialogOption CancelOption { get; set; } = new DialogOption("Cancel", DialogOptionTag.Negative, callback: DefaultCancelOptionHandler);

        /// <summary>
        /// 全选选项
        /// </summary>
        public DialogOption SelectAllOption { get; set; } = new DialogOption("All", DialogOptionTag.Functional);

        /// <summary>
        /// 反选选项
        /// </summary>
        public DialogOption ReverseSelectedOption { get; set; } = new DialogOption("Reverse", DialogOptionTag.Functional);

        /// <summary>
        /// 选项显示文本转换回调
        /// </summary>
        public Func<T, string> ItemDisplayTextConvertCallback { get; set; } = (item) => item.ToString();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Dialog默认选项集合</returns>
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { ConfirmOption, CancelOption, SelectAllOption, ReverseSelectedOption };
    }
}
