using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using Sgms.Frame.Rights;
using Sgms.Frame.Interface.UI;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.IO;

namespace Sgms.Frame.Page.MVC
{
    /// <summary>
    /// MVC普通页面类
    /// </summary>
    public abstract class NormalPage : Controller
    {
        #region Request Response Server Session Application

        /// <summary>
        /// Request
        /// </summary>
        protected HttpRequestBase Request
        {
            get
            {
                return HttpContext.Request;
            }
        }

        /// <summary>
        /// Response
        /// </summary>
        protected HttpResponseBase Response
        {
            get
            {
                return HttpContext.Response;
            }
        }

        /// <summary>
        /// Server
        /// </summary>
        protected HttpServerUtilityBase Server
        {
            get
            {
                return HttpContext.Server;
            }
        }

        /// <summary>
        /// Session
        /// </summary>
        protected HttpSessionStateBase Session
        {
            get
            {
                return HttpContext.Session;
            }
        }
         
        /// <summary>
        /// Application
        /// </summary>
        protected HttpApplicationStateBase Application
        {
            get
            {
                return HttpContext.Application;
            }
        }

        #endregion Request Response Server Session Application

        private IMVCDataDisplay _TmpDataDisplay;
        /// <summary>
        /// 数据展示
        /// </summary>
        protected virtual IMVCDataDisplay DataDisplay
        {
            get
            {
                if (_TmpDataDisplay == null)
                {
                    _TmpDataDisplay = new MVCDataDisplay();
                }
                return _TmpDataDisplay;
            }
        }




        /// <summary>
        /// 导航
        /// </summary>
        protected List<LinkInfo> Nav;

        /// <summary>
        /// 页面中文名
        /// </summary>
        protected virtual string PageCName { get { return String.Empty; } }

        #region 权限相关

        private RightsProcess _TmpCurRightsProcess;
        private RightsProcess CurRightsProcess
        {
            get
            {
                if (_TmpCurRightsProcess == null)
                {
                    _TmpCurRightsProcess = new RightsProcess(RightsDataObtain);
                }
                return _TmpCurRightsProcess;
            }
        }

        /// <summary>
        /// 当前Action
        /// </summary>
        protected string CurAction;
        /// <summary>
        /// 当前Controller
        /// </summary>
        protected string CurController;

        private IRightsDataObtain _TmpRightsDataObtain;
        private IRightsDataObtain RightsDataObtain
        {
            get
            {
                if (_TmpRightsDataObtain == null)
                {
                    _TmpRightsDataObtain = TypeUtil.CreateInstance<IRightsDataObtain>(SysPara.RightsDataObtainClassInfo);
                }
                return _TmpRightsDataObtain;
            }
        }

        private AuthenticateLevel _TmpAuthenticateLevel = SysPara.DefaultAuthenticateLevel;
        /// <summary>
        /// 认证级别
        /// </summary>
        protected virtual AuthenticateLevel CurAuthenticateLevel
        {
            get { return _TmpAuthenticateLevel; }
        }

        private string _TmpPageName;
        /// <summary>
        /// 页面名
        /// </summary>
        protected virtual string PageName
        {
            get
            {
                if (_TmpPageName == null)
                {
                    var typeName = GetType().Name;
                    var indexOfMVCPageName = typeName.LastIndexOf(SysKeys.SUFFIX_MVC_PAGE_CLASSNAME);
                    _TmpPageName = typeName.Substring(0, indexOfMVCPageName);
                }
                return _TmpPageName;
            }

        }

        /// <summary>
        /// 认证
        /// </summary>
        protected virtual bool Authenticate(string actionName)
        {
            if (CurAuthenticateLevel == AuthenticateLevel.None)
            {
                return true;
            }
            if (CurAuthenticateLevel == AuthenticateLevel.Simple || CurAuthenticateLevel == AuthenticateLevel.InDepth)
            {
                if (SysPara.CurAdmin == null)
                {
                    string WebMessge = Request.Browser.Platform.ToString();
                    WebUtil.WriteLog(WebMessge); 

                    String redirectURl = Server.UrlEncode(Request.Url.AbsoluteUri);

                    if (Request.Url.AbsolutePath.ToLower().IndexOf("mobile") <= 0 && (WebMessge.ToLower().IndexOf("win") >= 0 || WebMessge.ToLower().IndexOf("mac") >= 0))
                    {
                        Response.Redirect(string.Format(SysPara.PCLoginPageName + "?RedirectUrl=" + redirectURl));
                        Response.End();
                        WebUtil.WriteLog("WinNT||mac" + SysPara.PCLoginPageName);

                        WebUtil.WriteLog(string.Format("LoginPageName:{0}", SysPara.PCLoginPageName));
                        WebUtil.WriteLog(string.Format("AbsoluteUri:{0}", Request.Url.AbsoluteUri));
                        return false;
                    } 
                    else
                    {
                        Response.Redirect(string.Format(SysPara.MobileLoginPageName + "?RedirectUrl=" + Server.UrlEncode(Request.Url.AbsoluteUri)));
                        Response.End();
                        WebUtil.WriteLog("mobile" + SysPara.MobileLoginPageName);
                        WebUtil.WriteLog(string.Format("LoginPageName:{0}", SysPara.MobileLoginPageName));
                        WebUtil.WriteLog(string.Format("AbsoluteUri:{0}", Request.Url.AbsoluteUri));
                        return false;
                    }

                    //RedirectToAction("Login", "Home");
                    Response.Redirect(SysPara.PCLoginPageName /*+ "?url=" + Server.UrlEncode(Request.Url.AbsoluteUri)*/);
                    return false;
                }
                if (CurAuthenticateLevel == AuthenticateLevel.InDepth)
                {
                    if (RightsDataObtain != null && !CurRightsProcess.Authenticate(PageName, actionName))
                    {
                        DisplayNoRightsPage();
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        /// <summary>
        /// 权限判断
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!Authenticate(filterContext.ActionDescriptor.ActionName))
            {
                filterContext.Result = null;
                return;
                //throw new Exception();
            }
            base.OnAuthorization(filterContext);
            ViewBag.Action = filterContext.ActionDescriptor.ActionName;
            CurAction = filterContext.ActionDescriptor.ActionName;
            ViewBag.Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            CurController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        }

        #endregion 权限相关

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            Nav = GetListNav();
            ViewBag.Nav = Nav;
            return View();
        }

        /// <summary>
        /// 获取列表页面的导航
        /// </summary>
        /// <returns></returns>
        protected List<LinkInfo> GetListNav()
        {
            var result = new List<LinkInfo>();
            result.Add(new LinkInfo()
            {
                Href = Url.Action("Desktop", "Home"),
                Text = "桌面"
            });
            result.Add(new LinkInfo()
            {
                Text = PageCName + "列表"
            });
            return result;
        }

        #region Display

        /// <summary>
        /// 显示JSON字符串
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public ContentResult DisplayJson(string jsonStr)
        {
            return DataDisplay.DisplayJson(jsonStr);
        }

        /// <summary>
        /// 显示JSON字符串
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ContentResult DisplayJson(bool success, string message)
        {
            return DataDisplay.DisplayJson(success, message);
        }

        /// <summary>
        /// 显示JSON字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual ContentResult DisplayJson(object obj)
        {
            return DataDisplay.DisplayJson(obj);
        }

        /// <summary>
        /// 显示JSON字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual ContentResult DisplayJson(IEnumerable<object> data)
        {
            return DataDisplay.DisplayJson(data);
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual ContentResult DisplayText(string text)
        {
            return DataDisplay.DisplayText(text);
        }

        /// <summary>
        /// 显示没有权限的页面
        /// </summary>
        protected void DisplayNoRightsPage()
        {
            DataDisplay.DisplayNoRightsPage();
        }

        /// <summary>
        /// 显示没有权限的Json
        /// </summary>
        protected ContentResult DisplayNoRightsJson()
        {
            return DataDisplay.DisplayNoRightsJson();
        }

        #endregion Display

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        protected void FillRequestInfo<T>(T entity)
        {
            IEnumerable<PropertyInfo> properInfos = entity.GetType().GetProperties();

            foreach (var elem in properInfos)
            {
                string value = Request[elem.Name];
                if (value != null)
                {
                    try
                    {
                        elem.SetValue(entity, Convert.ChangeType(value, Nullable.GetUnderlyingType(elem.PropertyType) ?? elem.PropertyType), null);
                    }
                    catch
                    {
                        try
                        {
                            elem.SetValue(entity, elem.PropertyType.IsValueType ? Activator.CreateInstance(elem.PropertyType) : null, null);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}
