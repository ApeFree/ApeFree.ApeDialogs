using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// Dialog返回结果
    /// </summary>
    public class Result<T>
    {
        public bool IsCancel { get; internal set; }
        public T Data { get; private set; }

        public Result() : this(true) { }
        public Result(T data) : this(false, data) { }
        private Result(bool isCancel, T data = default)
        {
            IsCancel = isCancel;
            Data = data;
        }

        /// <summary>
        /// 更新结果数据
        /// </summary>
        /// <param name="value"></param>
        public void UpdateResultData(T value)
        {
            Data = value;
        }
    }
}
