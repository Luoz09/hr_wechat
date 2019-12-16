using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
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
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class ApplyController : DataOperationPage<HR_ApplyInfo>
    {

        protected override string PageCName
        {
            get { return "事务申请"; }
        }

        private HR_ApplyInfoManager _TmpCurManager;
        protected HR_ApplyInfoManager CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as HR_ApplyInfoManager;
                }
                return _TmpCurManager;
            }
        }


        string Web_Host = AppSettingUtil.GetAppSetting("Web_Host");
        string dataKey = AppSettingUtil.GetAppSetting("proce");
        //string Web_Host = "http://localhost:52996";

        // 事务申请类型（  请假 ： 1 ; 外出 ：2 ; 出差 ： 3 ;  加班 : 4 ; 补卡 : 5  ）


        #region 请假申请
        public ActionResult LeaveIndex()
        {
            return base.Index();
        }
         
        public virtual ActionResult LeaveCreate()
        {
            Nav = GetAddNav();
            ViewBag.Nav = Nav;
            return View("LeaveEdit", CurEntity);
        }

        [HttpPost]
        public virtual ContentResult LeaveCreate(FormCollection collection)
        {
            CurEntity.ApplyType = 1;

            var startDate = Convert.ToDateTime(collection["StartTime"]);
            var endDate   = Convert.ToDateTime(collection["EndTime"]);

            CurEntity.ApplyCode = DateTime.Now.ToString("yyyyMMddhhmmss");

            string sql = "exec " + dataKey + ".[dbo].[udp_DJ] @returnvalue  = 1 , @EmpNo = '" + collection["ApplyUserID"] + "' ,@DJBDate = '" + startDate.ToString("yyyy-MM-dd")  + "' ,@DJEDate = '" + endDate.ToString("yyyy-MM-dd")+ "' , @BTime = '" + startDate.ToString("HH:mm") + "' , @ETime = '" + endDate.ToString("HH:mm") + "' ,@BillType = '" +collection["LeaveType"] + "',@Reason= '"+collection["ApplyReason"] + "' , @AddDay = '"+DateTime.Now+ "' , @BillNo= '" + CurEntity.ApplyCode + "'";
            SqlHelp.RunSql(sql);

            return Create(collection);
        }


        public ActionResult LeaveEdit(string id)
        { 
            Nav = GetEditNav();
            ViewBag.Nav = Nav;
            return View(CurManager.GetEntity(id));  
        }

        [HttpPost]
        public ActionResult LeaveEdit(string id, FormCollection collection)
        {  
            return Edit(id,collection); 
        }


        #endregion


        #region 外出申请
        public ActionResult OutIndex()
        {
            return base.Index();
        }

        public virtual ActionResult OutCreate()
        {
            Nav = GetAddNav();
            ViewBag.Nav = Nav;
            return View("OutEdit", CurEntity);
        }

        [HttpPost]
        public virtual ContentResult OutCreate(FormCollection collection)
        {
            CurEntity.ApplyType = 2;

            CurEntity.ApplyCode = DateTime.Now.ToString("yyyyMMddhhmmss");
            return Create(collection);
        }


        public ActionResult OutEdit(string id)
        {
            Nav = GetEditNav();
            ViewBag.Nav = Nav;
            return View(CurManager.GetEntity(id));
        }

        [HttpPost]
        public ActionResult OutEdit(string id, FormCollection collection)
        {
            return Edit(id, collection);
        }

        #endregion


        #region 出差申请
        public ActionResult BusinessIndex()
        {
            return base.Index();
        }

        public virtual ActionResult BusinessCreate()
        {
            Nav = GetAddNav();
            ViewBag.Nav = Nav;
            return View("BusinessEdit", CurEntity);
        }

        [HttpPost]
        public virtual ContentResult BusinessCreate(FormCollection collection)
        {
            CurEntity.ApplyType = 3;
            CurEntity.ApplyCode = DateTime.Now.ToString("yyyyMMddhhmmss");
            return Create(collection);
        }

        public ActionResult BusinessEdit(string id)
        {
            Nav = GetEditNav();
            ViewBag.Nav = Nav;
            return View(CurManager.GetEntity(id));
        }

        [HttpPost]
        public ActionResult BusinessEdit(string id, FormCollection collection)
        {
            return Edit(id, collection);
        }


        #endregion


        #region 加班申请
        public ActionResult ExtWorkIndex()
        {
            return base.Index();
        }

        public virtual ActionResult ExtWorkCreate()
        {
            Nav = GetAddNav();
            ViewBag.Nav = Nav;
            return View("ExtWorkEdit", CurEntity);
        }

        [HttpPost]
        public virtual ContentResult ExtWorkCreate(FormCollection collection)
        {
            CurEntity.ApplyType = 4;

            CurEntity.ApplyCode = DateTime.Now.ToString("yyyyMMddhhmmss");

            string sql = "exec " + dataKey + ".[dbo].[udp_JB] @returnvalue  = 1 , @EmpNo = '" + collection["ApplyUserID"] + "' ,@BDateTime = '" + collection["StartTime"] + "' ,@EDateTime = '" + collection["EndTime"] + "' , @BillType = '" + collection["ExtWorkType"] + "' , @IsResult = '" + collection["ExtWorkIsOut"] + "' ,@Addday = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' , @BIllNo = '"+CurEntity.ApplyCode+"'";
            SqlHelp.RunSql(sql);

            return Create(collection);
        }


        public ActionResult ExtWorkEdit(string id)
        {
            Nav = GetEditNav();
            ViewBag.Nav = Nav;
            return View(CurManager.GetEntity(id));
        }

        [HttpPost]
        public ActionResult ExtWorkEdit(string id, FormCollection collection)
        {
            return Edit(id, collection);
        }

        #endregion


        #region 补卡申请
        public ActionResult SuppleCardIndex()
        {
            return base.Index();
        }


        public virtual ActionResult SuppleCardCreate()
        {
            Nav = GetAddNav();
            ViewBag.Nav = Nav;
            return View("SuppleCardEdit", CurEntity);
        }


        [HttpPost]
        public virtual ContentResult SuppleCardCreate(FormCollection collection)
        {
            CurEntity.ApplyType = 5;
            CurEntity.ApplyCode = DateTime.Now.ToString("yyyyMMddhhmmss");
            string sql = "exec " + dataKey + ".[dbo].[udp_BK] @returnvalue  = 1 , @EmpNo = '" + collection["ApplyUserID"] + "' ,@BDate = '" + collection["StartTime"] + "' ,@EDate = '" + collection["EndTime"] + "' , @KqTime = '" + collection["SuppleTime"] + "' , @Res = '"+collection["ApplyReason"]+ "' ,@DevFlag = '"+collection["SuppleType"]+"'";
            SqlHelp.RunSql(sql);

            return Create(collection);
        }


        public ActionResult SuppleCardEdit(string id)
        {
            Nav = GetEditNav();
            ViewBag.Nav = Nav;
            return View(CurManager.GetEntity(id));
        }

        [HttpPost]
        public ActionResult SuppleCardEdit(string id, FormCollection collection)
        {
            return Edit(id, collection);
        }

        #endregion


        public override ContentResult GetData()
        {
            EDMFilter filter = new EDMFilter();
            var applyType = Convert.ToInt32(Request["ApplyType"]);
            var data = CurManager.GetApplyData().Where(m=>m.ApplyType == applyType && (m.InputUserID == SysPara.CurAdmin.ID || m.ApplyUserID == SysPara.CurAdmin.ID));
            var result = filter.Filter(data, FilterInfo);
            return DisplayDataOfTotalWithDataCount(result, filter.LastCount);
        }


        public override ContentResult Create(FormCollection collection)
        {
            FillEntity(CurEntity);

            CurEntity.ApplyID = Guid.NewGuid().ToString(); 
            CurEntity.InputUserID = CurEntity.LastModifyUserID = SysPara.CurAdmin.ID;
            CurEntity.InputUserName = CurEntity.LastModifyUserName = SysPara.CurAdmin.UserName;
            CurEntity.InputDate = CurEntity.LastModifyDate = DateTime.Now;
             
            CurEntity.InputUserDepID = new OU_USERSManager().GetDataByUserID(SysPara.CurAdmin.ID).PARENT_GUID; 

            CurEntity.Status = 1;
            new AttachController().FilesSave(Request["Files"], CurEntity.ApplyID, Request["UploadTime"]);

            //var result = true;
            var result = CurManager.Insert(CurEntity);
            if (result)
            {
                var WFData = GetWFData(CurEntity.ApplyID, CurEntity.ApplyType);
                string url = Web_Host + "/WebApi/WorkFlow/WriteWFEngine";
                var res = WebUtil.Post(url, "EngineData=" + WFData + "");
            }
            return DisplayJson(result, CurManager.RunMessage.ToOnelineString()); 
        }

        public override ActionResult Edit(string id, FormCollection collection)
        {
            FillEntity(CurEntity);
               
            CurEntity.LastModifyUserID = SysPara.CurAdmin.ID;
            CurEntity.LastModifyUserName = SysPara.CurAdmin.UserName;
            CurEntity.LastModifyDate = DateTime.Now;


            new AttachController().FilesSave(Request["Files"], CurEntity.ApplyID, Request["UploadTime"]);

            var result = CurManager.Update(CurEntity);

            //var result = true;
            if (result)
            {
                var WFData = GetWFData(CurEntity.ApplyID, CurEntity.ApplyType);
                string url = Web_Host + "/WebApi/WorkFlow/WriteWFEngine";
                var res = WebUtil.Post(url, "EngineData=" + WFData + "");
                if (res == "success" && Request["IsProcessInstanceEnd"] == "false" && Request["IsWithdraw"] == "false")
                {
                    CurEntity.Status = 4;
                }
                return DisplayJson(CurManager.Update(CurEntity), CurManager.RunMessage.ToOnelineString());
            }

            return DisplayJson(result, CurManager.RunMessage.ToString());
        }

        #region WF提交数据json
        public string GetWFData(string id ,int? applytype)
        {
            WFSubmit wfsubmit = new WFSubmit();
            FILL(wfsubmit);
            wfsubmit.UserID = SysPara.CurAdmin.ID;
            wfsubmit.UserName = SysPara.CurAdmin.DisplayName;
            var user = new OU_USERSManager().GetDataByUserID(wfsubmit.UserID);
               
            //wfsubmit.UserName = Request["UserNames"];
               
            wfsubmit.UserFullPath = user.ALL_PATH_NAME;
            wfsubmit.DepID = user.PARENT_GUID;
            wfsubmit.ResourceID = id;

            switch (applytype) {
                case 1: wfsubmit.TaskUrl = "/Admin/Apply/LeaveEdit?id=" + id; break;
                case 2: wfsubmit.TaskUrl = "/Admin/Apply/OutEdit?id=" + id; break; 
                case 3: wfsubmit.TaskUrl = "/Admin/Apply/BusinessEdit?id=" + id; break;
                case 4: wfsubmit.TaskUrl = "/Admin/Apply/ExtWorkEdit?id=" + id; break;
                case 5: wfsubmit.TaskUrl = "/Admin/Apply/SuppleCardEdit?id=" + id; break;
            }
         
            var usernames = Request["NextUsers"].Split(',');
            var guids = Request["NextUsersGUID"].Split(',');
            var fullpaths = Request["NextUsersFullPath"].Split(',');
            var nextActivities = Request["NextActivity"].Split(',');

            //下一步活动
            //var nextactivities = Request["NextStepActivity"].Split(',');
            JArray JActivities = new JArray();
            foreach (var item in nextActivities)
            {
                JArray JUsers = new JArray();
                for (var i = 0; i < usernames.Length; i++)
                {
                    JObject jobject = new JObject();
                    jobject.Add("UserID", guids[i]);
                    jobject.Add("UserName", usernames[i]);
                    jobject.Add("AllPathName", fullpaths[i]);
                    JUsers.Add(jobject);
                }

                JObject jobject1 = new JObject();
                jobject1.Add("SelectedNextActivity", item);
                jobject1.Add("Users", JUsers);

                JActivities.Add(jobject1);
            }

            wfsubmit.Activities = JActivities;

            JArray JWFRelativeDatas = new JArray();

            wfsubmit.WFRelativeDatas = JWFRelativeDatas;

            return JsonConvert.SerializeObject(wfsubmit); ;
        }

        public void FILL(WFSubmit wfsubmit)
        {
            IEnumerable<PropertyInfo> properInfos = wfsubmit.GetType().GetProperties().Where(m => !NonAutoPopField.Contains(m.Name) && m.Name != SysKeys.KEY_ID);

            foreach (var elem in properInfos)
            {
                string value = Request[elem.Name];
                if (value != null)
                {
                    try
                    {
                        elem.SetValue(wfsubmit, Convert.ChangeType(value, Nullable.GetUnderlyingType(elem.PropertyType) ?? elem.PropertyType), null);
                    }
                    catch
                    {
                        try
                        {
                            elem.SetValue(wfsubmit, elem.PropertyType.IsValueType ? Activator.CreateInstance(elem.PropertyType) : null, null);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        #endregion



    }
}
