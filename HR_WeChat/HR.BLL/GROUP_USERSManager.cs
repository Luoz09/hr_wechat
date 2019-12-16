using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class GROUP_USERSManager : OperationManager<GROUP_USERS>
    {
        private GROUP_USERSService _TmpCurService;
        private GROUP_USERSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as GROUP_USERSService;
                }
                return _TmpCurService;
            }
        }

        public IQueryable<GROUP_USERS> GetGroupUsers()
        {
            return CurService.GetData();
        }
    }
}
