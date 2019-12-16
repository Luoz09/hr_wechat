using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sgms.Frame.Page.MVC;
using System.Web.Mvc;
using Sgms.Frame.Interface.UI;
using Sgms.Frame;
using System.Collections;

namespace HR.Web.Areas.WebApi.Codes
{
    public class WebApiNormalPage : NormalPage
    {

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
    }
}