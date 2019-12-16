using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class ROLE_TO_FUNCTIONSManager : OperationManager<ROLE_TO_FUNCTIONS>
    {
        private ROLE_TO_FUNCTIONSService _TmpCurService;
        private ROLE_TO_FUNCTIONSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as ROLE_TO_FUNCTIONSService;
                }
                return _TmpCurService;
            }
        }

        public IQueryable<ROLE_TO_FUNCTIONS> GetRoleFunDataByRoleID(string roleID) { return CurService.GetData().Where(m => m.ROLE_ID == roleID); }
         
    }
}
