using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using System.Web.Mvc;
using Sgms.Frame.BLL;
using Sgms.Frame.Sys;
using Sgms.Frame.Interface.BLL;

namespace Sgms.Frame.Page.MVC
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeDataOperationPage<T> : DataOperationPage<T> where T : class ,ITree
    {
        private ITreeOperationManager<T> _TmpCurManager;
        /// <summary>
        /// Manager
        /// </summary>
        private ITreeOperationManager<T> CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as ITreeOperationManager<T>;
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
