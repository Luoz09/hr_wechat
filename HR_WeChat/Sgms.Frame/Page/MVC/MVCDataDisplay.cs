using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sgms.Frame.Sys;
using System.Web;
using System.Collections;
using Sgms.Frame.Interface.UI;

namespace Sgms.Frame.Page.MVC
{
    /// <summary>
    /// 
    /// </summary>
    public class MVCDataDisplay : IMVCDataDisplay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public virtual System.Web.Mvc.ContentResult DisplayJson(string json)
        {
            ContentResult result = new ContentResult();
            result.ContentType = "application/json";
            result.ContentEncoding = Encoding.UTF8;
            result.Content = json;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual System.Web.Mvc.ContentResult DisplayJson(IEnumerable<object> data)
        {
            return DisplayJson(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual System.Web.Mvc.ContentResult DisplayJson(object obj)
        {
            return DisplayJson(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual System.Web.Mvc.ContentResult DisplayJson(bool isSuccess, string msg)
        {
            return DisplayJson("{\"Success\":" + (isSuccess ? "true" : "false") + ",\"Message\":\"" + msg.Replace("\n", "").Replace("\r", "").Replace("\"", "\\\"") + "\"}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual System.Web.Mvc.ContentResult DisplayText(string text)
        {
            ContentResult result = new ContentResult();
            result.ContentType = "text/plain";
            result.ContentEncoding = Encoding.UTF8;
            result.Content = text;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void DisplayNoRightsPage()
        {
            /* HttpContext.Current.Response.Clear(); //清除前台输出的内容
             HttpContext.Current.Response.StatusCode = 401; //输出401状态码
             HttpContext.Current.Server.Execute("/401.html");  //调用执行错误页面
             HttpContext.Current.Response.End();*/
            HttpContext.Current.Response.Redirect(SysPara.NoRightsPageName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual ContentResult DisplayDataOfTotal(IEnumerable<object> data)
        {
            if (!(data is IList) && !(data is Array))
            {
                data = data.ToArray();
            }
            return DisplayJson(JsonConvert.SerializeObject(new
            {
                total = data.Count(),
                rows = data
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual ContentResult DisplayDataOfTotal(IEnumerable data, int count)
        {
            return DisplayJson(JsonConvert.SerializeObject(new
            {
                total = count,
                rows = data
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ContentResult DisplayNoRightsJson()
        {
            return DisplayJson(false, SysLang.GetWords(SysLang.NO_RIGHTS));
        }
    }
}
