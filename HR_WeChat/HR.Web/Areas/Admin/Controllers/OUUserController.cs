using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Jeedaa.Framework.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sgms.Frame;
using Sgms.Frame.BLL;
using Sgms.Frame.EDM.DAL;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class OUUserController : DataOperationPage<OU_USERS>
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

        #region 树形数据相关

        //部门树数据
        public ContentResult GetOrgData()
        {
            var data = new ORGANIZATIONSManager().GetDepInfo().Where(m => m.STATUS == 1).OrderBy(m => m.GLOBAL_SORT);
            return DisplayJson(data);
        }

        //获取部门对应人员信息
        public ContentResult GetUserData()
        {
            var id = Request["id"];
            var userInfo = new USERSManager().GetUsersData();
            var data = new OU_USERSManager().GetOUUser().Where(m => m.PARENT_GUID == id && m.STATUS == 1).Join(userInfo, m => m.USER_GUID, m1 => m1.GUID, (m, m1) => new
            {
                m.USER_GUID,
                m.RANK_NAME,
                m.DISPLAY_NAME,
                m.ALL_PATH_NAME,
                m.GLOBAL_SORT,
                m.PARENT_GUID,
                m1.MOBILE,
                m.INNER_SORT,
                m.SIDELINE,
            });
            ReturnList rl = new ReturnList();
            rl.code = "0";
            rl.msg = "";
            rl.count = data.Count();
            rl.data = data.OrderBy(m => m.GLOBAL_SORT).AsQueryable();
            return DisplayJson(rl);
        }

        #endregion


        string dataBaseName = AppSettingUtil.GetAppSetting("dataBaseName");
        string useProce = AppSettingUtil.GetAppSetting("useProce");

        #region 人员相关

        //获取通讯录人员信息
        public ContentResult GetAddressUser()
        {
            var str = Request["str"];
            int page = int.Parse(Request["page"]);
            int limit = int.Parse(Request["limit"]);

            //var ouuserData = new OUUSERSManager().GetData().Where(m => m.STATUS == 1);
            //var data = new USERSManager().GetData().Join(ouuserData, m => m.GUID, m1 => m1.USER_GUID, (m, m1) => new
            //{
            //    m1.USER_GUID,
            //    m1.RANK_NAME,
            //    m1.DISPLAY_NAME,
            //    m1.ALL_PATH_NAME,
            //    m1.GLOBAL_SORT,
            //    m1.PARENT_GUID,
            //    m.MOBILE,
            //    m.CREATE_TIME,
            //});
            var data = new USERSManager().GetUsersData();
            if (!string.IsNullOrEmpty(str))
            {
                //data = data.Where(m => m.LOGON_NAME.Contains(str) || (!string.IsNullOrEmpty(m.MOBILE) && m.MOBILE.Contains(str)) || (!string.IsNullOrEmpty(m.Post) && m.Post.Contains(str)));
                data = data.Where(m => m.LOGON_NAME.Contains(str) || (!string.IsNullOrEmpty(m.MOBILE) && m.MOBILE.Contains(str)));
            }
            var pageData = data.OrderByDescending(m => m.CREATE_TIME).Skip((page - 1) * limit).Take(limit);
            foreach (var item in pageData)
            {
                var ouuserData = new OU_USERSManager().GetOUUser().Where(m => m.USER_GUID == item.GUID).OrderBy(m => m.SIDELINE).FirstOrDefault();
                if (ouuserData != null)
                {
                    if (ouuserData.SIDELINE == 0)
                    {
                        item.OFVIRTEL = ouuserData.ALL_PATH_NAME;
                    }
                    else
                    {
                        item.OFVIRTEL = "(兼职) " + ouuserData.ALL_PATH_NAME;
                    }
                }
                else
                {
                    item.OFVIRTEL = "";
                }
            }

            ReturnList rl = new ReturnList();
            rl.code = "0";
            rl.msg = "";
            rl.count = data.Count();
            rl.data = pageData.AsQueryable();
            return DisplayJson(rl);
        }

        //获取人员信息
        public ContentResult GetUserInfo()
        {
            var id = Request["id"];
            var depID = Request["depID"];
            var userInfo = new USERSManager().GetUsersData();
            var data = new OU_USERSManager().GetOUUser().Where(m => m.USER_GUID == id && m.PARENT_GUID == depID && m.STATUS == 1).Join(userInfo, m => m.USER_GUID, m1 => m1.GUID, (m, m1) => new
            {
                m.RANK_NAME,
                m.DISPLAY_NAME,
                m.ALL_PATH_NAME,
                m.GLOBAL_SORT,
                m.PARENT_GUID,
                m.INNER_SORT,
                m1.MOBILE,
                m1.DUTY,
                m1.SYSCONTENT1,
                m1.FIRST_NAME,
                m1.LAST_NAME,
                m1.LOGON_NAME,
                m1.SEX,
            }).FirstOrDefault();
            return DisplayJson(data);
        }

        //新增人员保存
        public ContentResult SaveUserInfo()
        {
            var result = false;
            var userID = Request["userID"];
            var depID = Request["depID"];
            OU_USERSManager ouuserManager = new OU_USERSManager();
            USERSManager userManager = new USERSManager();
            var ouuser = ouuserManager.GetOUUser().Where(m => m.USER_GUID == userID && m.PARENT_GUID == depID).FirstOrDefault();
            var user = userManager.GetEntity(userID);
            var depInfo = new ORGANIZATIONSManager().GetEntity(depID);
            var sort = Request["Sort"];
            if (user == null && ouuser == null)
            {
                user = new USERS();
                user.GUID = Guid.NewGuid().ToString(); 
                user.LOGON_NAME = user.LAST_NAME = user.FIRST_NAME = Request["LOGON_NAME"];
                user.PWD_TYPE_GUID = "21545d16-a62f-4a7e-ac2f-beca958e0fdf";
                user.USER_PWD = EncryptUtil.PwdCalculate("", "000000");
                user.MOBILE = Request["MOBILE"];
                user.CREATE_TIME = user.MODIFY_TIME = DateTime.Now;
                user.JOINCOMTIME = DateTime.Now;
                user.RANK_CODE = "COMMON_U";
                user.SEX = int.Parse(Request["SEX"]);
                user.SYSCONTENT1 = Request["SYSCONTENT1"];

                ouuser = new OU_USERS();
                ouuser.PARENT_GUID = depID;
                ouuser.USER_GUID = user.GUID;
                ouuser.DISPLAY_NAME = ouuser.OBJ_NAME = user.LOGON_NAME;
                ouuser.INNER_SORT = sort.PadLeft(6, '0');
                ouuser.GLOBAL_SORT = depInfo.GLOBAL_SORT + ouuser.INNER_SORT;
                ouuser.ORIGINAL_SORT = depInfo.ORIGINAL_SORT + ouuser.INNER_SORT;
                ouuser.ALL_PATH_NAME = depInfo.ALL_PATH_NAME + "\\" + ouuser.DISPLAY_NAME;
                ouuser.STATUS = 1;
                ouuser.CREATE_TIME = ouuser.MODIFY_TIME = DateTime.Now;
                ouuser.RANK_NAME = Request["RANK_NAME"];
                ouuser.SIDELINE = 0;
                ouuser.START_TIME = Convert.ToDateTime("2000-01-01");
                ouuser.END_TIME = DateTime.MaxValue;
                   
                if (userManager.Insert(user) && ouuserManager.Insert(ouuser))
                    result = true;
            }
            else
            {
                
                user.LOGON_NAME = user.LAST_NAME = user.FIRST_NAME  = Request["LOGON_NAME"];
                user.MOBILE = Request["MOBILE"];
                user.MODIFY_TIME = DateTime.Now;
                user.SEX = int.Parse(Request["SEX"]);
                user.SYSCONTENT1 = Request["SYSCONTENT1"];
                if(user.JOINCOMTIME == null)
                {
                    user.JOINCOMTIME = DateTime.Now;
                }
                
                ouuser.INNER_SORT = sort.PadLeft(6, '0');
                ouuser.DISPLAY_NAME = user.LOGON_NAME;
                ouuser.RANK_NAME = Request["RANK_NAME"];
                ouuser.GLOBAL_SORT = depInfo.GLOBAL_SORT + ouuser.INNER_SORT;
                ouuser.ORIGINAL_SORT = depInfo.ORIGINAL_SORT + ouuser.INNER_SORT;
                ouuser.ALL_PATH_NAME = depInfo.ALL_PATH_NAME + "\\" + ouuser.DISPLAY_NAME;
                 
               
                if (userManager.Update(user) && ouuserManager.Update(ouuser))
                    result = true;
            }

            if (useProce == "true")
            { 
                var sex = user.SEX == 1 ? "男" : "女";
                string sql = "exec " + dataBaseName + ".[dbo].[udp_Emp] @returnvalue  = 1 , @EmpNo= '" + user.GUID + "' ,@EmpName = '" + user.LOGON_NAME + "' ,@EmpSex = '" + user.SEX + "' , @EmpGrpdate = '" + Convert.ToDateTime(user.JOINCOMTIME).ToString("yyyy-MM-dd HH:mm:ss") + "', @DptID = '" + depID + "' , @CMD = 'U'";
                SqlHelp.RunSql(sql);
            }
             

            return DisplayJson(result);
        }

        //删除机构人员数据
        public ContentResult DelUserInfo()
        {
            var id = Request["id"];
            var sideLine = int.Parse(Request["sideLine"]);
            OU_USERSManager ouuserManager = new OU_USERSManager();
            var sql = "delete from OU_USERS where user_guid = '{0}' and SIDELINE= {1} ";
            sql = string.Format(sql, id, sideLine);
            SqlHelp.RunSql(sql);
            //var data = ouuserManager.GetData().Where(m => m.USER_GUID == id).FirstOrDefault();
            //data.STATUS = 4; 
            return DisplayJson(true);
        }

        #endregion

        #region 部门相关

        //获取部门
        public ContentResult GetDepInfo()
        {
            var id = Request["id"];
            var data = new ORGANIZATIONSManager().GetEntity(id);
            return DisplayJson(data);
        }

        //保存部门
        public ContentResult SaveDepInfo()
        {
            var parentId = Request["parentDepID"];
            var sort = Request["sort"];
            var depID = Request["id"];
            var depName = Request["depName"];
            var result = false;


            ORGANIZATIONS org = new ORGANIZATIONS();
            ORGANIZATIONSManager orgManager = new ORGANIZATIONSManager();
            var parentDepInfo = orgManager.GetEntity(parentId);
            var depInfo = orgManager.GetEntity(depID);

            if (depInfo == null)
            {
                org.GUID = Guid.NewGuid().ToString();
                org.DISPLAY_NAME = org.OBJ_NAME = depName;
                org.PARENT_GUID = parentId;
                org.RANK_CODE = "";
                org.INNER_SORT = sort.PadLeft(6, '0');
                org.ORIGINAL_SORT = org.GLOBAL_SORT = parentDepInfo.GLOBAL_SORT + org.INNER_SORT;
                org.ALL_PATH_NAME = parentDepInfo.ALL_PATH_NAME + "\\" + org.DISPLAY_NAME;
                org.STATUS = 1;
                org.ORG_CLASS = org.ORG_TYPE = org.CHILDREN_COUNTER = 0;
                org.CREATE_TIME = org.MODIFY_TIME = DateTime.Now;

                result = orgManager.Insert(org);
            
            }
            else
            {
                depInfo.ALL_PATH_NAME = depInfo.ALL_PATH_NAME.Replace(depInfo.DISPLAY_NAME, "") + depName;
                depInfo.DISPLAY_NAME = depInfo.OBJ_NAME = depName;
                depInfo.INNER_SORT = sort.PadLeft(6, '0');
                if (parentDepInfo != null)
                {
                    depInfo.GLOBAL_SORT = depInfo.ORIGINAL_SORT = parentDepInfo.GLOBAL_SORT + depInfo.INNER_SORT;
                }
                else
                {
                    depInfo.GLOBAL_SORT = depInfo.INNER_SORT;
                }
                depInfo.MODIFY_TIME = DateTime.Now;

                var ouusersData = new OU_USERSManager().GetOUUser().Where(m => m.PARENT_GUID == depID);
                foreach (var item in ouusersData)
                {
                    item.ALL_PATH_NAME = depInfo.ALL_PATH_NAME + "\\" + item.DISPLAY_NAME;
                    var sql = " update  OU_USERS  set ALL_PATH_NAME = '{0}' where USER_GUID = '{1}' and PARENT_GUID = '{2}' ";
                    sql = string.Format(sql, item.ALL_PATH_NAME, item.USER_GUID, depID);
                    SqlHelp.RunSql(sql);
                } 
                result = orgManager.Update(depInfo);
            }
            try
            {
                if (useProce == "true")
                {
                    var guid = depInfo == null ? org.GUID : depInfo.GUID;
                    var name = depInfo == null ? org.DISPLAY_NAME : depInfo.DISPLAY_NAME;
                    var parentDepID = string.IsNullOrEmpty(parentId) ? "00000000-0000-0000-0000-000000000000" : parentId;
                    string sql = "exec " + dataBaseName + ".[dbo].[udp_Dpt] @returnvalue  = 1 , @DptID= '" + guid + "' ,@DptName = '" + name + "' ,@FthDptID = '" + parentDepID + "' ,@CMD = 'U'";
                    SqlHelp.RunSql(sql);
                }
            }
            catch (Exception ex) {

            }

            return DisplayJson(result);
        }

        //删除部门
        public ContentResult delDepInfo()
        {
            var id = Request["id"];
            ORGANIZATIONSManager orgManager = new ORGANIZATIONSManager();
            var childDepInfo = new ORGANIZATIONSManager().GetDepsByParentID(id); 
            if(childDepInfo.Count() > 0)
            {
                return DisplayJson(false, "该部门存在子部门，不能删除");
            }
            var childUserInfo = new OU_USERSManager().GetUsersListByDepID(id); 
            if (childUserInfo.Count() > 0)
            {
                return DisplayJson(false, "该部门下存在人员，不能删除");
            } 
            var data = orgManager.GetEntity(id);
            var sql = " delete  from  OU_USERS  where PARENT_GUID = '{0}'";
            sql = string.Format(sql, id);
            SqlHelp.RunSql(sql);

            if (useProce == "true")
            {
                sql = "exec " + dataBaseName + ".[dbo].[udp_Dpt] @returnvalue  = 1 , @DptID= '" + id + "' ,@CMD = 'D'";
                SqlHelp.RunSql(sql); 
            }

            return DisplayJson(orgManager.Delete(data));
        }

        //改变通讯录人员部门信息
        public ContentResult ChangeDep()
        {
            var depID = Request["depID"];
            var userID = Request["userID"];
            var sort = Request["sort"];
            var sideLine = Request["isSideLine"] == "true" ? 1 : 0;
            var depData = new ORGANIZATIONSManager().GetEntity(depID);

            OU_USERSManager ouuserManager = new OU_USERSManager();
            var ouUserData = ouuserManager.GetDataByUserID(userID);


            OU_USERS ouuser = new OU_USERS();
            ouuser.USER_GUID = userID;
            ouuser.DISPLAY_NAME = ouuser.OBJ_NAME = Request["LOGON_NAME"];
            if (ouUserData != null && sideLine == 0)
            {
                ouuser.STATUS = ouUserData.STATUS;
                ouuser.START_TIME = ouUserData.START_TIME;
                ouuser.END_TIME = ouUserData.END_TIME;
                ouuser.CREATE_TIME = ouUserData.CREATE_TIME;
                ouuser.RANK_NAME = ouUserData.RANK_NAME;
                var sql = "delete from OU_USERS where user_guid = '" + ouuser.USER_GUID + "' ";
                var result = SqlHelp.LoadDataTable(sql);
            }
            else
            {
                ouuser.STATUS = 1;
                ouuser.START_TIME = Convert.ToDateTime("2000-01-01");
                ouuser.END_TIME = DateTime.MaxValue;
                ouuser.CREATE_TIME = DateTime.Now;
                ouuser.RANK_NAME = "";
            }
            ouuser.SIDELINE = sideLine;
            ouuser.PARENT_GUID = depID;
            ouuser.INNER_SORT = sort.PadLeft(6, '0');
            ouuser.ORIGINAL_SORT = depData.ORIGINAL_SORT + ouuser.INNER_SORT;
            ouuser.GLOBAL_SORT = depData.GLOBAL_SORT + ouuser.INNER_SORT;
            ouuser.ALL_PATH_NAME = depData.ALL_PATH_NAME + "\\" + ouuser.DISPLAY_NAME;
            ouuser.MODIFY_TIME = DateTime.Now;

            return DisplayJson(ouuserManager.Insert(ouuser));
        }


        //保存修改后的部门人员排序

        public ContentResult UpdateSortChange()
        {
            var result = true;
            var parentID = Request["parentID"];
            var tableSortList = (JArray)JsonConvert.DeserializeObject(Request["changeSortList"]);
            var depData = new ORGANIZATIONSManager().GetEntity(parentID);

            OU_USERSManager ouusersManager = new OU_USERSManager();

            var usersData = ouusersManager.GetOUUser().Where(m => m.PARENT_GUID == parentID);

            foreach (var item in tableSortList)
            {
                var userID = item["userID"].ToString();
                var sideLine = int.Parse(item["sideLine"].ToString());
                var userData = usersData.Where(m => m.USER_GUID == userID && m.SIDELINE == sideLine).FirstOrDefault();
                if (userData != null)
                {
                    userData.INNER_SORT = item["sort"].ToString().PadLeft(6, '0');
                    userData.GLOBAL_SORT = userData.ORIGINAL_SORT = depData.GLOBAL_SORT + userData.INNER_SORT;
                    userData.MODIFY_TIME = DateTime.Now;

                    result = ouusersManager.Update(userData);
                    if (!result) break;
                }
            }

            return DisplayJson(result);
        }


        #endregion

    }


}
