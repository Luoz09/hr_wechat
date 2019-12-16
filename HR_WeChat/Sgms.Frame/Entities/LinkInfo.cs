using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Sys;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 超链接信息
    /// </summary>
    public class LinkInfo
    {
        /// <summary>
        /// 默认目标 值：_self
        /// </summary>
        public static string DefaultTarget = "_self";

        private string _Target = DefaultTarget;

        /// <summary>
        /// 超链接文本
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// 超链接地址
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// 默认 DefaultTarget   DefaultTarget的默认值"_self"
        /// </summary>
        public string Target { get { return _Target; } set { _Target = value; } }
    }
}
