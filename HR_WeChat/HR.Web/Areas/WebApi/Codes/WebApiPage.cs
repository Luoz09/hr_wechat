using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sgms.Frame.Page.MVC;
using System.Web.Mvc;
using Sgms.Frame.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Sgms.Frame;

using Newtonsoft.Json.Converters;
using Sgms.Frame.Sys;
using Sgms.Frame.Interface.BLL;
using Sgms.Frame.Interface.UI;
using System.Collections;
using Sgms.Frame.BLL;

namespace HR.Web.Areas.WebApi.Codes
{
    public class WebApiPage<T> : DataAccessPage<T> where T : class,IEntity
    {
        /*private IManager<T> _TmpManager;
        protected override IManager<T> Manager
        {
            get
            {
                if (_TmpManager == null)
                {
                    Type thisType = this.GetType();
                    Assembly bllAssemblyName;
                    if (SysPara.DALAssemblyName != String.Empty)
                    {
                        bllAssemblyName = Assembly.Load(SysPara.DALAssemblyName);
                    }
                    else
                    {
                        bllAssemblyName = Assembly.Load("Jida.AirportDispatch.BLL");
                    }
                    var bllTypeName = thisType.Name.Replace("Controller", SysKeys.SUFFIX_BLL_CLASSNAME);
                    _TmpManager = (IOperationManager<T>)bllAssemblyName.CreateInstance(bllAssemblyName.GetTypes().FirstOrDefault(m => m.Name == bllTypeName).FullName);
                }
                return _TmpManager;
            }
        }*/

        private OperationManager<T> _TmpCurManager;
        private OperationManager<T> CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as OperationManager<T>;
                }
                return _TmpCurManager;
            }
        }

        private IMVCDataDisplay _TmpDataDisplay;
        protected override IMVCDataDisplay DataDisplay
        {
            get
            {
                if (_TmpDataDisplay == null)
                {
                    _TmpDataDisplay = new MVCWebApiDataDisplay();
                }
                return _TmpDataDisplay;
            }
        }

        protected override Sgms.Frame.AuthenticateLevel CurAuthenticateLevel
        {
            get
            {
                return AuthenticateLevel.None;
            }
        }

        /*[HttpPost]
        public virtual ContentResult GetData(FormCollection collection)
        {
            string msg = String.Empty;
            return DisplayJson(CurManager.GetData(ref msg), msg);
        }

        [HttpPost]
        public override ContentResult GetEntity(string id = null)
        {
            string msg = String.Empty;
            var type = Request.QueryString["type"];
            if (String.IsNullOrEmpty(type))
            {
                return DisplayJson(CurManager.GetEntity(ref msg), msg);
            }
            else
            {
                var method = GetType().GetMethod("GetEntityBy" + type, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (method == null)
                {
                    msg = "非法调用";
                    return DisplayJson(false, msg);
                }
                var param = new object[] { msg };
                try
                {
                    var result = method.Invoke(this, param);
                    msg = param[0].ToString();
                    return DisplayJson(result, msg);
                }
                catch (Exception e)
                {
                    msg = "调用时发送错误原因为:" + e.Message;
                    return DisplayJson(false, msg);
                }
            }
        }*/

        /// <summary>
        /// 显示JSON字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ContentResult DisplayJson(IEnumerable<object> data, string msg)
        {
            if (data == null)
            {
                return DataDisplay.DisplayJson(false, msg == String.Empty ? "没有相关数据" : msg);
                //return DataDisplay.DisplayJson(false, msg);
            }
            else
            {
                return DataDisplay.DisplayJson(data);
            }
        }

        /// <summary>
        /// 显示JSON字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ContentResult DisplayJson(object obj, string msg)
        {
            if (obj == null)
            {
                return DataDisplay.DisplayJson(false, msg == String.Empty ? "没有相关数据" : msg);
            }
            else
            {
                return DataDisplay.DisplayJson(obj);
            }
        }

        public new ContentResult DisplayJson(bool isSuccess, string msg)
        {
            return DataDisplay.DisplayJson(isSuccess, msg);
        }

        protected ContentResult DisplayNonsupport()
        {
            return DisplayJson(false, "不支持此操作");
        }

        protected ContentResult DisplaySystemError()
        {
            return DisplayJson(false, "系统错误");
        }

        /*/// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public override ContentResult GetData()
        {
            string msg = String.Empty;
            IEnumerable data = null;
            var type = Request.QueryString["type"];
            if (String.IsNullOrEmpty(type))
            {
                try
                {
                    data = CurManager.GetData();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                }
            }
            else  //带Type参数的GetData
            {
                var method = GetType().GetMethod("GetDataBy" + type, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (method == null)
                {
                    DisplayJson(false, "非法调用");
                    return null;
                }

                var param = new object[] { msg };
                try
                {
                    var result = method.Invoke(this, param) as IEnumerable<object>;
                    msg = param[0].ToString();
                    DisplayJson(result, msg);
                }
                catch (Exception e)
                {
                    DisplayJson(false, "调用时发送错误原因为:" + e.Message);
                    return null;
                }
            }
            return DisplayJson(data, msg);
        }*/
    }
}