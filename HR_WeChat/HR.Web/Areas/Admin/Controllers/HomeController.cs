using HR.BLL;
using Newtonsoft.Json.Linq;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class HomeController : NormalPage
    {
        //protected override AuthenticateLevel CurAuthenticateLevel
        //{
        //    get
        //    {
        //        return AuthenticateLevel.None;
        //    }
        //}

        public ActionResult Desktop()
        { 
            return View(); 
        }



        #region 待办事项相关
        //获取待办事项数量
        public ContentResult GetWorkItemCount()
        {
            var applicationname = Request["AppName"];
            JArray jarray = new JArray();
            JObject jobject = new JObject();
            jobject.Add("Name", applicationname);
            jobject.Add("Count", new TASK_LIST_VIEWManager().GetWorkItemCount(SysPara.CurAdmin.ID, applicationname));
            jarray.Add(jobject);
            return DisplayJson(jarray);
        }

        //获取分类待办事项数据
        public ContentResult GetWorkItemData()
        {
            return DisplayJson(new TASK_LIST_VIEWManager().GetWorkItem(SysPara.CurAdmin.ID, Request["AppName"]));
        }
        #endregion


        #region 在办事项相关
        //获取在办事项数量
        public ContentResult GetWorkItemingCount()
        {
            var applicationname = Request["AppName"];
            JArray jarray = new JArray();
            JObject jobject = new JObject();
            jobject.Add("Name", applicationname);
            jobject.Add("Count", new TASK_LIST_VIEWManager().GetWorkItemingCount(SysPara.CurAdmin.ID, applicationname));
            jarray.Add(jobject);
            return DisplayJson(jarray);
        }

        //获取分类待办事项数据
        public ContentResult GetWorkItemingData()
        {
            return DisplayJson(new TASK_LIST_VIEWManager().GetWorkIteming(SysPara.CurAdmin.ID, Request["AppName"]));
        }
        #endregion



        public ContentResult GetCountByApplyType()
        {
            var applyType = Request["ApplyType"];
            var data = new HR_ApplyInfoManager().GetApplyCountByApplyType(Convert.ToInt32(applyType));
            return DisplayJson(data);
        } 
    }


}
