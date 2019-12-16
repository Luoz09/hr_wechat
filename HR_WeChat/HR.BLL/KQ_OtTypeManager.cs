using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class KQ_OtTypeManager : OperationManager<KQ_OtType>
    {
        private KQ_OtTypeService _TmpCurService;
        private KQ_OtTypeService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as KQ_OtTypeService;
                }
                return _TmpCurService;
            }
        }


        public IQueryable<KQ_OtType> GetOtTypeData(string TypeNo) { return CurService.GetData().Where(m=>m.OtTypeNo.Contains(TypeNo)).OrderBy(m => m.OtTypeNo); }


    }
}
