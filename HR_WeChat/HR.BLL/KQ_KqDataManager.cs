using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;
using Sgms.Frame.Sys;
using System.Data;

namespace HR.BLL
{
    public class KQ_KqDataManager : OperationManager<KQ_KqData>
    {
        private KQ_KqDataService _TmpCurService;
        private KQ_KqDataService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as KQ_KqDataService;
                }
                return _TmpCurService;
            }
        }


        //获取当前登录人员的考勤数据
        public IQueryable<KQ_KqData> GetKQDataBySysAdminID() { return CurService.GetData().Where(m => m.EmpSysID == SysPara.CurAdmin.ID);   }


        //获取考勤数据
        public IQueryable<KQ_KqData> GetKQData() { return CurService.GetData(); }


        public DataTable GetKQDataCount(string checkName, string minDate, string maxDate)
        {

            string sql = "select empsysid  , LOGON_NAME , " +
                         "count (*) as checkCount ,"+
                         "count(case  IsKouKuan  when 'N' then '' end) as NormalCheckCount," +
                         "count (case  IsKouKuan  when 'Y' then '' end ) as DangerCheckCount " +
                         "from dbo.KQ_KqData a left join USERS b on a.empsysid= b.GUID  where 1=1";

            if(!string.IsNullOrEmpty(checkName))
                sql += " and LOGON_NAME like '%" + checkName + "%'"; 
            if (!string.IsNullOrEmpty(minDate))
                sql += " and KqDate >= '" + minDate + "'";
            if (!string.IsNullOrEmpty(maxDate))
                sql += " and KqDate < '" + Convert.ToDateTime(maxDate).AddDays(1) + "'";

            sql += " group by empsysid , LOGON_NAME ";
            DataTable dt = SqlHelp.LoadDataTable(sql); 
            return dt;
        }

    }
}
