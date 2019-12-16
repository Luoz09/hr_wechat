using Sgms.Frame.Rights.Entities;
using Sgms.Frame.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR.Web
{
    public  static class  ClientSysPara
    {


        private static string _TmpClientSessionName = null;
        /// <summary>
        /// 管理员Session名 CurAdmin 需要用到这个变量
        /// </summary>
        public static string ClientSessionName
        {
            get { return _TmpClientSessionName ?? "caclient" + SysPara.GUIDKey; }
        }
        /// <summary>
        /// 当前登录管理员
        /// </summary>
        public static Sgms.Frame.Rights.Entities.IUser CurClient
        {
            get
            {
                var result = HttpContext.Current.Session[ClientSessionName] as IUser;

                /*
                //重新登录
                if (result == null)
                {
                    var loginUrl = AppSettingUtil.GetAppSetting("LoginUrl").ToString();
                    var redirectUrl = HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.ToString());
                    loginUrl += "?RedirectUrl=" + redirectUrl;
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.BufferOutput = true;//设置输出缓冲
                    if (!HttpContext.Current.Response.IsRequestBeingRedirected)//在跳转之前做判断,防止重复
                    {
                        HttpContext.Current.Response.Redirect(loginUrl);
                    }


                }
                */

                /* if (result == null)
                 {
                     var cookie = HttpContext.Current.Request.Cookies[AdminSessionName];
                     if (cookie == null)
                     {
                         return result;
                     }
                     /var cookieStr = cookie.Value;

                     result = JsonConvert.DeserializeObject(cookieStr) as IUser;
                 }*/
                /*if (result == null)
                {
                    var rightsDataObtain = TypeUtil.CreateInstance<IRightsDataObtain>(SysPara.RightsDataObtainClassInfo);
                    rightsDataObtain.Login();
                    result = HttpContext.Current.Session[AdminSessionName] as IUser;
                }*/

                return result;
            }
            set
            {
                HttpContext.Current.Session[ClientSessionName] = value;
                // HttpContext.Current.Response.Cookies.Add(new HttpCookie(AdminSessionName, JsonConvert.SerializeObject(value)));
            }
        }

    }
}