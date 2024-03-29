﻿using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 数据录入表单
    /// </summary>
    public class DataEntrySheet
    {
        /// <summary>
        /// 字段列表
        /// </summary>
        public List<SheetField> Fields { get; }

        public DataEntrySheet()
        {
            Fields = new List<SheetField>();
        }

        /// <summary>
        /// 获取字段列表中第index个字段
        /// </summary>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public SheetField this[int fieldIndex] => Fields[fieldIndex];

        /// <summary>
        /// 获取字段列表中指定名称的字段
        /// </summary>
        /// <param name="fieldTitle"></param>
        /// <returns></returns>
        public SheetField this[string fieldTitle] => Fields.FirstOrDefault(x => x.Title == fieldTitle);


        public void AddField(SheetField field)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (this[field.Title] != null)
            {
                throw new ArgumentException("An item with the same field title already exists in the sheet.");
            }

            Fields.Add(field);
        }

        public void AddTextField(TextField field) => AddField(field);

        public void AddPasswordField(PasswordField field) => AddField(field);

        public void AddFilePathField(FilePathField field) => AddField(field);

        public void AddNumberField(NumberField field) => AddField(field);

        public void AddSingleChoiceField(SingleChoiceField field) => AddField(field);

        public void AddComboBoxField(ComboBoxField field) => AddField(field);

        public void AddMultipleChoiceField(MultipleChoiceField field) => AddField(field);

        public void AddDateTimeField(DateTimeField field) => AddField(field);

        public void AddPicturePathField(PicturePathField field) => AddField(field);
    }

    /// <summary>
    /// 字段类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,

        /// <summary>
        /// 密码文本
        /// </summary>
        Password,

        /// <summary>
        /// 16进制字节数组
        /// </summary>
        HexBytes,

        /// <summary>
        /// 文件路径
        /// </summary>
        FilePath,

        /// <summary>
        /// 图片文件路径
        /// </summary>
        PicturePath,

        /// <summary>
        /// 数值
        /// </summary>
        Number,

        /// <summary>
        /// 下拉列表
        /// </summary>
        ComboBox,

        /// <summary>
        /// 单选
        /// </summary>
        SingleChoice,

        /// <summary>
        /// 多选
        /// </summary>
        MultipleChoice,

        /// <summary>
        /// 日期
        /// </summary>
        DateTime,
    }

    /// <summary>
    /// 表单字段
    /// </summary>
    public abstract class SheetField
    {
        public abstract FieldType FieldType { get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 执行数据有效性检查
        /// </summary>
        /// <returns></returns>
        public abstract FormatCheckResult ValidityCheck();
    }

    /// <summary>
    /// 表单字段
    /// </summary>
    public abstract class SheetField<TData> : SheetField
    {
        /// <summary>
        /// 数据
        /// </summary>
        public new TData Data { get => (TData)base.Data; set => base.Data = value; }

        /// <summary>
        /// 数据有效性检查过程
        /// </summary>
        public Func<TData, FormatCheckResult> ValidityCheckHandler { get; set; }

        public override FormatCheckResult ValidityCheck()
        {
            if (ValidityCheckHandler == null)
            {
                return FormatCheckResult.Success;
            }

            return ValidityCheckHandler.Invoke(Data);
        }
    }

    /// <summary>
    /// 文本类型的表单字段
    /// </summary>
    public class TextField : SheetField<string>
    {
        public override FieldType FieldType => FieldType.Text;

        public int MaximumLength { get; set; } = int.MaxValue;
        public bool IsMultiline { get; set; }
    }

    /// <summary>
    /// 密码类型的表单字段
    /// </summary>
    public class PasswordField : SheetField<string>
    {
        public override FieldType FieldType => FieldType.Password;

        public int MaximumLength { get; set; } = 128;
        public char PasswordChar { get; set; } = '●';
    }

    /// <summary>
    /// Hex格式的字节数组类型的表单字段
    /// </summary>
    public class HexBytesField : SheetField<byte[]>
    {
        public override FieldType FieldType => FieldType.HexBytes;

        public bool AllowEmpty { get; set; }
        public int MaximumLength { get; set; } = int.MaxValue;
        public int MinimumLength { get; set; } = 0;
        public bool IsMultiline { get; set; } = true;
    }

    /// <summary>
    /// 文件路径类型的表单字段
    /// </summary>
    public class FilePathField : SheetField<string>
    {
        public override FieldType FieldType => FieldType.FilePath;

        public string BrowseButtonText { get; set; } = "Browse...";
    }

    public class PicturePathField : FilePathField
    {
        public override FieldType FieldType => FieldType.PicturePath;
    }

    /// <summary>
    /// 数值类型的表单字段
    /// </summary>
    public class NumberField : SheetField<decimal>
    {
        public override FieldType FieldType => FieldType.Number;

        /// <summary>
        /// 保留小数的位数
        /// </summary>
        public int DecimalPlaces { get; set; } = 1;

        /// <summary>
        /// 最大值
        /// </summary>
        public decimal Maximum { get; set; } = 1000000;

        /// <summary>
        /// 最小值
        /// </summary>
        public decimal Minimum { get; set; } = -1000000;
    }

    /// <summary>
    /// 单选类型的表单字段
    /// </summary>
    public class SingleChoiceField : SheetField<object>
    {
        public override FieldType FieldType => FieldType.SingleChoice;

        public object[] Items { get; set; }
        public Func<object, string> ItemDisplayTextConvertHandler { get; set; } = defaultItemDisplayTextConvertHandle;

        private static string defaultItemDisplayTextConvertHandle(object item) => item.ToString();
    }

    /// <summary>
    /// 单选类型的表单字段
    /// </summary>
    public class ComboBoxField : SingleChoiceField
    {
        public override FieldType FieldType => FieldType.ComboBox;
    }

    /// <summary>
    /// 多选类型的表单字段
    /// </summary>
    public class MultipleChoiceField : SheetField<object[]>
    {
        public override FieldType FieldType => FieldType.MultipleChoice;
        public MultipleChoiceField()
        {
            Data = new object[0];
        }

        public object[] Items { get; set; }
        public Func<object, string> ItemDisplayTextConvertHandler { get; set; } = defaultItemDisplayTextConvertHandle;

        private static string defaultItemDisplayTextConvertHandle(object item) => item.ToString();
    }

    /// <summary>
    /// 日期类型的表单字段
    /// </summary>
    public class DateTimeField : SheetField<DateTime>
    {
        public override FieldType FieldType => FieldType.DateTime;

        public string DateTimeFormat { get; set; } = "yyyy-MM-dd hh:mm:ss";
    }

    /// <summary>
    /// 格式检查结果
    /// </summary>
    public struct FormatCheckResult
    {
        /// <summary>
        /// 校验成功的结果
        /// </summary>
        public readonly static FormatCheckResult Success = new FormatCheckResult(true, null);

        public bool IsSuccess { get; }
        public string ErrorMessage { get; } = string.Empty;

        public FormatCheckResult(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public FormatCheckResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }

}
