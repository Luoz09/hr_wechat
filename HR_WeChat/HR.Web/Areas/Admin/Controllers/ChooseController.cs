using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.EDM.DAL;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class ChooseController : DataOperationPage<ORGANIZATIONS>
    {

        public ActionResult Users()
        {
            return View();
        }

        public ContentResult GetUsers(string parentID)
        {
            var data = new OU_USERSManager().GetUsersListByDepID(parentID);
            return DisplayJson(data);
        }


        public ContentResult SearchUsers()
        { 
            return DisplayJson(new OU_USERSManager().GetUsersBySearch(Request["str"]));
        }

        public ActionResult Branch()
        {
            return View();
        }

        public ContentResult GetBranchs()
        {
            var data = new ORGANIZATIONSManager().GetDepInfo();
            return DisplayJson(data);
        }


        public ContentResult SearchDeps()
        {  
            return DisplayJson(new ORGANIZATIONSManager().GetDepsBySearch(Request["str"]));
        }


        public ContentResult GetDepByParentID(string Guid)
        {
            var data = new ORGANIZATIONSManager().GetDepsByParentID(Guid);
            return DisplayJson(data);
        }


        public ActionResult Users_WF() { return View(); }
         

    }
}
