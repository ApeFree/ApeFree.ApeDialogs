using System;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// 数据有效性校验结果（不可变值类型）。
    /// </summary>
    public readonly record struct ValidationResult(bool IsSuccess, string? ErrorMessage)
    {
        /// <summary>
        /// 校验成功的结果。
        /// </summary>
        public static ValidationResult Success => new(true, null);

        /// <summary>
        /// 构造一个校验失败的结果。
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public static ValidationResult Failure(string errorMessage)
        {
            if (errorMessage is null) { throw new ArgumentNullException(nameof(errorMessage)); }
            return new ValidationResult(false, errorMessage);
        }

        /// <summary>
        /// 构造一个校验失败的结果（<see cref="Failure"/> 的别名）。
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public static ValidationResult Failed(string errorMessage) => Failure(errorMessage);

        public override string ToString() => IsSuccess ? "Success" : ErrorMessage ?? "Failed";
    }
}
