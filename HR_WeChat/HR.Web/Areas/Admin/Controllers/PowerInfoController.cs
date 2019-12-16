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
    public class PowerInfoController : DataOperationPage<APPLICATIONS>
    { 
        public ContentResult GetUsers()
        {
            JObject jobject = new JObject();
            var userData = new OU_USERSManager().GetOUUser();
            jobject.Add("UserData", JsonConvert.SerializeObject(userData));
            var orgData = new ORGANIZATIONSManager().GetDepInfo();
            jobject.Add("OrgData", JsonConvert.SerializeObject(orgData));
            return DisplayJson(jobject);
        }


        public ContentResult GetPowerList(string Name)
        {
            var data = new JArray(); 
             
            var application = new APPLICATIONSManager().GetApplicationByCodeName("HR");
            var roles = new ROLESManager().GetCLASSIFYData();
            foreach (var item in application)
            {
                JArray jarray = new JArray();
                JObject jobject = new JObject();
                jobject.Add("ID", item.ID);
                jobject.Add("Name", item.NAME);
                var rolesList = roles.Where(m => m.APP_ID == item.ID);
                foreach (var item1 in rolesList)
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


        public ContentResult GetUserPowerList(string name)
        {
            var userPower = new EXPRESSIONSManager().GetDataByName(name);
            foreach (var item in userPower)
            {
                var roles = new ROLESManager().GetDataByRoleID(item.ROLE_ID).FirstOrDefault();
                item.ParentID = roles.APP_ID;
            }
            return DisplayJson(userPower);
        }


        public ContentResult SaveChoosePower()
        {
            var powerData = Request["powerData"];
            var chooseName = Request["Name"];
            var chooseID = Request["ID"];
            var chooseType = Request["Type"];

            var powerList = powerData.Split(',');
            string Expression = "BelongTo({0}, \"{1}\", \"{2}\")";

            var allPathName = "";


            EXPRESSIONSManager expressManager = new EXPRESSIONSManager();

            if (chooseType == "ORGANIZATIONS")
            {
                var entity = new ORGANIZATIONSManager().GetEntity(chooseID);
                Expression = string.Format(Expression, chooseType, chooseID, entity.PARENT_GUID);
                allPathName = entity.ALL_PATH_NAME;
            }
            if (chooseType == "USERS")
            {
                var entity = new OU_USERSManager().GetDataByUserID(chooseID);
                Expression = string.Format(Expression, chooseType, chooseID, entity.PARENT_GUID);
                allPathName = entity.ALL_PATH_NAME;
            }

            var expressData = expressManager.GetDataByName(chooseName);

            List<EXPRESSIONS> delList = new List<EXPRESSIONS>();

            foreach (var item in expressData)
            {
                if (powerData.Contains(item.ROLE_ID))
                {
                    continue;
                }
                else
                {
                    delList.Add(item); 
                }
            }

            foreach (var item in delList)
            {
                expressManager.Delete(item);
            }

            foreach (var item in powerList)
            {
                if (expressData.Where(m => m.ROLE_ID == item).Count() > 0 || string.IsNullOrEmpty(item))
                {
                    continue;
                }
                else
                {
                    EXPRESSIONS express = new EXPRESSIONS();
                    express.ROLE_ID = item;
                    express.NAME = chooseName;
                    express.EXPRESSION = Expression;
                    express.DESCRIPTION = allPathName;
                    express.SORT_ID = expressData.Count() + 1;
                    express.INHERITED = "n";
                    express.CLASSIFY = 0;
                    express.MODIFY_TIME = DateTime.Now;
                    if (!expressManager.Insert(express))
                        return DisplayJson(false, "保存失败");
                }
            }
            return DisplayJson(true, "保存成功");
        }

    }
}
