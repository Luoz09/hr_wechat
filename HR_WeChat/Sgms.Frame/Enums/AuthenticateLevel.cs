using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sgms.Frame
{
    /// <summary>
    /// 认证级别
    /// </summary>
    [XmlRoot]
    public enum AuthenticateLevel
    {
        /// <summary>
        /// 不认证
        /// </summary>
        [XmlEnum]
        None,
        /// <summary>
        /// 简单认证 就认证用户是否登录
        /// </summary>
        [XmlEnum]
        Simple,
        /// <summary>
        /// 深入的认证 认证权限
        /// </summary>
        [XmlEnum]
        InDepth
    }
}
