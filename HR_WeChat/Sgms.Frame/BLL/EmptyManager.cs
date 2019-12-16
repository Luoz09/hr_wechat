using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Interface.BLL;
using Sgms.Frame.Interface.DAL;
using Sgms.Frame.Sys;
using System.Reflection;

namespace Sgms.Frame.BLL
{
    public class EmptyManager : IEmptyManager
    {
        public Sys.SysMessage RunMessage
        {
            get { return Service.RunMessage; }
        }

        protected IEmptyService TmpService;
        /// <summary>
        /// 数据访问层的实例
        /// </summary>
        protected virtual IEmptyService Service
        {
            get
            {
                if (TmpService == null)
                {
                    var tType = this.GetType();
                    string dalAssemblyName = tType.Assembly.FullName.Replace(SysKeys.SUFFIX_BLL, SysKeys.SUFFIX_DAL);
                    if (!SysCache.DALAssemblyDic.ContainsKey(dalAssemblyName))
                    {
                        /*
                        if (SysPara.DALAssemblyName != String.Empty)
                        {
                            SysCache.DALAssembly = Assembly.Load(SysPara.DALAssemblyName);
                        }
                        else
                        {
                            SysCache.DALAssembly = Assembly.Load(tType.Assembly.FullName.Replace(SysKeys.SUFFIX_BLL, SysKeys.SUFFIX_DAL));
                        }
                        */
                        SysCache.DALAssemblyDic.Add(dalAssemblyName, Assembly.Load(dalAssemblyName));
                    }
                    Assembly dalAssembly = SysCache.DALAssemblyDic[dalAssemblyName];
                    var dalType = dalAssembly.GetTypes().FirstOrDefault(m => m.Name == tType.Name.Replace(SysKeys.SUFFIX_BLL_CLASSNAME, SysKeys.SUFFIX_DAL_CLASSNAME));
                    TmpService = System.Activator.CreateInstance(dalType) as IEmptyService;
                }
                return TmpService;
            }
        }
    }
}
