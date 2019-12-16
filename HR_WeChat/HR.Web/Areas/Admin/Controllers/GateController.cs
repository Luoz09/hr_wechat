using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class GateController : NormalPage
    {
        protected override AuthenticateLevel CurAuthenticateLevel
        {
            get
            {
                return AuthenticateLevel.None;
            }
        }


        public ActionResult Login()
        {
            if (SysPara.CurAdmin != null)
            {
                string redirectURl = Server.UrlDecode(Request.Params["RedirectUrl"]);
                if (redirectURl == null)
                    redirectURl = "~/Admin/Home/Index";
                Response.Redirect(redirectURl);
            }
            
            return View();
        }

        public ContentResult UserLogin(FormCollection form)
        { 
            var user = new USERSManager().GetUserByNamePwd(Request.Form["username"], Request.Form["password"]);
            if (user != null)
            {
                SysPara.CurAdmin = user;
                return DisplayJson(true, "登录成功!");
            }
            return DisplayJson(false,"登录失败! 账号 或 密码错误");
        }


        public ActionResult ChangePwd()
        {
            return View();
        }

        public ActionResult Logout()
        {
            for (int i = 0; i < this.Request.Cookies.Count; i++)
            {
                this.Response.Cookies[this.Request.Cookies[i].Name].Expires = DateTime.Now.AddDays(-1);
            } 

            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();

            return RedirectToAction("Login");
        }


    }
}
