using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class APPLICATIONSManager : OperationManager<APPLICATIONS>
    {
        private APPLICATIONSService _TmpCurService;
        private APPLICATIONSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as APPLICATIONSService;
                }
                return _TmpCurService;
            }
        }


        public IQueryable<APPLICATIONS> GetApplicationByCodeName(string codeName)
        {
            return CurService.GetData(m => m.CODE_NAME== codeName);
        }



    }
}
