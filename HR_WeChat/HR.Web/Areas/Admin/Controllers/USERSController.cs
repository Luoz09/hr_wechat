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

namespace HR.Web.Areas.Admin.Controllers
{
    public class USERSController : DataOperationPage<USERS>
    {

        protected override string PageCName
        {
            get { return "人员信息"; }
        }

        private USERSManager _TmpCurManager;
        protected USERSManager CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as USERSManager;
                }
                return _TmpCurManager;
            }
        }

        protected override Sgms.Frame.AuthenticateLevel CurAuthenticateLevel
        {
            get
            {
                return Sgms.Frame.AuthenticateLevel.Simple;
            }
        }

        string dataBaseName = AppSettingUtil.GetAppSetting("dataBaseName");
        string useProce = AppSettingUtil.GetAppSetting("useProce");

        public override ContentResult Create(FormCollection collection)
        {
            USERS users = new USERS();
              
            FillEntity(users);

            users.GUID = Guid.NewGuid().ToString();
            users.USER_PWD = EncryptUtil.PwdCalculate("", collection["USER_PWD"]);
            users.PWD_TYPE_GUID = "21545d16-a62f-4a7e-ac2f-beca958e0fdf";
            users.CREATE_TIME = users.MODIFY_TIME = DateTime.Now;
            users.JOINCOMTIME = DateTime.Now;
            //users.FIRST_NAME = collection["FIRST_NAME"];
            //users.LAST_NAME = collection["LAST_NAME"];
            //users.LOGON_NAME = collection["LOGON_NAME"];
            //users.MOBILE = collection["MOBILE"];
            users.RANK_CODE = collection["RANK_NAME"];
            users.POSTURAL = 4;
            users.RANK_CODE = "";
            users.QQ = users.EmergencyContact = users.EmergencyTel = users.OFVIRTEL = "";
            users.SEX = int.Parse(collection["Sex"]);

            
            if (useProce == "true")
            {
                var sex = users.SEX == 1 ? "男" : "女";
                var depID = new ORGANIZATIONSManager().GetTopDep().GUID;
                string sql = "exec " + dataBaseName + ".[dbo].[udp_Emp] @returnvalue  = 1 , @EmpNo= '" + users.GUID + "' ,@EmpName = '" + users.LOGON_NAME + "' ,@EmpSex = '" + sex + "' , @EmpGrpdate = '" + Convert.ToDateTime(users.JOINCOMTIME).ToString("yyyy-MM-dd HH:mm:ss") + "', @DptID = '" + depID + "',  @CMD = 'U'";
                SqlHelp.RunSql(sql);
            }


            return DisplayJson(CurManager.Insert(users), CurManager.RunMessage.ToOnelineString());
        }

        public override ActionResult Edit(string id, FormCollection collection)
        {
            FillEntity(CurEntity);

            if (collection["USER_PWD"] != CurEntity.USER_PWD)
            {
                CurEntity.USER_PWD = EncryptUtil.PwdCalculate("", collection["USER_PWD"]);
            }
            if(CurEntity.JOINCOMTIME==null)
            {
                CurEntity.JOINCOMTIME = DateTime.Now;
            }
            CurEntity.MODIFY_TIME = DateTime.Now;
             

            if (useProce == "true")
            {
                var sex = CurEntity.SEX == 1 ? "男" : "女";
                var depID = new ORGANIZATIONSManager().GetTopDep().GUID; 
                string sql = "exec " + dataBaseName + ".[dbo].[udp_Emp] @returnvalue  = 1 , @EmpNo= '" + CurEntity.GUID + "' ,@EmpName = '" + CurEntity.LOGON_NAME + "' ,@EmpSex = '" + sex + "' , @EmpGrpdate = '" + Convert.ToDateTime(CurEntity.JOINCOMTIME).ToString("yyyy-MM-dd HH:mm:ss") + "', @DptID = '" + depID + "', @CMD = 'U'";
                SqlHelp.RunSql(sql);
            }


            return DisplayJson(CurManager.Update(CurEntity), CurManager.RunMessage.ToOnelineString());
        }

        public override ActionResult Delete(string ids, FormCollection collection)
        {
            var result = true;
            foreach (var id in ids.Split(','))
            {
                OU_USERSManager ouuserManager = new OU_USERSManager();
                var data = ouuserManager.GetDataByUserID(id);
                if (data != null)
                {
                    data.STATUS = 4;
                    result = ouuserManager.Update(data);
                }
                if (useProce == "true")
                {
                    string sql = "exec " + dataBaseName + ".[dbo].[udp_Emp] @returnvalue  = 1 , @EmpNo= '" + id + "', @CMD = 'D'";
                    SqlHelp.RunSql(sql);
                }
            }
            if (result)
            { 
                return base.Delete(ids, collection);
            }
            else
            {
                return View();
            }
        }

        public static string DateFormat(DateTime? date)
        {
            if (!string.IsNullOrEmpty(date.ToString()))
            {
                return DateTime.Parse(date.ToString()).ToString("yyyy-MM-dd");
            }
            return "";
        }
    }
}
