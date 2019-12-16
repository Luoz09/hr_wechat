using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Sys
{
    /// <summary>
    /// 系统消息
    /// </summary>
    public class SysMessage
    {
        /// <summary>
        /// 是否有消息
        /// </summary>
        public bool HasMessage
        {
            get { return MessageList.Count != 0; }
        }

        private List<string> MessageList = new List<string>();

        /// <summary>
        /// 追加系统消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public SysMessage Append(string message)
        {
            MessageList.Add(message);
            return this;
        }

        /// <summary>
        /// 追加系统消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public SysMessage Append(Exception e)
        {
            int i = 0;
            var curE = e;
            while (curE.InnerException != null && i < 10)
            {
                curE = curE.InnerException;
                i++;
            }
            return Append(curE.Message);
        }

        /// <summary>
        /// 系统消息合并
        /// </summary>
        /// <param name="sysMessage"></param>
        /// <returns></returns>
        public SysMessage Union(SysMessage sysMessage)
        {
            MessageList.AddRange(sysMessage.MessageList);
            return this;
        }

        /// <summary>
        /// 转成以&lt;br /&gt;隔开的消息
        /// </summary>
        /// <returns></returns>
        public string ToHtmlString()
        {
            return ToString("<br />");
        }

        /// <summary>
        /// 转成以空格隔开的消息
        /// </summary>
        /// <returns></returns>
        public string ToOnelineString()
        {
            return ToString(" ");
        }

        /// <summary>
        /// 转成以/r/n隔开的消息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("\r\n");
        }

        /// <summary>
        /// 转成以自定义分隔符隔开的消息
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public string ToString(string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var elem in MessageList)
            {
                sb.Append(elem);
                sb.Append(separator);
            }
            return sb.ToString().Trim();
        }
    }
}
