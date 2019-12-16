using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Sys;
using System.Reflection;
using Sgms.Frame.Entities;
using System.Web;
using Sgms.Frame.Interface.DAL;

namespace Sgms.Frame.BLL
{
    /// <summary>
    /// 
    /// </summary>
    internal static class BLLHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IService<T> GetService<T>() where T : class,IEntity
        {
            var tType = typeof(T);
            String dalAssemblyName = tType.Assembly.FullName.Replace(SysKeys.SUFFIX_MODEL, SysKeys.SUFFIX_DAL);
            if (!SysCache.DALAssemblyDic.ContainsKey(dalAssemblyName))
            {
                SysCache.DALAssemblyDic.Add(dalAssemblyName, Assembly.Load(dalAssemblyName));

            }
            Assembly dalAssembly = SysCache.DALAssemblyDic[dalAssemblyName];

            /*
            if (SysPara.DALAssemblyName != String.Empty)
                {
                    SysCache.DALAssembly = Assembly.Load(SysPara.DALAssemblyName);
                }
                else
                {
                    SysCache.DALAssembly = Assembly.Load(tType.Assembly.FullName.Replace(SysKeys.SUFFIX_MODEL, SysKeys.SUFFIX_DAL));
                }
            }
            */
            var dalType= dalAssembly.GetTypes().FirstOrDefault(m => m.Name == typeof(T).Name + SysKeys.SUFFIX_DAL_CLASSNAME);
            return System.Activator.CreateInstance(dalType) as IService<T>;
        }
    }
}
