using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class TASK_LIST_VIEWManager : OperationManager<TASK_LIST_VIEW>
    {
        private TASK_LIST_VIEWService _TmpCurService;
        private TASK_LIST_VIEWService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as TASK_LIST_VIEWService;
                }
                return _TmpCurService;
            }
        }


        #region 待办事项

        //个人全部待办事项
        public List<TASK_LIST_VIEW> GetWorkItem(string userid, string application)
        {
            var data = CurService.GetData().Where(m => m.DESTINATION == userid && (m.URL.ToLower().Contains("admin/") || m.URL.ToLower().Contains("mobile/")) && m.APPLICATION_NAME.ToLower() == application.ToLower()).OrderByDescending(m => m.DELIVER_TIME).ToList();
            return data;

        }

        //个人分类待办事项数
        public int GetWorkItemCount(string userid, string applicationname)
        {

            var data = CurService.GetData().Where(m => m.DESTINATION == userid && (m.URL.ToLower().Contains("admin/") || m.URL.ToLower().Contains("mobile/")) && m.APPLICATION_NAME.ToLower() == applicationname.ToLower());
            return data.Count();
            //return data.Where(m => m.APPLICATION_NAME == applicationname).Count();
        }
        #endregion

        #region 在办事项

        //个人全部在办事项
        public List<TASK_LIST_VIEW> GetWorkIteming(string userid, string application)
        {
            var data = CurService.GetData().Where(m => m.SOURCE == userid && (m.URL.ToLower().Contains("admin/") || m.URL.ToLower().Contains("mobile/")) && m.APPLICATION_NAME == application).OrderByDescending(m => m.DELIVER_TIME).ToList();
            return data;
        }

        //个人分类在办事项数
        public int GetWorkItemingCount(string userid, string applicationname)
        {
            var data = CurService.GetData().Where(m => m.SOURCE == userid && (m.URL.ToLower().Contains("admin/") || m.URL.ToLower().Contains("mobile/")) && m.APPLICATION_NAME == applicationname);
            return data.Count();
        }
        #endregion


    }
}
