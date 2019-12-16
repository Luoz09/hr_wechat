using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Utils
{
    public abstract class OrderUtil
    {
        private static object _lock = new object();
        private static int count = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="appID">应用标识</param>
        /// <returns></returns>
        public static string GetOrder(string prefix, string appID = "0")
        {
            lock (_lock)
            {
                if (count >= 1000)
                {
                    count = 1;
                }
                var number = new StringBuilder(prefix).Append(DateTime.Now.ToString("yyMMddHHmmss")).Append(appID).Append(count.ToString("000")).ToString();
                count++;
                return number;
            }
        }
    }
}
