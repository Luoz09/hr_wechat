using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.BLL;
using Sgms.Frame.EDM.DAL;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class ORGANIZATIONSController : DataOperationPage<ORGANIZATIONS>
    {
        protected override string PageCName
        {
            get { return "部门";
            }
        }


        private ORGANIZATIONSManager _TmpCurManager;
        protected ORGANIZATIONSManager CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as ORGANIZATIONSManager;
                }
                return _TmpCurManager;
            }
        }


        string dataBaseName = AppSettingUtil.GetAppSetting("dataBaseName");
        string useProce = AppSettingUtil.GetAppSetting("useProce");

        public override ContentResult GetData()
        {
            EDMFilter filter = new EDMFilter();
            var data = CurManager.GetDepInfo(); 
            var result = filter.Filter(data, FilterInfo);
            return DisplayDataOfTotalWithDataCount(result, filter.LastCount); 
        }


        public ContentResult GetDepByGuid()
        {
            return DisplayJson(CurManager.GetDepByDepID(Request["Guid"]).FirstOrDefault());
        }

        public override ContentResult Create(FormCollection collection)
        {

            FillEntity(CurEntity);
              
            CurEntity.GUID = Guid.NewGuid().ToString();


            //存储过程传入父部门
            var FthDptID = CurEntity.PARENT_GUID;
             
            CurEntity.STATUS = 1;
            CurEntity.OBJ_NAME = CurEntity.DISPLAY_NAME;

            CurEntity.GLOBAL_SORT = CurEntity.ORIGINAL_SORT = CurEntity.INNER_SORT;
            CurEntity.CREATE_TIME = CurEntity.MODIFY_TIME = DateTime.Now;
            CurEntity.RANK_CODE = "";


            //是否对部门进行修改
            if (!string.IsNullOrEmpty(Request["allPathName"]))
            {
                CurEntity.ALL_PATH_NAME = Request["allPathName"] + "\\" + CurEntity.DISPLAY_NAME;
            }
             
             
            // 判断是否选择了父部门  , 没有选择父部门则为根部门
            if (string.IsNullOrEmpty(Request["ParentName"]))
            {
                CurEntity.PARENT_GUID = "";
                CurEntity.ALL_PATH_NAME = CurEntity.DISPLAY_NAME;
                FthDptID = "0";

            }

            if (useProce == "true")
            {
                string sql = "exec " + dataBaseName + ".[dbo].[udp_Dpt] @returnvalue  = 1 , @DptID= '" + CurEntity.GUID + "' ,@DptName = '" + CurEntity.DISPLAY_NAME + "' ,@FthDptID = '" + FthDptID + "' ,@CMD = 'U'";
                SqlHelp.RunSql(sql);
            }

             
            return DisplayJson(CurManager.Insert(CurEntity), CurManager.RunMessage.ToOnelineString());
        }


        public override ActionResult Edit(string id)
        {
            var entity = CurManager.GetEntity(id);

            Nav = GetEditNav();
            ViewBag.Nav = Nav;
             
            var orgData = new ORGANIZATIONSManager().GetDepByDepID(entity.PARENT_GUID).FirstOrDefault();
            if (orgData != null)
            {
                ViewBag.DepName = orgData.DISPLAY_NAME;
                ViewBag.AllPathName = orgData.ALL_PATH_NAME;
            }
            return View(entity);
        }


        public override ActionResult Edit(string id, FormCollection collection)
        {

            FillEntity(CurEntity);

            CurEntity.MODIFY_TIME = DateTime.Now;


            //存储过程传入父部门
            var FthDptID = CurEntity.PARENT_GUID;


            //是否对部门进行修改
            if (!string.IsNullOrEmpty(Request["allPathName"]))
            {
                CurEntity.ALL_PATH_NAME = Request["allPathName"] + "\\" + CurEntity.DISPLAY_NAME;
            } 
              
            // 判断是否选择了父部门  , 没有选择父部门则为根部门
            if (string.IsNullOrEmpty(Request["ParentName"]))
            { 
                CurEntity.PARENT_GUID = "";
                CurEntity.ALL_PATH_NAME = CurEntity.DISPLAY_NAME;
                FthDptID = "0";
                 
            }

            if (useProce == "true")
            {
                string sql = "exec " + dataBaseName + ".[dbo].[udp_Dpt] @returnvalue  = 1 , @DptID= '" + CurEntity.GUID + "' ,@DptName = '" + CurEntity.DISPLAY_NAME + "' ,@FthDptID = '" + FthDptID + "' ,@CMD = 'U'";
                SqlHelp.RunSql(sql);
            }

              
            return DisplayJson(CurManager.Update(CurEntity), CurManager.RunMessage.ToOnelineString());
        }


        public override ActionResult Delete(string ids, FormCollection collection)
        {
            var idlist = ids.Split(',');
            if (useProce == "true")
            {
                foreach (var item in idlist)
                {
                    string sql = "exec " + dataBaseName + ".[dbo].[udp_Dpt] @returnvalue  = 1 , @DptID= '" + item + "' ,@CMD = 'D'";
                    SqlHelp.RunSql(sql);
                }
            }
            return base.Delete(ids, collection);
        }

       
    }
}
