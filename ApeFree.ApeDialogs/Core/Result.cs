using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ApeDialogs.Core
{
    /// <summary>
    /// Dialog返回结果
    /// </summary>
    public class Result
    {
        public bool IsCancel { get; }

        public object Data { get; }

        public Result(bool isCancel, object data = null)
        {
            IsCancel = isCancel;
            Data = data;
        }
    }

    /// <summary>
    /// Dialog返回结果
    /// </summary>
    public class Result<T> : Result
    {
        public new T Data => (T)base.Data;
        public Result() : base(true, null) { }
        public Result(T data) : base(false, data) { }
        public Result(bool isCancel, T data) : base(isCancel, data) { }
    }
}
