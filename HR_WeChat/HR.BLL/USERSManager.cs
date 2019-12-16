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
    public class USERSManager : OperationManager<USERS>
    {
        private USERSService _TmpCurService;
        private USERSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as USERSService;
                }
                return _TmpCurService;
            }
        }


        public USERS GetUserByNamePwd(string userName, string passWord)
        {
            var password = EncryptUtil.PwdCalculate("",passWord);
            var userinfo = CurService.GetData().Where(m => m.LOGON_NAME == userName && m.USER_PWD == password).FirstOrDefault();
            return userinfo;
        }


        public IQueryable<USERS> GetUsersData() { return CurService.GetData(); }



    }
}
