using Newtonsoft.Json;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;
using HR.Models;
using Sgms.Frame.Utils;
using Sgms.Frame;

namespace HR.Web.Areas.Admin.Controllers
{
    public class CardRecordController : DataOperationPage<HR_Check>
    {

        protected override string PageCName
        {
            get { return "打卡记录"; }
        }

        public ActionResult AdminRecord()
        {
            return base.Index();
        }
         

    }


}
