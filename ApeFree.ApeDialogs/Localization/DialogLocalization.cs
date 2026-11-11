using System;
using System.Globalization;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// 提供 ApeDialogs 内置的中英文自适应文本与本地化支持。
    /// </summary>
    public static class DialogLocalization
    {
        /// <summary>
        /// 当前是否为中文语言环境。
        /// </summary>
        public static bool IsChinese => CultureInfo.CurrentUICulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase);

        /// <summary>确认按钮文本</summary>
        public static string Confirm => IsChinese ? "确定" : "Confirm";

        /// <summary>取消按钮文本</summary>
        public static string Cancel => IsChinese ? "取消" : "Cancel";

        /// <summary>重置按钮文本</summary>
        public static string Reset => IsChinese ? "重置" : "Reset";

        /// <summary>关闭按钮文本</summary>
        public static string Close => IsChinese ? "关闭" : "Close";

        /// <summary>必填字段为空提示</summary>
        public static string RequiredFieldEmpty => IsChinese ? "该字段不允许为空。" : "This field cannot be empty.";

        /// <summary>数值超出范围提示</summary>
        public static string ValueOutOfRange(object min, object max) => IsChinese ? $"数值必须在 [{min}, {max}] 范围内。" : $"Value must be between {min} and {max}.";

        /// <summary>文本长度超出限制提示</summary>
        public static string TextLengthExceeded(int max) => IsChinese ? $"文本长度不能超过 {max} 个字符。" : $"Text length cannot exceed {max} characters.";

        /// <summary>密码长度超出限制提示</summary>
        public static string PasswordLengthExceeded(int max) => IsChinese ? $"密码长度不能超过 {max} 个字符。" : $"Password length cannot exceed {max} characters.";

        /// <summary>字节数据不能为空提示</summary>
        public static string ByteDataEmpty => IsChinese ? "字节数据不能为空。" : "Byte data cannot be empty.";

        /// <summary>字节长度小于最小值提示</summary>
        public static string ByteLengthLessThan(int min) => IsChinese ? $"字节长度不能小于 {min}。" : $"Byte length cannot be less than {min}.";

        /// <summary>字节长度超出最大值提示</summary>
        public static string ByteLengthExceeded(int max) => IsChinese ? $"字节长度不能超过 {max}。" : $"Byte length cannot exceed {max}.";

        /// <summary>十六进制格式无效提示</summary>
        public static string InvalidHexFormat => IsChinese ? "十六进制格式无效。" : "Invalid hex format.";

        /// <summary>十六进制长度必须为偶数提示</summary>
        public static string HexLengthMustBeEven => IsChinese ? "十六进制长度必须为偶数。" : "Hex string length must be an even number.";

        /// <summary>包含无效十六进制字符提示</summary>
        public static string HexContainsInvalidChars => IsChinese ? "包含无效的十六进制字符。" : "Contains invalid hexadecimal characters.";

        /// <summary>日期格式无效提示</summary>
        public static string InvalidDateFormat => IsChinese ? "日期格式无效。" : "Invalid date format.";

        /// <summary>预期日期格式提示</summary>
        public static string DateFormatExpected(string format) => IsChinese ? $"日期格式应为 {format}。" : $"Date format must be {format}.";

        /// <summary>数值格式无效提示</summary>
        public static string InvalidNumberFormat => IsChinese ? "数值格式无效。" : "Invalid number format.";
    }
}
