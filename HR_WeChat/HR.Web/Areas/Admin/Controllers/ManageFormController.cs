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
    public class ManageFormController : DataOperationPage<HR_ApplyInfo>
    {
          

        public ActionResult Apply()
        {
            return base.Index();
        }

        public ActionResult Check()
        {
            return base.Index();
        }

        public ActionResult LeaveManage()
        {
            return base.Index();
        }

        public ActionResult BusinessManage()
        {
            return base.Index();
        }

        public ActionResult ExtWorkManage()
        {
            return base.Index();
        }

        public ActionResult OutManage()
        {
            return base.Index();
        }

        public ActionResult SuppleCardManage()
        {
            return base.Index();
        }


        public override ContentResult GetData()
        {
            EDMFilter filter = new EDMFilter();
            var applyType = Convert.ToInt32(Request["ApplyType"]);
            var data = new HR_ApplyInfoManager().GetApplyData().Where(m => m.ApplyType == applyType);
            var result = filter.Filter(data, FilterInfo);
            return DisplayDataOfTotalWithDataCount(result, filter.LastCount);
        }

        public ContentResult GetCheckData()
        {
            var advanced = Request["advanced"]; 
            var checkName = "";
            var minDate = "";
            var maxDate = "";
            if (!string.IsNullOrEmpty(advanced))
            {
                JArray jobject = (JArray)JsonConvert.DeserializeObject(advanced);
                foreach (var item in jobject)
                {
                    if (item["Field"].ToString() == "LOGON_NAME")
                    {
                        checkName = item["SearchStr"].ToString();
                    }

                    if (item["Field"].ToString() == "KqDate")
                    {
                        minDate = item["MinValue"].ToString();
                        maxDate = item["MaxValue"].ToString();
                    }
                }
            }

            return DisplayJson(new KQ_KqDataManager().GetKQDataCount(checkName,minDate,maxDate));

        }


        public ContentResult GetApplyTypeCount()
        {
            var advanced =  Request["advanced"];
            var applyName = "";
            var minDate = "";
            var maxDate = "";
            if (!string.IsNullOrEmpty(advanced))
            {
                JArray jobject = (JArray)JsonConvert.DeserializeObject(advanced);
                foreach (var item in jobject)
                {
                    if (item["Field"].ToString() == "ApplyUserName")
                    {
                        applyName = item["SearchStr"].ToString();
                    }

                    if (item["Field"].ToString() == "InputDate")
                    {
                        minDate = item["MinValue"].ToString();
                        maxDate = item["MaxValue"].ToString();
                    }
                }
            }
            return DisplayJson(new HR_ApplyInfoManager().GetApplyTypeCount(applyName,minDate,maxDate));
        }

         

    }
}
