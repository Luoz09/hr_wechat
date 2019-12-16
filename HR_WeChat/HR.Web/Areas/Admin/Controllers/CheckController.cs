using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.EDM.DAL;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class CheckController : DataOperationPage<KQ_KqData>
    {
        protected override string PageCName
        {
            get { return "考勤"; }
        }


        public ContentResult GetUsersKQData()
        {
            EDMFilter filter = new EDMFilter();
            var data = new KQ_KqDataManager().GetKQDataBySysAdminID();
            var result = filter.Filter(data, FilterInfo);
            return DisplayDataOfTotalWithDataCount(result, filter.LastCount); 
        }

 
        public ActionResult History()
        { 
            return base.Index();
        }



        public ContentResult GetAdminKQData()
        {
            EDMFilter filter = new EDMFilter();
            var data = new KQ_KqDataManager().GetKQData();
            var result = filter.Filter(data, FilterInfo);
            foreach (var item in result)
            {
                item.EmpSysID = new USERSManager().GetEntity(item.EmpSysID).LOGON_NAME;
            }
            return DisplayDataOfTotalWithDataCount(result, filter.LastCount);
        }


        public ActionResult AdminCheck()
        {
            return base.Index();
        }

    }
}
