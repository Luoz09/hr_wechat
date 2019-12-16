using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class KQ_DayoffTypeManager : OperationManager<KQ_DayoffType>
    {
        private KQ_DayoffTypeService _TmpCurService;
        private KQ_DayoffTypeService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as KQ_DayoffTypeService;
                }
                return _TmpCurService;
            }
        }

        public IQueryable<KQ_DayoffType> GetDayoffTypeData(string TypeNo) { return CurService.GetData().Where(m=>m.DayoffTypeNo.Contains(TypeNo)).OrderBy(m => m.DayoffTypeNo); }
         

    }
}
