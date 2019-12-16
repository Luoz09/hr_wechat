using HR.BLL;
using HR.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Sgms.Frame.Page.MVC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Sgms.Frame.Utils;

namespace HR.Web.Areas.Admin.Controllers
{
    public class AttachController : DataOperationPage<MATERIAL>
    {

        private MATERIALManager _TmpCurManager;
        protected MATERIALManager CurManager
        {


            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as MATERIALManager;
                }
                return _TmpCurManager;

            }
        }


        #region 附件上传相关

        public ContentResult GetFiles()
        {
            var id = Request["id"];
            var data = CurManager.GetFilesData(id);
            return DisplayJson(data);
        }


        public ActionResult UpLoad()
        {
            try
            {
                var file = Request.Files[0];
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (CurManager.Upload(file.InputStream, file.FileName, time))
                {
                    return DisplayFileName(CurManager.RunMessage.ToOnelineString(), 0, time);
                }
                return DisplayJson(false, CurManager.RunMessage.ToOnelineString());
            }
            catch (Exception e)
            {
                return DisplayJson(false);
            }
        }

        //附件保存
        public void FilesSave(string files, string resourceid, string uploadtime)
        {
            if (!string.IsNullOrEmpty(files))
            {
                files = files.Substring(0, files.Length - 1);

                for (var i = 0; i < files.Split(',').Length; i++)
                {
                    MATERIAL material = new MATERIAL();
                    material.ID = Guid.NewGuid().ToString();
                    material.RESOURCE_ID = resourceid;
                    material.SORT_ID = i;
                    material.TITLE = files.Split(',')[i];
                    var title = files.Split(',')[i].Replace(files.Split(',')[i].Split('.')[0], uploadtime);
                    material.FILE_PATH = "/uploads/" + DateTime.Now.ToString("yyyyMMdd") + "/" + title + "";
                    material.LAST_MODIFY_TIME = DateTime.Now;

                    CurManager.Insert(material);
                }
            }

        }

        public ActionResult DeleteFile()
        {
            if (CurManager.DeleteFile(Request["fileName"], Request["key"]))
            {
                return DisplayJson(true, String.Empty);
            }
            return DisplayJson(false, "删除失败");
        }


        private ActionResult DisplayFileName(string fileName, int code, string time)
        {
            return DisplayJson(new
            {
                srcName = Request.Files[0].FileName,
                fileName = fileName,
                code = code,
                uploadtime = time,
            });
        }
        #endregion




        #region 数据导入


        private struct OrgList
        {
            public string orgName;
            public string allPathName;
            public int sort;
        }

        private struct UserList
        {
            public string userName;
            public string mobile;
            public string parentDepName;
            public string allPathName;
            public int isSideLine;
        }

        private class orgSortCompare : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((OrgList)x).sort - ((OrgList)y).sort;
            }

        }

        public ContentResult Import()
        { 
            var result = false;
            var list = new List<OrgList>();
            ArrayList arrList = new ArrayList();
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(Request.Files[0].InputStream);
                ISheet sheet = hssfworkbook.GetSheetAt(0);
                IEnumerator rows = sheet.GetRowEnumerator();

                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                var type = Request["type"];
                  
                if (type == "org")   // 部门数据模板： 部门名称 -- 全路径
                {
                    //遍历每一行的数据
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    { 
                        IRow row = sheet.GetRow(i); 

                        OrgList orgList = new OrgList();
                        orgList.orgName = row.GetCell(0).ToString();
                        orgList.allPathName = row.GetCell(1).ToString(); 
                        //计算部门的层级
                        orgList.sort = orgList.allPathName.Split('\\').Length;

                        arrList.Add(orgList);  
                    }
                    arrList.Sort(new orgSortCompare());
                    result = ImportOrgList(arrList);
                }
                else
                {   // 人员数据模板： 用户名 -- 电话 -- 所属部门 -- 全路径 -- 是否兼职 (1:是 0:否)
                    //遍历每一行的数据
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    { 
                        IRow row = sheet.GetRow(i); 

                        UserList userList = new UserList();
                        userList.userName = row.GetCell(0).ToString();
                        userList.mobile = row.GetCell(1).ToString();
                        userList.parentDepName = row.GetCell(2).ToString();
                        userList.allPathName = row.GetCell(3).ToString();
                        userList.isSideLine = int.Parse(row.GetCell(4).ToString()); 
                        arrList.Add(userList);
                    }
                    result = ImportUserList(arrList);
                }
                 
                return DisplayJson(result);
            }
            catch (Exception ex)
            {
                return DisplayJson(result);
            }

        }


        //导入部门数据
        public bool ImportOrgList(ArrayList list)
        {
            var result = true;
            JArray jarray = (JArray)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(list));
            try
            {
                foreach (var item in jarray)
                {
                    var allPathName = item["allPathName"].ToString();
                    int sort = int.Parse(item["sort"].ToString());
                    var orgName = item["orgName"].ToString();
                    ORGANIZATIONS org = new ORGANIZATIONS();
                    org.GUID = Guid.NewGuid().ToString();
                    org.DISPLAY_NAME = org.OBJ_NAME = orgName;
                    org.ALL_PATH_NAME = allPathName;
                    org.CREATE_TIME = org.MODIFY_TIME = DateTime.Now;
                    org.RANK_CODE = "COMMON_D";
                    org.STATUS = 1;


                    //判断是否存在相同部门名称、全路径的数据
                    var checkData = new ORGANIZATIONSManager().GetDepInfo().Where(m => m.DISPLAY_NAME == orgName && m.ALL_PATH_NAME == allPathName).FirstOrDefault();
                    if (checkData != null) continue;

                    if (sort == 1) //判断是否为根部门
                    {
                        org.PARENT_GUID = null;
                        //获取根部门的排序
                        var rootSort = new ORGANIZATIONSManager().GetDepInfo().Where(m => string.IsNullOrEmpty(m.PARENT_GUID)).Max(m => m.INNER_SORT);
                        org.INNER_SORT = org.GLOBAL_SORT = org.ORIGINAL_SORT = rootSort.PadLeft(6, '0');
                    }
                    else
                    {
                        //判断部门数据的父部门是否存在
                        var depInfo = new ORGANIZATIONSManager().GetDepInfo().Where(m => (m.ALL_PATH_NAME + "\\" + orgName) == allPathName).FirstOrDefault();
                        if (depInfo == null) continue;
                        org.PARENT_GUID = depInfo.GUID;
                        var maxSort = new ORGANIZATIONSManager().GetDepInfo().Where(m => m.PARENT_GUID == depInfo.GUID).Max(m => m.INNER_SORT);
                        var Sort = int.Parse(maxSort == null ? "0" : maxSort) + 1;
                        org.INNER_SORT = Sort.ToString().PadLeft(6, '0');
                        org.GLOBAL_SORT = org.ORIGINAL_SORT = depInfo.INNER_SORT + org.INNER_SORT;
                    }

                    result = new ORGANIZATIONSManager().Insert(org);

                }
                return result;
            }
            catch {
                return false;
            }
          
        }


        //导入人员数据
        public bool ImportUserList(ArrayList list)
        {
            var result = true;
            JArray jarray = (JArray)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(list));
            try
            {
                foreach (var item in jarray)
                {
                    var userName = item["userName"].ToString();
                    var mobile = item["mobile"].ToString();
                    var parentDepName = item["parentDepName"].ToString();
                    var allPathName = item["allPathName"].ToString();
                    var isSideLine = int.Parse(item["isSideLine"].ToString());

                    var userInfo = new USERSManager().GetUsersData().Where(m => m.LOGON_NAME == userName).FirstOrDefault();
                    USERS user = new USERS();
                    //判断人员是否存在
                    if (userInfo == null)
                    { 
                        user.GUID = Guid.NewGuid().ToString();
                        user.FIRST_NAME = user.LAST_NAME = user.LOGON_NAME = userName;
                        user.PWD_TYPE_GUID = "21545d16-a62f-4a7e-ac2f-beca958e0fdf";
                        user.USER_PWD = EncryptUtil.PwdCalculate("", "000000");
                        user.RANK_CODE = "COMMON_D";
                        user.CREATE_TIME = user.MODIFY_TIME = DateTime.Now;
                        user.MOBILE = mobile;

                        result = new USERSManager().Insert(user);
                    }


                    //判断人员所在部门是否存在
                    var depInfo = new ORGANIZATIONSManager().GetDepInfo().Where(m => m.DISPLAY_NAME == parentDepName && m.STATUS == 1 && (m.ALL_PATH_NAME + "\\" + userName) == allPathName ).FirstOrDefault();
                    if (depInfo == null) continue;

                    var userID = userInfo == null ? user.GUID : userInfo.GUID;

                    //判断是否已经存在对应的数据
                    var ouuserInfo = new OU_USERSManager().GetOUUser().Where(m => m.USER_GUID == userID && m.PARENT_GUID == depInfo.GUID).FirstOrDefault();
                    if (ouuserInfo != null) continue;
     
                    OU_USERS ouuser = new OU_USERS();
                    ouuser.PARENT_GUID = depInfo.GUID;
                    ouuser.USER_GUID = userID;
                    ouuser.DISPLAY_NAME = ouuser.OBJ_NAME = userName;

                    var userMaxSort = new OU_USERSManager().GetUsersListByDepID(depInfo.GUID).Max(m => m.INNER_SORT);
                    var Sort = int.Parse(userMaxSort) + 1;
                    ouuser.INNER_SORT = Sort.ToString().PadLeft(6,'0');
                    ouuser.GLOBAL_SORT = ouuser.ORIGINAL_SORT = depInfo.INNER_SORT + ouuser.INNER_SORT;
                    ouuser.ALL_PATH_NAME = allPathName;
                    ouuser.STATUS = 1;
                    ouuser.CREATE_TIME = ouuser.MODIFY_TIME = DateTime.Now;
                    ouuser.SIDELINE = isSideLine;
                    ouuser.START_TIME = Convert.ToDateTime("2000-01-01");
                    ouuser.END_TIME = DateTime.MaxValue;

                    result = new OU_USERSManager().Insert(ouuser);
                }
                return result;
            }
            catch {
                return false;
            }
        } 


        #endregion
    }  
}