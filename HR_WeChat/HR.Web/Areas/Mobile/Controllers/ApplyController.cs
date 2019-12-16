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
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace HR.Web.Areas.Mobile.Controllers
{
    public class ApplyController : DataOperationPage<HR_ApplyInfo>
    {

        HR_ApplyInfoManager CurManager = new HR_ApplyInfoManager();


        public ContentResult GetApplyData()
        { 
            var type = Convert.ToInt32(Request["Type"]);
            var page = Convert.ToInt32(Request["Page"]);
            var str = Request["SearchStr"];

            var data = CurManager.GetMobileApplyData(type);
            EDMFilter filter = new EDMFilter();
            FilterInfo.CurPage = page;

            //获取过滤过后的数据
            FilterInfo.SortField = "InputDate";
            FilterInfo.IsDesc = true;
            FilterInfo.SearchStr = str;
            var result = filter.Filter(data, FilterInfo);

            //获取过滤后的数据总数
            FilterInfo.PageSize = data.Count();
            var count  = filter.Filter(data,FilterInfo).Count();  

            return DisplayDataOfTotalWithDataCount(result,count);

        }



    }
}
