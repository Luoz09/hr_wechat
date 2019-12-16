using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class GROUPSManager : OperationManager<GROUPS>
    {
        private GROUPSService _TmpCurService;
        private GROUPSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as GROUPSService;
                }
                return _TmpCurService;
            }
        }

        public IQueryable<GROUPS> GetGroups()
        {
            return CurService.GetData();
        }

    }

    
}
