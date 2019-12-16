using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.BLL;
using HR.Models;
using HR.DAL;  

namespace HR.BLL
{
    public class SYS_DICTIONARYManager : OperationManager<SYS_DICTIONARY>
    {
        private SYS_DICTIONARYService _TmpCurService;
        private SYS_DICTIONARYService CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as SYS_DICTIONARYService;
                }
                return _TmpCurService;
            }
        }


        /// <summary>
        /// 获取字典
        /// </summary>
        /// <param name="itemTypeID"></param>
        /// <returns></returns>
        public IQueryable<SYS_DICTIONARY> GetDictionarysByItemTypeID(String itemTypeID)
        {
            return CurService.GetData().Where(m => m.ItemTypeID == itemTypeID && String.IsNullOrEmpty(m.ParentID) && m.State == 0).OrderBy(m => m.SortID);
        }
          
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <param name="itemTypeID"></param>
        /// <returns></returns>
        public IQueryable<SYS_DICTIONARY> GetDictionarysByParentID(String parentID)
        {
            return CurService.GetData().Where(m => m.ParentID == parentID && m.State == 0).OrderBy(m => m.SortID);
        }


        public IQueryable<SYS_DICTIONARY> GetDictionarysByParentIDAndItemTypeID(String parentID, string itemTypeID)
        {
            return CurService.GetData().Where(m => m.ParentID == parentID && m.ItemTypeID == itemTypeID && m.State == 0).OrderBy(m => m.SortID);
        }

        public SYS_DICTIONARY GetDictionaryByItemTypeAndValue(String itemTypeID, String itemValue)
        {
            return CurService.GetFirstOrDefault(m => m.ItemTypeID == itemTypeID && m.ItemValue == itemValue && m.State == 0);

        }
         
        public IQueryable<SYS_DICTIONARY> GetDictionaryData()
        {
            return CurService.GetData();
        }



    }
}
