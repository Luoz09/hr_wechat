using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class EXPRESSIONSManager : OperationManager<EXPRESSIONS>
    {
        private EXPRESSIONSService _TmpCurService;
        private EXPRESSIONSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as EXPRESSIONSService;
                }
                return _TmpCurService;
            }
        }


        public IQueryable<EXPRESSIONS> GetDataByName(string name) { return CurService.GetData(m => m.NAME == name); }

        public IQueryable<EXPRESSIONS> GetExpressData() { return CurService.GetData(); }
          

    }
}
