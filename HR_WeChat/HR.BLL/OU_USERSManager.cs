using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;
using Sgms.Frame.Utils;

namespace HR.BLL
{
    public class OU_USERSManager : OperationManager<OU_USERS>
    {
        private OU_USERSService _TmpCurService;
        private OU_USERSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as OU_USERSService;
                }
                return _TmpCurService;
            }
        }

      
        public IQueryable<OU_USERS> GetOUUser() { return CurService.GetData(); }

        public OU_USERS GetDataByUserID(string userID) { return CurService.GetData().Where(m => m.USER_GUID == userID && m.SIDELINE == 0).FirstOrDefault(); }

        public IQueryable<OU_USERS> GetUsersListByDepID(string parentID)
        {
            return CurService.GetData().Where(m => m.PARENT_GUID == parentID);
        }
          

        public IQueryable GetUsersBySearch(string str)
        {
            return  CurService.GetData().Where(m => m.DISPLAY_NAME.Contains(str)).OrderBy(m=>m.GLOBAL_SORT);
        }


    }
}
