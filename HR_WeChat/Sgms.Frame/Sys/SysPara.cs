using System.Xml.Serialization;
using Sgms.Frame.Entities;
using System.IO;
using System.Web;
using Sgms.Frame.Utils;
using Sgms.Frame.Rights.Entities;
using Sgms.Frame.Rights;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Sgms.Frame.Sys
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public static class SysPara
    {
        private static SysConfig _SysConfigEntity;

        /// <summary>
        /// 业务逻辑层程序集名
        /// </summary>
        public static string BLLAssemblyName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.BLLAssemblyName;
            }
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.ConnectionString;
            }
        }

        /// <summary>
        /// 数据访问层程序集名
        /// </summary>
        public static string DALAssemblyName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.DALAssemblyName;
            }
        }

        /// <summary>
        /// 默认认证级别
        /// </summary>
        public static AuthenticateLevel DefaultAuthenticateLevel
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.DefaultAuthenticateLevel;
            }
        }

        /// <summary>
        /// 进去的欢迎页面
        /// </summary>
        public static string DesktopPage
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.DesktopPage;
            }
        }

        /// <summary>
        /// GUIDKey
        /// </summary>
        public static string GUIDKey
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.GUIDKey;
            }
        }

        /// <summary>
        /// 语言
        /// </summary>
        public static string Lang
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.Lang;
            }
        }

        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public static string LoginPageName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.LoginPageName;
            }
        }
        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public static string PCLoginPageName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.PCLoginPageName;
            }
        }
        public static string MobileLoginPageName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.MobileLoginPageName;
            }
        }
        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public static string RebackUrl
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.RebackUrl;
            }
        }
        /// <summary>
        /// 没权限页面
        /// </summary>
        public static string NoRightsPageName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.NoRightsPageName;
            }
        }

        /// <summary>
        /// 日志路径
        /// </summary>
        public static string LogPath
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.LogPath;
            }
        }

        /// <summary>
        /// 提示页面文件名
        /// </summary>
        public static string PromptTemplatePage
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.PromptTemplatePage;
            }
        }

        /// <summary>
        /// 模板路径
        /// </summary>
        public static string TemplateName
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.TemplateName;
            }
        }

        /// <summary>
        /// 上传文件路径
        /// </summary>
        public static string UploadsPath
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.UploadsPath;
            }
        }

        /// <summary>
        /// 权限数据获取类信息
        /// </summary>
        public static string RightsDataObtainClassInfo
        {
            get { return _SysConfigEntity.RightsDataObtainClassInfo; }
        }

        /// <summary>
        /// 允许登录失败次数
        /// </summary>
        public static int AllowLoginFailureTimes
        {
            get { return _SysConfigEntity.AllowLoginFailureTimes; }
        }

        /// <summary>
        /// 登录出错时间间隔单位 分钟
        /// </summary>
        public static int LoginFailureInterval
        {
            get { return _SysConfigEntity.LoginFailureInterval; }
        }

        /// <summary>
        /// 允许操作的时间差 0为不限制
        /// </summary>
        public static int AllowOperaTimeDiff
        {
            get
            {
                LoadConfig();
                return _SysConfigEntity.AllowOperaTimeDiff;
            }
        }

        private static string _TmpAdminSessionName = null;
        /// <summary>
        /// 管理员Session名 CurAdmin 需要用到这个变量
        /// </summary>
        public static string AdminSessionName
        {
            get { return _TmpAdminSessionName ?? "ca" + GUIDKey; }
        }


        /// <summary>
        /// 当前登录管理员
        /// </summary>
        public static IUser CurAdmin
        {
            get
            {
                var result = HttpContext.Current.Session[AdminSessionName] as IUser;

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
                HttpContext.Current.Session[AdminSessionName] = value;
               // HttpContext.Current.Response.Cookies.Add(new HttpCookie(AdminSessionName, JsonConvert.SerializeObject(value)));
            }
        }

        private static void LoadConfig()
        {
            if (_SysConfigEntity == null)
            {
                using (FileStream fs = new FileStream(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config/Config.xml"), FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SysConfig));
                    _SysConfigEntity = xs.Deserialize(fs) as SysConfig;
                }
            }
        }
        /// <summary>
        /// 把json转为JArray，在转为List />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JArrayToEntities<T>(string json) where T : class,new()
        {
            JArray jarray = JsonConvert.DeserializeObject<JArray>(json);
            return JArrayToEntities<T>(jarray);
        }
        /// <summary>
        /// 将JSON转换出来的JArray类型数据转成实体
        /// </summary>
        /// <param name="jArray">JArray类型的数据</param>
        /// <param name="foreignKeyField">外键字段名</param>
        /// <param name="foreignKey">外键值</param>
        /// <returns></returns>
        public static List<T> JArrayToEntities<T>(JArray jArray) where T: class,new()
        {
            List<T> result = new List<T>();
            var tType = typeof(T);
            var properties = tType.GetProperties();

            foreach (var elem in jArray)
            {
                string json= JsonConvert.SerializeObject(elem);            
                result.Add(JsonConvert.DeserializeObject<T>(json));
            }
            return result;
        }
    }
}