using System;
using System.Collections.Generic;
using System.Linq;
using ApeFree.ApeDialogs.Core;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 字段类型。
    /// </summary>
    public enum FieldType : byte
    {
        Text,
        Password,
        HexBytes,
        FilePath,
        PicturePath,
        Number,
        ComboBox,
        SingleChoice,
        MultipleChoice,
        DateTime,
    }

    /// <summary>
    /// 表单字段基类。
    /// </summary>
    public abstract class SheetField
    {
        /// <summary>
        /// 字段类型。
        /// </summary>
        public abstract FieldType FieldType { get; }

        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 获取原始值。
        /// </summary>
        public abstract object? GetRawValue();

        /// <summary>
        /// 设置原始值。
        /// </summary>
        public abstract void SetRawValue(object? value);

        /// <summary>
        /// 执行数据有效性检查。
        /// </summary>
        public abstract ValidationResult Validate();
    }

    /// <summary>
    /// 带强类型值的表单字段。
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public abstract class SheetField<T> : SheetField
    {
        private T? _value;

        /// <summary>
        /// 字段值。
        /// </summary>
        public T? Value { get => _value; set => _value = value; }

        /// <summary>
        /// 数据有效性检查过程。
        /// </summary>
        public Func<T?, ValidationResult>? Validator { get; set; }

        /// <inheritdoc/>
        public override object? GetRawValue() => _value;

        /// <inheritdoc/>
        public override void SetRawValue(object? value) => _value = value is null ? default : (T)value;

        /// <inheritdoc/>
        public override ValidationResult Validate() => Validator?.Invoke(_value) ?? ValidationResult.Success;
    }

    /// <summary>
    /// 文本类型的表单字段。
    /// </summary>
    public class TextField : SheetField<string>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.Text;

        /// <summary>
        /// 最大长度（UI 约束提示）。
        /// </summary>
        public int MaximumLength { get; set; } = int.MaxValue;

        /// <summary>
        /// 是否多行。
        /// </summary>
        public bool IsMultiline { get; set; }
    }

    /// <summary>
    /// 密码类型的表单字段。
    /// </summary>
    public class PasswordField : SheetField<string>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.Password;

        /// <summary>
        /// 最大长度（UI 约束提示）。
        /// </summary>
        public int MaximumLength { get; set; } = 128;

        /// <summary>
        /// 密码遮掩字符。
        /// </summary>
        public char PasswordChar { get; set; } = '●';
    }

    /// <summary>
    /// Hex 格式的字节数组类型的表单字段。
    /// </summary>
    public class HexBytesField : SheetField<byte[]>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.HexBytes;

        /// <summary>
        /// 是否允许为空。
        /// </summary>
        public bool AllowEmpty { get; set; }

        /// <summary>
        /// 最大长度（UI 约束提示）。
        /// </summary>
        public int MaximumLength { get; set; } = int.MaxValue;

        /// <summary>
        /// 最小长度（UI 约束提示）。
        /// </summary>
        public int MinimumLength { get; set; }

        /// <summary>
        /// 是否多行。
        /// </summary>
        public bool IsMultiline { get; set; } = true;
    }

    /// <summary>
    /// 文件路径类型的表单字段。
    /// </summary>
    public class FilePathField : SheetField<string>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.FilePath;

        /// <summary>
        /// 浏览按钮文本。
        /// </summary>
        public string BrowseButtonText { get; set; } = "Browse...";
    }

    /// <summary>
    /// 图片文件路径类型的表单字段。
    /// </summary>
    public class PicturePathField : FilePathField
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.PicturePath;
    }

    /// <summary>
    /// 数值类型的表单字段。
    /// </summary>
    public class NumberField : SheetField<decimal>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.Number;

        /// <summary>
        /// 保留小数的位数（UI 约束提示）。
        /// </summary>
        public int DecimalPlaces { get; set; } = 1;

        /// <summary>
        /// 最大值。
        /// </summary>
        public decimal Maximum { get; set; } = 1000000;

        /// <summary>
        /// 最小值。
        /// </summary>
        public decimal Minimum { get; set; } = -1000000;
    }

    /// <summary>
    /// 单选类型的表单字段。
    /// </summary>
    public class SingleChoiceField : SheetField<object?>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.SingleChoice;

        /// <summary>
        /// 可选项集合。
        /// </summary>
        public IReadOnlyList<object?> Items { get; set; } = new object?[0];

        /// <summary>
        /// 选项显示文本转换器。
        /// </summary>
        public Func<object?, string> ItemDisplayConverter { get; set; } = item => item?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// 下拉列表类型的表单字段。
    /// </summary>
    public class ComboBoxField : SingleChoiceField
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.ComboBox;
    }

    /// <summary>
    /// 多选类型的表单字段。
    /// </summary>
    public class MultipleChoiceField : SheetField<IReadOnlyList<object?>>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.MultipleChoice;

        /// <summary>
        /// 可选项集合。
        /// </summary>
        public IReadOnlyList<object?> Items { get; set; } = new object?[0];

        /// <summary>
        /// 选项显示文本转换器。
        /// </summary>
        public Func<object?, string> ItemDisplayConverter { get; set; } = item => item?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// 日期时间类型的表单字段。
    /// </summary>
    public class DateTimeField : SheetField<DateTime>
    {
        /// <inheritdoc/>
        public override FieldType FieldType => FieldType.DateTime;

        /// <summary>
        /// 日期时间格式（24 小时制）。
        /// </summary>
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
    }
}
