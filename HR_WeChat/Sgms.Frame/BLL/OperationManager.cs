using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.DAL;
using Sgms.Frame.Sys;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Web;
using System.Web.SessionState;
using Sgms.Frame.Interface.DAL;
using Sgms.Frame.Interface.BLL;
using System.Collections;
using System.Data;

namespace Sgms.Frame.BLL
{
    /// <summary>
    /// 包含增删改查操作的业务逻辑层基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OperationManager<T> : Manager<T>, IOperationManager<T> where T : class, IEntity
    {
        #region 数据访问层实例相关

        private IOperationService<T> _TmpCurService;
        /// <summary>
        /// 数据访问层的实例
        /// </summary>
        private IOperationService<T> CurService
        {
            get
            {
                if (_TmpCurService == null)
                {
                    _TmpCurService = Service as IOperationService<T>;
                }
                return _TmpCurService;
            }
        }

        #endregion 数据访问层实例相关
        public virtual bool Insert(T entity)
        {
            var tEntity = (T)entity;
            if (!VerificationEntity(tEntity, true)) return false;
            SetDefaultData(tEntity, true);
            return CurService.Insert(entity);
        }

        public virtual bool Update(T entity)
        {
            var tEntity = (T)entity;
            if (!VerificationEntity(tEntity, false)) return false;
            SetDefaultData(tEntity, false);
            return CurService.Update(entity);
        }
   

        public virtual bool Insert(IEnumerable entities)
        {
            if (!VerificationEntity(entities, true)) return false;
            SetDefaultData(entities, true);
            return CurService.Insert(entities);
        }

        public virtual bool Update(IEnumerable entities)
        {
            if (!VerificationEntity(entities, false)) return false;
            SetDefaultData(entities, false);
            return CurService.Update(entities);
        }

        public virtual bool Delete(string id)
        {
            return CurService.Delete(id);
        }

        public virtual bool Delete(IEnumerable<string> ids)
        {
            return CurService.Delete(ids);
        }

        public virtual bool VerificationEntity(T entity, bool isCreate)
        {
            return true;
        }

        public virtual bool VerificationEntity(IEnumerable entities, bool isCreate)
        {
            return true;
        }

        public virtual void SetDefaultData(T entity, bool isCreate)
        {
        }

        public virtual void SetDefaultData(IEnumerable entities, bool isCreate)
        {
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="dt"></param>
        public virtual void ImportExcel(DataTable dt)
        { }

          
        public bool Delete(T entity)
        {
            return CurService.Delete(entity);
        }
    }
}