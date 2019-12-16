using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Reflection;
using Sgms.Frame.DAL;
using Sgms.Frame.Interface.BLL;
using Sgms.Frame.Interface.DAL;

namespace Sgms.Frame.BLL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeOperationManager<T> : OperationManager<T>, ITreeOperationManager<T> where T : class,ITree
    {
        private ITreeOperationService<T> _TmpCurService;
        /// <summary>
        /// 数据访问层的实例
        /// </summary>
        private ITreeOperationService<T> CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as ITreeOperationService<T>;
                }
                return _TmpCurService;
            }
        }

        #region 树操作

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetTree()
        {
            return CurService.GetTree();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topName"></param>
        /// <param name="filterID"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTreeWithTop(string topName, string filterID = null)
        {
            return CurService.GetTreeWithTop(topName, filterID);
        }

        #endregion 树操作
    }
}
