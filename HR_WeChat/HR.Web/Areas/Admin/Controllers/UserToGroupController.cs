using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sgms.Frame.Page.MVC;
using HR.Models;
using HR.BLL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sgms.Frame.Utils;
using HR.DAL;
using System.Data;

namespace HR.Web.Areas.Admin.Controllers
{
    public class UserToGroupController : DataOperationPage<OU_USERS>
    { 
        protected override Sgms.Frame.AuthenticateLevel CurAuthenticateLevel
        {
            get
            {
                return Sgms.Frame.AuthenticateLevel.Simple;
            }
        }

        private struct ReturnList
        {
            public string code;
            public string msg;
            public int count;
            public IQueryable data;
        }

        public ContentResult GetUserData()
        {
            JObject jobject = new JObject();
            var userData = new OU_USERSManager().GetOUUser().Where(m => m.STATUS == 1).OrderBy(m => m.GLOBAL_SORT);
            jobject.Add("UserData", JsonConvert.SerializeObject(userData));
            var orgData = new ORGANIZATIONSManager().GetDepInfo().Where(m => m.STATUS == 1).OrderBy(m => m.GLOBAL_SORT);
            jobject.Add("OrgData", JsonConvert.SerializeObject(orgData));
            return DisplayJson(jobject);
            //var ouusersData = new OUUSERSManager().GetData().Where(m=>m.STATUS == 1);
            //var orgData = new ORGANIZATIONSManager().GetData().Where(m=>m.STATUS == 1);
            //var str = Request["str"];
            //var data = ouusersData.Join(orgData, m => m.PARENT_GUID, m1 => m1.GUID, (m, m1) => new
            //{
            //    m.DISPLAY_NAME,
            //    m1.OBJ_NAME,
            //    m.GLOBAL_SORT,
            //    m.USER_GUID,
            //    m.PARENT_GUID,
            //});
            //if (!string.IsNullOrEmpty(str))
            //{
            //    data = data.Where(m => m.DISPLAY_NAME.Contains(str) || m.OBJ_NAME.Contains(str));
            //}
            //ReturnList rl = new ReturnList();
            //rl.code = "0";
            //rl.msg = "";
            //rl.count = data.Count();
            //rl.data = data.OrderBy(m => m.GLOBAL_SORT).AsQueryable();
            //return DisplayJson(ouusersData);
        }


        public ContentResult GetGroupData()
        {
            var data = new GROUPSManager().GetGroups().Where(m=>m.STATUS == 1).OrderBy(m=>m.GLOBAL_SORT);
            ReturnList rl = new ReturnList();
            rl.code = "0";
            rl.msg = "";
            rl.count = data.Count();
            rl.data = data.AsQueryable();
            return DisplayJson(rl);
        }


        public ContentResult GetUserToGroupData()
        {
            var id = Request["id"];
            var groupData = new GROUPSManager().GetGroups().Where(m=>m.STATUS == 1);
            var data = new GROUP_USERSManager().GetGroupUsers().Where(m => m.USER_GUID == id).Join(groupData, m => m.GROUP_GUID, m1 => m1.GUID, (m, m1) => new
            {
                m.GROUP_GUID,
                m.USER_GUID,
                m.USER_PARENT_GUID,
                m1.DISPLAY_NAME,
            });
            return DisplayJson(data);
        }

        public ContentResult SaveResult()
        {
            var userGroupData = (JArray)JsonConvert.DeserializeObject(Request["userGroupData"]);
            var result = false;
            foreach(var item in userGroupData)
            { 
                var userID = item["userID"].ToString();
                var userGroups = new GROUP_USERSManager().GetGroupUsers().Where(m => m.USER_GUID == userID);

                var newUserGroups = item["userGroupData"]; 

                //删除原先的数据
                foreach(var userGroup in userGroups)
                {
                    var sql = "delete from GROUP_USERS where GROUP_GUID = '" + userGroup.GROUP_GUID + "' and USER_GUID= '" + userGroup.USER_GUID + "' ";
                    SqlHelp.RunSql(sql); 
                }

                //添加数据
                foreach (var newUserGroup in newUserGroups)
                {
                    var groupID = newUserGroup["GROUP_GUID"].ToString();
                    var groupUserData = new GROUP_USERSManager().GetGroupUsers().Where(m => m.GROUP_GUID == groupID).OrderByDescending(m => m.INNER_SORT).FirstOrDefault();
                    var sort = "0";
                    if (groupUserData != null)
                    {
                        sort = (int.Parse(groupUserData.INNER_SORT) + 1).ToString();
                    }
                    var sql = "insert into GROUP_USERS values ('{0}','{1}','{2}','{3}','{4}','{4}')";
                    sql = string.Format(sql, groupID, userID, newUserGroup["USER_PARENT_GUID"].ToString(), sort.PadLeft(6, '0'), DateTime.Now);
                    SqlHelp.RunSql(sql); 
                }

                //判断修改是否成功
                var groupUserCount = new GROUP_USERSManager().GetGroupUsers().Where(m => m.USER_GUID == userID).Count();
                if(groupUserCount == newUserGroups.Count())
                {
                    result = true;
                }

            } 
            return DisplayJson(result);
        }


    }
}
