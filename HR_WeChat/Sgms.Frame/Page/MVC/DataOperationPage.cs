using System;
using System.Collections.Generic;
using System.Linq;
using Sgms.Frame.Entities;
using System.Web.Mvc;
using Sgms.Frame.Sys;
using Sgms.Frame.Interface.BLL;
using System.Reflection; 
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sgms.Frame.Page.MVC
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataOperationPage<T> : DataAccessPage<T> where T : class, IEntity
    {
        private IOperationManager<T> _TmpCurManager;
        /// <summary>
        /// Manager
        /// </summary>
        private IOperationManager<T> CurManager
        {
            get
            {
                if (_TmpCurManager == null)
                {
                    _TmpCurManager = Manager as IOperationManager<T>;
                }
                return _TmpCurManager;
            }
        }



        /// <summary>
        /// GET: /WebApi/Default1/Create
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Create()
        {
            Nav = GetAddNav();
            ViewBag.Nav = Nav;
            return View("Edit", CurEntity);
        }

        /// <summary>
        /// POST: /WebApi/Default1/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public virtual ContentResult Create(FormCollection collection)
        {
            FillEntity(CurEntity);
            //UpdateModel(CurEntity);
            AfterUpdateModel(true);
            return DisplayJson(CurManager.Insert(CurEntity), CurManager.RunMessage.ToOnelineString());
        }

        /// <summary>
        /// GET: /WebApi/Default1/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ActionResult Edit(string id)
        {
            Nav = GetEditNav();
            ViewBag.Nav = Nav;
            return View(CurManager.GetEntity(id));
        }

        /// <summary>
        /// POST: /WebApi/Default1/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, FormCollection collection)
        {
            if (id == null)
            {
                return DisplayJson(false, SysLang.GetWords(SysLang.COULD_NOT_GET_ID));
            }
            if (CurEntity == null)
            {
                return DisplayJson(false, SysLang.GetWords(SysLang.ENTITY_DELETED));
            }
            //UpdateModel(CurEntity);
            FillEntity(CurEntity);
            AfterUpdateModel(false);
            return DisplayJson(CurManager.Update(CurEntity), CurManager.RunMessage.ToOnelineString());
        }

        protected virtual void AfterUpdateModel(bool isCreate)
        {

        }


        /// <summary>
        /// POST: /WebApi/Default1/Delete/5
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult Delete(string ids, FormCollection collection)
        {
            return DisplayJson(CurManager.Delete(ids.Split(',')), CurManager.RunMessage.ToOnelineString());
        }

        /// <summary>
        /// 添加页面导航
        /// </summary>
        /// <returns></returns>
        protected List<LinkInfo> GetAddNav()
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
                Text = PageCName + "添加"
            });
            return result;
        }

        /// <summary>
        /// 编辑页面导航
        /// </summary>
        /// <returns></returns>
        protected List<LinkInfo> GetEditNav()
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
                Text = PageCName + "编辑"
            });
            return result;
        }


        /// <summary>
        /// 详情页面导航
        /// </summary>
        /// <returns></returns>
        protected List<LinkInfo> GetDetailsNav()
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
        /// 不要自动填充的字段
        /// </summary>
        protected virtual string[] NonAutoPopField { get { return new String[0]; } }

        #region DataOperation

        /// <summary>
        /// 填充实体
        /// </summary>
        /// <param name="entity"></param>
        protected void FillEntity(T entity)
        {
            IEnumerable<PropertyInfo> properInfos = entity.GetType().GetProperties().Where(m => !NonAutoPopField.Contains(m.Name) && m.Name != SysKeys.KEY_ID);

            foreach (var elem in properInfos)
            {
                string value = Request[elem.Name];
                if (value != null)
                {
                    try
                    {
                        elem.SetValue(entity, Convert.ChangeType(value, Nullable.GetUnderlyingType(elem.PropertyType) ?? elem.PropertyType), null);
                    }
                    catch
                    {
                        try
                        {
                            elem.SetValue(entity, elem.PropertyType.IsValueType ? Activator.CreateInstance(elem.PropertyType) : null, null);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(ID))
            {
                entity.ID = ID;
            }
        }

        

        #endregion DataOperation
    }
}
