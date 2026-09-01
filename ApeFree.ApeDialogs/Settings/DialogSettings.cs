using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// Dialog 通用配置。
    /// </summary>
    public abstract class DialogSettings
    {
        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// 提示语内容。
        /// </summary>
        public string? Content { get; init; }

        /// <summary>
        /// 是否允许取消（如是否显示关闭按钮、Esc 是否生效）。
        /// </summary>
        public bool Cancelable { get; init; } = true;

        /// <summary>
        /// 配色主题；为空时使用 <see cref="DefaultTheme"/>。
        /// </summary>
        public IDialogTheme? Theme { get; init; }

        /// <summary>
        /// 建议尺寸；为空时由实现方决定默认尺寸。
        /// </summary>
        public ApeSize? DialogSize { get; init; }

        /// <summary>
        /// 默认字体；为空时由实现方决定默认字体。
        /// </summary>
        public DialogFont? Font { get; init; }

        /// <summary>
        /// 自动关闭时长；设置后对话框在显示满该时长时以取消结果自动关闭。
        /// </summary>
        public TimeSpan? Duration { get; init; }

        /// <summary>
        /// 显示完成后的回调。
        /// </summary>
        public Action<IDialog>? OnShown { get; init; }

        /// <summary>
        /// 销毁完成后的回调。
        /// </summary>
        public Action<IDialog>? OnDismissed { get; init; }

        /// <summary>
        /// 默认选项集合。
        /// </summary>
        public virtual IReadOnlyList<DialogOption> DefaultOptions => new DialogOption[0];
    }

    /// <summary>
    /// 带返回结果的 Dialog 通用配置。
    /// </summary>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public abstract class DialogSettings<TResult> : DialogSettings
    {
        /// <summary>
        /// 结果校验规则；提交结果时执行，失败则关闭前拦截并回调实现方。
        /// </summary>
        public Func<TResult, ValidationResult>? Validator { get; init; }

        /// <summary>
        /// 值实时变化回调（调节类 Dialog，如滑块）。
        /// 仅用于观察/实时影响外界；取消 Dialog 并不会回滚已推送的值。
        /// </summary>
        public Action<TResult>? OnValueChanged { get; init; }
    }
}
