using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;

namespace HR.BLL
{
    public class ROLESManager : OperationManager<ROLES>
    {
        private ROLESService _TmpCurService;
        private ROLESService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as ROLESService;
                }
                return _TmpCurService;
            }
        }

        public IQueryable<ROLES> GetCLASSIFYData() { return CurService.GetData().Where(m=>m.CLASSIFY=="n"); }


        public IQueryable<ROLES> GetRolesData() { return CurService.GetData();  }

        public IQueryable<ROLES> GetDataByRoleID(string roleID) { return CurService.GetData().Where(m => m.ID == roleID); }


    }
}
