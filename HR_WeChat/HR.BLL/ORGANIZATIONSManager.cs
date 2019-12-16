using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class ORGANIZATIONSManager : OperationManager<ORGANIZATIONS>
    {
        private ORGANIZATIONSService _TmpCurService;
        private ORGANIZATIONSService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as ORGANIZATIONSService;
                }
                return _TmpCurService;
            }
        }


        /// <summary>
        /// 获取所有排序部门
        /// </summary>
        public IQueryable<ORGANIZATIONS> GetDepInfo()
        {
            return CurService.GetData().OrderBy(m=>m.GLOBAL_SORT);
        }


        public IQueryable<ORGANIZATIONS> GetDepsBySearch(string str)
        {
            return CurService.GetData().Where(m => m.DISPLAY_NAME.Contains(str)).OrderBy(m => m.GLOBAL_SORT);
        }


        /// <summary>
        /// 通过部门ID获取该部门信息
        /// </summary>
        public IQueryable<ORGANIZATIONS> GetDepByDepID(string depID)
        {
            return CurService.GetData().Where(m=>m.GUID==depID);
        }


        /// <summary>
        /// 获取根部门
        /// </summary>
        public ORGANIZATIONS GetTopDep()
        {
            return CurService.GetData().Where(m=> string.IsNullOrEmpty(m.PARENT_GUID)).FirstOrDefault();
        }


        /// <summary>
        ///获取该部门下的所有子部门
        /// </summary>
        public IQueryable<ORGANIZATIONS> GetDepsByParentID(string guid)
        {
            return CurService.GetData().Where(m => m.PARENT_GUID == guid); ;
        }


        /// <summary>
        /// 直接获取二级部门
        /// </summary>
        public IQueryable<ORGANIZATIONS> GetSecDepsByTopDep()
        {
            var topDep = CurService.GetData().Where(m => string.IsNullOrEmpty(m.PARENT_GUID)).FirstOrDefault();
            if (topDep != null)
            {
                return CurService.GetData().Where(m => m.PARENT_GUID == topDep.GUID);
            }
            else {
                return CurService.GetData();
            }
        }


    }
}
