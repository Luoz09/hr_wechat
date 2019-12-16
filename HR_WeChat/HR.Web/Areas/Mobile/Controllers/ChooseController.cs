using HR.BLL;
using HR.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HR.Web.Areas.Mobile.Controllers
{
    public class ChooseController : NormalPage
    {
          
        public ActionResult Users()
        {
            return View();
        }


        //获取根部门
        public  ContentResult GetTopDep()
        {
            return DisplayJson(new ORGANIZATIONSManager().GetTopDep());
        }

        //获取二级部门
        public ContentResult GetSecDeps()
        { 
            var data = new ORGANIZATIONSManager().GetSecDepsByTopDep();
            return DisplayJson(data);
        }


        //depID 获取子部门
        public ContentResult GetDepsByParentID()
        { 
            var data = new ORGANIZATIONSManager().GetDepsByParentID(Request["ParentID"]);
            return DisplayJson(data);
        }

         
        public ContentResult GetDepByDepID()
        { 
            return DisplayJson(new ORGANIZATIONSManager().GetDepByDepID(Request["DepID"]));
        }
         

        public ContentResult GetUsersByDepID(string depID) 
        {
            var data = new OU_USERSManager().GetUsersListByDepID(depID); 
            return DisplayJson(data);
        }


        string Web_Host = AppSettingUtil.GetAppSetting("Web_Host");

        public ContentResult GetNextResource()
        {

            //string url = "";
            //var userid = SysPara.CurAdmin.ID; 
            //var data = "";
            //JArray jarray = new JArray();
            //try
            //{
            //    var parentid = Request["parentid"].Split(',');
            //    url = Web_Host + "/WebApi/WorkFlow/GetNextActivityResource";
            //    data = WebUtil.Post(url, "processDKey=" + parentid[0] + "&userID=" + userid + "&nextActivityKey=" + parentid[1] + "&AppName =" + parentid[2] + "");
            //    jarray = (JArray)JsonConvert.DeserializeObject(data);
            //}
            //catch
            //{
            //    var parentid = Request["parentid"].Split(',');
            //    url = Web_Host + "/WebApi/WorkFlow/GetNextActivityResource";
            //    data = WebUtil.Post(url, "activityID=" + parentid[0] + "&userID=" + userid + "&nextActivityKey=" + parentid[1] + "&AppName =" + parentid[2] + "");
            //    jarray = (JArray)JsonConvert.DeserializeObject(data);
            //}

            var data = GetNextResourceUser(Request["parentid"]);

            IEnumerable<object> UserList = null;

            var str = Request["str"];
            if (Request["search"] == "1")
            {
                UserList = data.Where(m => m["DisplayName"].ToString().Contains(str));
            }
            else
            {
                UserList = data.GroupBy(m => m["ObjName"]);
            }

            return DisplayJson(UserList);
        }

        //获取工作流下一环节人员
        public JArray GetNextResourceUser(string parentids)
        {
            string url = "";
            var userid = SysPara.CurAdmin.ID;
            var data = "";
            JArray jarray = new JArray();
            var parentid = parentids.Split(',');

            try
            {
                if (parentid[3] == "true")
                {
                    url = Web_Host + "/WebApi/WorkFlow/GetNextActivityResource";
                    data = WebUtil.Post(url, "processDKey=" + parentid[0] + "&userID=" + userid + "&nextActivityKey=" + parentid[1] + "&AppName =" + parentid[2] + "");
                    jarray = (JArray)JsonConvert.DeserializeObject(data);
                }
                else
                {
                    url = Web_Host + "/WebApi/WorkFlow/GetNextActivityResource";
                    data = WebUtil.Post(url, "activityID=" + parentid[0] + "&userID=" + userid + "&nextActivityKey=" + parentid[1] + "&AppName =" + parentid[2] + "");
                    jarray = (JArray)JsonConvert.DeserializeObject(data);
                }
            }
            catch
            {

            }

            return jarray;
        }

        public ContentResult SearchUsers()
        {
            var data = new OU_USERSManager().GetUsersBySearch(Request["str"]);
            return DisplayJson(data);
        }


        public ActionResult Users_WF()
        {
            return View();
        }


    }
}
