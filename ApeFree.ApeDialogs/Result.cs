using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ApeDialogs
{
    /// <summary>
    /// Dialog返回结果
    /// </summary>
    public class Result: Result<object>
    { 

    }

    /// <summary>
    /// Dialog返回结果
    /// </summary>
    public class Result<T>
    {
        public bool IsCancel { get; }

        public T Data { get; set; }
    }
}
