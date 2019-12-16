using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.BLL;
using Sgms.Frame.Sys;
using System.Web.Mvc;
using Sgms.Frame.Interface.BLL;

namespace Sgms.Frame.Page.MVC
{
    /// <summary>
    /// 树形数据访问页面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeDataAccessPage<T> : DataAccessPage<T> where T : class,ITree
    {
        private ITreeManager<T> _TmpCurManager;
        /// <summary>
        /// Manager
        /// </summary>
        private ITreeManager<T> CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as ITreeManager<T>;
                }
                return _TmpCurManager;
            }
        }

        /// <summary>
        /// 获取树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTree()
        {
            return DisplayJson(CurManager.GetTree());
        }

        /// <summary>
        /// 获取包含顶级的树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTreeWithTop()
        {
            string topName = SysLang.GetWords(SysLang.TEXT_TOP);
            if (!String.IsNullOrWhiteSpace(Request.QueryString["topName"]))
            {
                topName = Request.QueryString["topName"];
            }
            return DisplayJson(CurManager.GetTreeWithTop(topName, Request.QueryString["filterID"]));
        }
    }
}