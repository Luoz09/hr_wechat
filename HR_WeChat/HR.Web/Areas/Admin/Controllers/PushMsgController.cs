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
    public class PushMsgController : NormalPage
    {
        protected override AuthenticateLevel CurAuthenticateLevel
        {
            get
            {
                return AuthenticateLevel.None;
            }
        }

        public bool PushWeChatMsg()
        {
            string userID = Request.Params["UserID"];
            string title = Request.Params["Title"];
            string keyword1 = Request.Params["KeyWord1"];
            string keyword2 = Request.Params["KeyWord2"];
            string keyword3 = Request.Params["KeyWord3"];
            string keyword4 = Request.Params["KeyWord4"];
            string keyword5 = Request.Params["KeyWord5"];
            string remark = Request.Params["Remark"];

            string[] values = new string[] { title, keyword1, keyword2, keyword3, keyword4, keyword5, remark };
            string result = new WechatUtil().SendMessage(userID, "", values);

            return string.IsNullOrEmpty(result) ? false : true; 
        }

    }
}
