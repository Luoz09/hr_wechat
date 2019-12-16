using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using System.Web.Mvc;
using Sgms.Frame.BLL;
using Sgms.Frame.Sys;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using Sgms.Frame.Interface.BLL;

namespace Sgms.Frame.Page.MVC
{
    /// <summary>
    /// MVC 数据访问类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataAccessPage<T> : NormalPage
        where T : class, IEntity
    {
        private IManager<T> _TmpManager;
        /// <summary>
        /// Manager
        /// </summary>
        protected virtual IManager<T> Manager
        {
            get
            {
                if (_TmpManager == null)
                {
                    _TmpManager = PageHelper.GetManager<T>();
                    _TmpManager.FilterInfo = FilterInfo;
                }
                return _TmpManager;
            }
        }

        #region 实体获取及ID相关

        private string _TmpID = String.Empty;
        /// <summary>
        /// ID
        /// </summary>
        protected string ID
        {
            get
            {
                if (_TmpID == String.Empty)
                {
                    if (!String.IsNullOrEmpty(Request[SysKeys.KEY_ID]))
                    {
                        _TmpID = Request[SysKeys.KEY_ID];
                    }
                    else
                    {
                        _TmpID = null;
                    }
                }
                return _TmpID;
            }
        }

        /// <summary>
        /// 当前实体是否赋值过
        /// </summary>
        private bool _TmpCurEntityAssignment = false;
        private T _TmpCurEntity;
        /// <summary>
        /// 当前实体
        /// </summary>
        protected virtual T CurEntity
        {
            get
            {
                if (!_TmpCurEntityAssignment)
                {
                    _TmpCurEntityAssignment = true;
                    if (ID != null)
                    {
                        _TmpCurEntity = Manager.GetEntity(ID) ?? System.Activator.CreateInstance<T>();
                    }
                    else
                    {
                        _TmpCurEntity = System.Activator.CreateInstance<T>();
                    }
                }
                return _TmpCurEntity;
            }
        }

        #endregion 实体获取及ID相关

        #region 查询过滤相关

        /// <summary>
        /// 是否为逆序
        /// </summary>
        private bool IsDesc
        {
            get { return Request.Form["order"] == "desc"; }
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        private string SortField
        {
            get { return Request.Form["sort"]; }
        }

        /// <summary>
        /// 查询的字段
        /// </summary>
        private string SearchStr
        {
            get { return Request["searchstr"]; }
        }

        /// <summary>
        /// 高级查询用的JSON
        /// </summary>
        private string AdvancedSearchJson
        {
            get { return Request["advanced"]; }
        }

        /// <summary>
        /// 每页多少条
        /// </summary>
        private int PageSize
        {
            get
            {
                int result = 15;
                if (!String.IsNullOrWhiteSpace(Request["rows"]))
                {
                    try
                    {
                        result = Convert.ToInt32(Request["rows"]);
                    }
                    catch { }
                }
                if (result < 1) result = 1;
                return result;
            }
        }

        /// <summary>
        /// 当前页
        /// </summary>
        private int CurPage
        {
            get
            {
                int result = 1;
                if (!String.IsNullOrWhiteSpace(Request["rows"]))
                {
                    try
                    {
                        result = Convert.ToInt32(Request["page"]);
                    }
                    catch { }
                }
                if (result < 1) result = 1;
                return result;
            }
        }

        private DataFilterInfo _TmpFilterInfo;
        /// <summary>
        /// 过滤信息
        /// </summary>
        protected virtual DataFilterInfo FilterInfo
        {
            get
            {
                if (_TmpFilterInfo == null)
                {
                    _TmpFilterInfo = new DataFilterInfo()
                    {
                        AdvancedSearchJson = AdvancedSearchJson,
                        CurPage = CurPage,
                        IsDesc = IsDesc,
                        PageSize = PageSize,
                        SearchStr = SearchStr,
                        SortField = SortField
                    };
                }
                return _TmpFilterInfo;
            }
        }

        #endregion 查询过滤相关

        /// <summary>
        /// 详情页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ActionResult Details(string id = null)
        {
            Nav = GetViewNav();
            ViewBag.Nav = Nav;
            T entity = Manager.GetEntity(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public virtual ContentResult GetData()
        {
            return DisplayDataOfTotalWithLastCount(Manager.GetListData());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ContentResult GetEntity(string id = null)
        {
            return DisplayJson(Manager.GetEntity(id));
        }

        /// <summary>
        /// 视图页面导航
        /// </summary>
        /// <returns></returns>
        protected List<LinkInfo> GetViewNav()
        {
            var result = new List<LinkInfo>();
            result.Add(new LinkInfo()
            {
                Href = Url.Action("Desktop", "Home"),
                Text = "桌面"
            });
            result.Add(new LinkInfo()
            {
                Href = Url.Action("Index"),
                Text = PageCName + "列表"
            });
            result.Add(new LinkInfo()
            {
                Text = PageCName + "详情"
            });
            return result;
        }

        /// <summary>
        /// 显示EasyUI Datagrid所用的Json格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ContentResult DisplayDataOfTotal(IEnumerable<object> data)
        {
            return DataDisplay.DisplayDataOfTotal(data);
        }

        /// <summary>
        /// 显示EasyUI Datagrid所用的Json格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ContentResult DisplayDataOfTotalWithLastCount(IEnumerable data)
        {
            return DataDisplay.DisplayDataOfTotal(data, Manager.LastCount);
        }

        /// <summary>
        /// 显示EasyUI Datagrid所用的Json格式
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ContentResult DisplayDataOfTotalWithDataCount(IEnumerable data,int count)
        {
            return DataDisplay.DisplayDataOfTotal(data,count);
        }

        protected override void Dispose(bool disposing)
        {
            Manager.Dispose();
            base.Dispose(disposing);
        }
    }
}
