using System;
using System.Collections.Generic;
using System.Linq;
using ApeFree.ApeDialogs.Core;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 数据录入表单（不可变集合；通过 <see cref="AddField"/> 追加字段）。
    /// </summary>
    public class DataEntrySheet
    {
        private static readonly object?[] EmptyObjects = new object?[0];
        private readonly List<SheetField> _fields = new();

        /// <summary>
        /// 字段列表。
        /// </summary>
        public IReadOnlyList<SheetField> Fields => _fields;

        /// <summary>
        /// 获取第 index 个字段。
        /// </summary>
        public SheetField this[int fieldIndex] => _fields[fieldIndex];

        /// <summary>
        /// 获取指定标题的字段；不存在时返回 null。
        /// </summary>
        public SheetField? this[string fieldTitle] => _fields.FirstOrDefault(field => field.Title == fieldTitle);

        /// <summary>
        /// 追加字段；标题重复时抛异常。
        /// </summary>
        public DataEntrySheet AddField(SheetField field)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (string.IsNullOrWhiteSpace(field.Title))
            {
                throw new ArgumentException("Field title cannot be empty.", nameof(field));
            }

            if (this[field.Title] is not null)
            {
                throw new ArgumentException($"A field with the title '{field.Title}' already exists in the sheet.", nameof(field));
            }

            _fields.Add(field);
            return this;
        }

        #region Text Field

        /// <summary>
        /// 添加文本字段。
        /// </summary>
        public DataEntrySheet AddTextField(TextField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加文本字段。
        /// </summary>
        public DataEntrySheet AddTextField(Action<TextField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new TextField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加文本字段。
        /// </summary>
        public DataEntrySheet AddTextField(string title, string? value = null, bool isMultiline = false, int maximumLength = int.MaxValue, Func<string?, ValidationResult>? validator = null)
        {
            return AddField(new TextField
            {
                Title = title,
                Value = value,
                IsMultiline = isMultiline,
                MaximumLength = maximumLength,
                Validator = validator,
            });
        }

        #endregion

        #region Password Field

        /// <summary>
        /// 添加密码字段。
        /// </summary>
        public DataEntrySheet AddPasswordField(PasswordField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加密码字段。
        /// </summary>
        public DataEntrySheet AddPasswordField(Action<PasswordField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new PasswordField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加密码字段。
        /// </summary>
        public DataEntrySheet AddPasswordField(string title, string? value = null, char passwordChar = '●', int maximumLength = 128, Func<string?, ValidationResult>? validator = null)
        {
            return AddField(new PasswordField
            {
                Title = title,
                Value = value,
                PasswordChar = passwordChar,
                MaximumLength = maximumLength,
                Validator = validator,
            });
        }

        #endregion

        #region Number Field

        /// <summary>
        /// 添加数值字段。
        /// </summary>
        public DataEntrySheet AddNumberField(NumberField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加数值字段。
        /// </summary>
        public DataEntrySheet AddNumberField(Action<NumberField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new NumberField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加数值字段。
        /// </summary>
        public DataEntrySheet AddNumberField(string title, decimal value = 0, decimal minimum = -1000000, decimal maximum = 1000000, int decimalPlaces = 1, Func<decimal, ValidationResult>? validator = null)
        {
            return AddField(new NumberField
            {
                Title = title,
                Value = value,
                Minimum = minimum,
                Maximum = maximum,
                DecimalPlaces = decimalPlaces,
                Validator = validator != null ? v => validator(v) : null,
            });
        }

        #endregion

        #region DateTime Field

        /// <summary>
        /// 添加日期时间字段。
        /// </summary>
        public DataEntrySheet AddDateTimeField(DateTimeField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加日期时间字段。
        /// </summary>
        public DataEntrySheet AddDateTimeField(Action<DateTimeField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new DateTimeField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加日期时间字段。
        /// </summary>
        public DataEntrySheet AddDateTimeField(string title, DateTime? value = null, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss", Func<DateTime, ValidationResult>? validator = null)
        {
            return AddField(new DateTimeField
            {
                Title = title,
                Value = value ?? DateTime.Now,
                DateTimeFormat = dateTimeFormat,
                Validator = validator != null ? v => validator(v) : null,
            });
        }

        #endregion

        #region File / Picture Path Fields

        /// <summary>
        /// 添加文件路径字段。
        /// </summary>
        public DataEntrySheet AddFilePathField(FilePathField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加文件路径字段。
        /// </summary>
        public DataEntrySheet AddFilePathField(Action<FilePathField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new FilePathField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加文件路径字段。
        /// </summary>
        public DataEntrySheet AddFilePathField(string title, string? value = null, string browseButtonText = "Browse...", Func<string?, ValidationResult>? validator = null)
        {
            return AddField(new FilePathField
            {
                Title = title,
                Value = value,
                BrowseButtonText = browseButtonText,
                Validator = validator,
            });
        }

        /// <summary>
        /// 添加图片路径字段。
        /// </summary>
        public DataEntrySheet AddPicturePathField(PicturePathField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加图片路径字段。
        /// </summary>
        public DataEntrySheet AddPicturePathField(Action<PicturePathField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new PicturePathField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加图片路径字段。
        /// </summary>
        public DataEntrySheet AddPicturePathField(string title, string? value = null, string browseButtonText = "Browse...", Func<string?, ValidationResult>? validator = null)
        {
            return AddField(new PicturePathField
            {
                Title = title,
                Value = value,
                BrowseButtonText = browseButtonText,
                Validator = validator,
            });
        }

        #endregion

        #region HexBytes Field

        /// <summary>
        /// 添加十六进制字节数组字段。
        /// </summary>
        public DataEntrySheet AddHexBytesField(HexBytesField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加十六进制字节数组字段。
        /// </summary>
        public DataEntrySheet AddHexBytesField(Action<HexBytesField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new HexBytesField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加十六进制字节数组字段。
        /// </summary>
        public DataEntrySheet AddHexBytesField(string title, byte[]? value = null, bool isMultiline = true, bool allowEmpty = false, int minimumLength = 0, int maximumLength = int.MaxValue, Func<byte[]?, ValidationResult>? validator = null)
        {
            return AddField(new HexBytesField
            {
                Title = title,
                Value = value,
                IsMultiline = isMultiline,
                AllowEmpty = allowEmpty,
                MinimumLength = minimumLength,
                MaximumLength = maximumLength,
                Validator = validator,
            });
        }

        #endregion

        #region Choice Fields

        /// <summary>
        /// 添加下拉框字段。
        /// </summary>
        public DataEntrySheet AddComboBoxField(ComboBoxField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加下拉框字段。
        /// </summary>
        public DataEntrySheet AddComboBoxField(Action<ComboBoxField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new ComboBoxField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加下拉框字段。
        /// </summary>
        public DataEntrySheet AddComboBoxField(string title, IReadOnlyList<object?> items, object? value = null, Func<object?, string>? itemDisplayConverter = null, Func<object?, ValidationResult>? validator = null)
        {
            return AddField(new ComboBoxField
            {
                Title = title,
                Items = items ?? EmptyObjects,
                Value = value,
                ItemDisplayConverter = itemDisplayConverter ?? (item => item?.ToString() ?? string.Empty),
                Validator = validator,
            });
        }

        /// <summary>
        /// 添加单选字段。
        /// </summary>
        public DataEntrySheet AddSingleChoiceField(SingleChoiceField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加单选字段。
        /// </summary>
        public DataEntrySheet AddSingleChoiceField(Action<SingleChoiceField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new SingleChoiceField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加单选字段。
        /// </summary>
        public DataEntrySheet AddSingleChoiceField(string title, IReadOnlyList<object?> items, object? value = null, Func<object?, string>? itemDisplayConverter = null, Func<object?, ValidationResult>? validator = null)
        {
            return AddField(new SingleChoiceField
            {
                Title = title,
                Items = items ?? EmptyObjects,
                Value = value,
                ItemDisplayConverter = itemDisplayConverter ?? (item => item?.ToString() ?? string.Empty),
                Validator = validator,
            });
        }

        /// <summary>
        /// 添加多选字段。
        /// </summary>
        public DataEntrySheet AddMultipleChoiceField(MultipleChoiceField field) => AddField(field);

        /// <summary>
        /// 通过配置委托添加多选字段。
        /// </summary>
        public DataEntrySheet AddMultipleChoiceField(Action<MultipleChoiceField> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            var field = new MultipleChoiceField();
            configure(field);
            return AddField(field);
        }

        /// <summary>
        /// 添加多选字段。
        /// </summary>
        public DataEntrySheet AddMultipleChoiceField(string title, IReadOnlyList<object?> items, IReadOnlyList<object?>? value = null, Func<object?, string>? itemDisplayConverter = null, Func<IReadOnlyList<object?>?, ValidationResult>? validator = null)
        {
            return AddField(new MultipleChoiceField
            {
                Title = title,
                Items = items ?? EmptyObjects,
                Value = value,
                ItemDisplayConverter = itemDisplayConverter ?? (item => item?.ToString() ?? string.Empty),
                Validator = validator,
            });
        }

        #endregion

        /// <summary>
        /// 校验所有字段，返回第一个失败结果。
        /// </summary>
        public ValidationResult Validate()
        {
            foreach (var field in _fields)
            {
                var result = field.Validate();

                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            return ValidationResult.Success;
        }
    }
}
