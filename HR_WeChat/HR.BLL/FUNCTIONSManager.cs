using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class FUNCTIONSManager : OperationManager<FUNCTIONS>
    {
        private FUNCTIONSService _TmpCurService;
        private FUNCTIONSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as FUNCTIONSService;
                }
                return _TmpCurService;
            }
        }

        public IQueryable<FUNCTIONS> GetFunctions()
        {
            return CurService.GetData();
        }

    }
}
