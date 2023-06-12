using ApeFree.ApeDialogs.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.ApeDialogs.Settings
{
    public static class DialogOptionsExtensions
    {
        /// <summary>
        /// 获取有效选项（所有文本非空的选项）
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<DialogOption> GetValidItems(this IEnumerable<DialogOption> options)
        {
            return options.Where(o=>!string.IsNullOrWhiteSpace(o.Text));
        }
    }
}
