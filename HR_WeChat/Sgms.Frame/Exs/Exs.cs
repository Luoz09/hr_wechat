using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Exs
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class Ex
    {
        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="str">要替换的字符串</param>
        /// <param name="oldValue">要替换的内容</param>
        /// <param name="newValue">替换成的内容</param>
        /// <param name="times">替换几次</param>
        /// <returns>替换的结果</returns>
        public static string Replace(this string str, string oldValue, string newValue, int times)
        {
            int indexOf = str.IndexOf(oldValue);
            if (indexOf == -1) return str;
            string pre = str.Substring(0, indexOf);
            int endOfOldValue = indexOf + oldValue.Length;
            string end = str.Substring(endOfOldValue, str.Length - endOfOldValue);
            string result = pre + newValue + end;
            if (times == 1)
            {
                return result;
            }
            else
            {
                return result.Replace(oldValue, newValue, times - 1);
            }
        }
    }
}
