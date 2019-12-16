using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class HR_CheckManager : OperationManager<HR_Check>
    {
        private HR_CheckService _TmpCurService;
        private HR_CheckService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as HR_CheckService;
                }
                return _TmpCurService;
            }
        }
         
      
    }
}
