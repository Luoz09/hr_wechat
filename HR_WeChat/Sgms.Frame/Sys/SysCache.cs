using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sgms.Frame.Sys
{
    /// <summary>
    /// 系统缓存
    /// </summary>
    public static class SysCache
    {
        /// <summary>
        /// DAL层程序集
        /// </summary>
        public static Dictionary<String, Assembly> DALAssemblyDic = new Dictionary<string, Assembly>();

        /// <summary>
        /// BLL层程序集
        /// </summary>
        public static Dictionary<String, Assembly> BLLAssemblyDic = new Dictionary<string, Assembly>();
    }
     
}
