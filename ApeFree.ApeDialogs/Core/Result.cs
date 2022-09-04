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
        public T Data { get; }

        public Result() : this(true) { }
        public Result(T data) : this(false, data) { }
        private Result(bool isCancel, T data = default(T))
        {
            IsCancel = isCancel;
            Data = data;
        }
    }
}
