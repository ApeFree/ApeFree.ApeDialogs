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
