using HR.BLL;
using HR.Models;
using Sgms.Frame.Page.MVC;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Admin.Controllers
{
    public class SYS_DICTIONARYController : DataOperationPage<SYS_DICTIONARY>
    {  

        private SYS_DICTIONARYManager _TmpCurManager;
        protected SYS_DICTIONARYManager CurManager
        {


            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as SYS_DICTIONARYManager;
                }
                return _TmpCurManager;

            }
        }



        #region 获取字典数据

        //获取请假类型
        public ContentResult GetDayoffTypeData()
        {
            return DisplayJson(new KQ_DayoffTypeManager().GetDayoffTypeData(Request["TypeNo"]));
        }



        //获取加班类型
        public ContentResult GetOtTypeData()
        { 
            return DisplayJson(new KQ_OtTypeManager().GetOtTypeData(Request["TypeNo"]));
        }


    
        #endregion


        #region 下拉框选择数据

        //字典数据类型
        public ActionResult GetDictionaryData()
        {

            var data = new SYS_DICTIONARYManager().GetDictionarysByItemTypeID(Request["itemTypeID"]);
            return DisplayJson(data);
        }
          
        #endregion


    }

}
