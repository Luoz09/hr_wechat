using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Web;
using System.Web.SessionState;
using Sgms.Frame.Interface.BLL;
using Sgms.Frame.Interface.DAL;

namespace Sgms.Frame.BLL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeManager<T> : Manager<T>, ITreeManager<T> where T : class, ITree
    {
        #region 数据访问层实例相关

        private ITreeService<T> _TmpCurService;
        /// <summary>
        /// 数据访问层的实例
        /// </summary>
        private ITreeService<T> CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as ITreeService<T>;
                }
                return _TmpCurService;
            }
        }

        #endregion 数据访问层实例相关

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
        /// <returns></returns>
        public IEnumerable<T> GetTreeWithTop(string topName, string filterID = null)
        {
            return CurService.GetTreeWithTop(topName, filterID);
        }

        #endregion 树操作
    }
}