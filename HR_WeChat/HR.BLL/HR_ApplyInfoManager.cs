using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;
using Newtonsoft.Json.Linq;
using System.Data;
using Sgms.Frame.Sys;

namespace HR.BLL
{
    public class HR_ApplyInfoManager : OperationManager<HR_ApplyInfo>
    {
        private HR_ApplyInfoService _TmpCurService;
        private HR_ApplyInfoService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as HR_ApplyInfoService;
                }
                return _TmpCurService;
            }
        }


        public int GetApplyCountByApplyType(int ApplyType) {
            return CurService.GetData().Where(m => m.ApplyType == ApplyType && ( m.ApplyUserID.Contains(SysPara.CurAdmin.ID) || m.InputUserID.Contains(SysPara.CurAdmin.ID) ) ).Count();
        }

        public IQueryable<HR_ApplyInfo> GetMobileApplyData(int ApplyType)
        { 
            var data = CurService.GetData().Where(m => m.ApplyType == ApplyType) ;
            return data; 
        }


        public DataTable GetApplyTypeCount(string applyName,string minDate,string maxDate)
        {
            
            var sql  = "select ApplyUserName," + 
                      "count(case ApplyType when 1 then '' end) as 'LeaveCount'," +
                      "count(case ApplyType when 2 then '' end) as 'OutCount'," +
                      "count(case ApplyType when 3 then '' end) as 'BusinessCount'," +
                      "count(case ApplyType when 4 then '' end) as 'ExtWorkCount'," +
                      "count(case ApplyType when 5 then '' end) as 'SupplementCount' " +
                      "FROM [HR_ApplyInfo] where 1=1 ";

            if (!string.IsNullOrEmpty(applyName))
                sql += " and ApplyUserName like '%" + applyName + "%'";
            if (!string.IsNullOrEmpty(minDate))
                sql += " and InputDate >= '"+minDate+"'";
            if (!string.IsNullOrEmpty(maxDate))
                sql += " and InputDate < '"+Convert.ToDateTime(maxDate).AddDays(1)+"'";

            sql += " group by ApplyUserName";


            DataTable dt = SqlHelp.LoadDataTable(sql);  
             
            return dt;
        }


        public IQueryable<HR_ApplyInfo> GetApplyData() { return CurService.GetData(); }


    }
}
