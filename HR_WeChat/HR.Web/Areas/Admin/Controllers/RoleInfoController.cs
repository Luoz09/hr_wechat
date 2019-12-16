using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sgms.Frame;
using Sgms.Frame.BLL;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class RoleInfoController : NormalPage
    {
        public ContentResult GetRoles()
        {
            JObject jobject = new JObject();
            var application = new APPLICATIONSManager().GetApplicationByCodeName("HR");
            jobject.Add("Application", JsonConvert.SerializeObject(application));

            List<ROLES> list = new List<ROLES>();

            foreach (var item in application)
            {
                var roles = new ROLESManager().GetCLASSIFYData().Where(m => m.APP_ID == item.ID).OrderBy(m=>m.SORT_ID);
                list.AddRange(roles); 
            } 
            jobject.Add("Roles", JsonConvert.SerializeObject(list));
            return DisplayJson(jobject);
        }


        public ContentResult GetFunctionList()
        {
            var data = new JArray();
            var application = new APPLICATIONSManager().GetApplicationByCodeName("HR");
             
            foreach (var item in application)
            {
                JArray jarray = new JArray();
                JObject jobject = new JObject();
                jobject.Add("ID", item.ID);
                jobject.Add("Name", item.NAME); 
                var functions = new FUNCTIONSManager().GetFunctions().Where(m => m.APP_ID == item.ID && m.CLASSIFY=="n" && m.MENU=="y").OrderBy(m=>m.SORT_ID);
                foreach (var item1 in functions)
                {
                    JObject newjobject = new JObject();
                    newjobject.Add("ID", item1.ID);
                    newjobject.Add("Name", item1.NAME);
                    newjobject.Add("Code_Name", item1.CODE_NAME);
                    newjobject.Add("Description", item1.DESCRIPTION);
                    jarray.Add(newjobject);
                }
                jobject.Add("children", jarray);
                data.Add(jobject);
            }
          
            return DisplayJson(data);
        }

        public ContentResult GetRolesFunctionList(string RoleID)
        {
            var RolesFun = new ROLE_TO_FUNCTIONSManager().GetRoleFunDataByRoleID(RoleID);   
            return DisplayJson(RolesFun); 
        }


        public ContentResult SaveRoleFunc()
        {
            var RolesFuncData = Request["RolesFuncData"];
            var chooseName = Request["Name"];
            var chooseID = Request["ID"];
           
            var funcID = RolesFuncData.Split(','); 
            var rolesFunc = new ROLE_TO_FUNCTIONSManager().GetRoleFunDataByRoleID(chooseID);

            List<ROLE_TO_FUNCTIONS> delList= new List<ROLE_TO_FUNCTIONS>();

            //判断当前选中的功能授权是否存在
            foreach (var item in rolesFunc)
            {
                if (RolesFuncData.Contains(item.FUNC_ID)) { continue; }
                else {  delList.Add(item); }
            }

            //删除未选中的功能授权
            foreach (var item in delList)
            {
                new ROLE_TO_FUNCTIONSManager().Delete(item);
            }
             

            foreach (var item in funcID)
            {
                if (rolesFunc.Where(m => m.FUNC_ID == item).Count() > 0) { continue; }
                else {
                    ROLE_TO_FUNCTIONS roleToFunc = new ROLE_TO_FUNCTIONS();
                    roleToFunc.FUNC_ID = item;
                    roleToFunc.ROLE_ID = chooseID;
                    roleToFunc.INHERITED = "n";
                    if (!new ROLE_TO_FUNCTIONSManager().Insert(roleToFunc))
                        return DisplayJson(false, "保存失败");
                } 
            }
             
            return DisplayJson(true, "保存成功");
        }

          
    }
}
