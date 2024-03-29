﻿using System;
using System.Collections.Generic;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 选择Dialog设置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectionDialogSettings<T> : DialogSettings<T>
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
        /// 选项显示文本转换回调
        /// </summary>
        public Func<T, string> ItemDisplayTextConvertCallback { get; set; } = (item) => item.ToString();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Dialog默认选项集合</returns>
        protected override IEnumerable<DialogOption> GetDefaultOptionsHandler() => new DialogOption[] { ConfirmOption, CancelOption };
    }
}
