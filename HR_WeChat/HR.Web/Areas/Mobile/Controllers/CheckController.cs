using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Mobile.Controllers
{
    public class CheckController : DataOperationPage<HR_Check>
    {

        public ActionResult History()
        {
            return View();
        }


        public ContentResult GetDayCheck()
        {
            var day = Request["Day"];
            var data = new HR_CheckManager().GetData();
            return DisplayJson(data);
        }
          
    }
}
