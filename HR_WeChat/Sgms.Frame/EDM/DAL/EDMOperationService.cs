using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Data.Objects.DataClasses;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using Sgms.Frame.Exs;
using Sgms.Frame.DAL;
using System.Linq.Expressions;
using System.Data.Objects;
using Sgms.Frame.Interface.DAL;

namespace Sgms.Frame.EDM.DAL
{
    /// <summary>
    /// 实体数据模型 数据操作类
    /// </summary>
    public abstract class EDMOperationService<T> : EDMService<T>, IOperationService<T> where T : class, IEntity
    {
        #region 数据操作相关

        #region 插入相关

        /// <summary>
        /// 将一个实体插入到缓存中(在调用SubmitToDb()前不提交给数据库)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual void InsertToCache(T entity)
        {
            if (String.IsNullOrEmpty(entity.ID))
            {
                entity.ID = Guid.NewGuid().ToString();
            }
            DbSet.Add(entity);
        }

        /// <summary>
        /// 将一组实体集合插入到缓存中(在调用SubmitToDb()前不提交给数据库)
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public virtual void InsertToCache(IEnumerable<T> entities)
        {
            foreach (var elem in entities)
            {
                InsertToCache(elem);
            }
        }

    
        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Insert(T entity)
        {
            InsertToCache((T)entity);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }
        /// <summary>
        /// 插入一组实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual bool Insert(System.Collections.IEnumerable entities)
        {
            foreach (T elem in entities)
            {
                InsertToCache(elem);
            }
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        #endregion 插入相关

        #region 修改相关

        /// <summary>
        /// 更新实体到缓存 非数据库查询结果
        /// </summary>
        /// <param name="entity"></param>
        public virtual void ModifyToCache(T entity)
        {
            DbSet.Attach(entity);
            DbContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
        }

        /// <summary>
        /// 更新实体到缓存 非数据库查询结果
        /// </summary>
        /// <param name="entities"></param>
        public virtual void ModifyToCache(IEnumerable<T> entities)
        {
            foreach (var elem in entities)
            {
                ModifyToCache(elem);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <returns></returns>
        public virtual bool Modify(T entity)
        {
            ModifyToCache(entity);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        /// <summary>
        /// 更新实体到缓存（在调用SubmitToDb()前不提交给数据库）
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void UpdateToCache(T entity)
        {
            DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;//.ChangeObjectState(entity, System.Data.EntityState.Modified);
        }

        /// <summary>
        /// 批量更新实体到缓存（在调用SubmitToDb()前不提交给数据库）
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void UpdateToCache(IEnumerable<T> entities)
        {
            foreach (var elem in entities)
            {
                UpdateToCache(elem);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual bool Update(T entity)
        {
            UpdateToCache((T)entity);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public virtual bool Update(System.Collections.IEnumerable entities)
        {
            foreach (T elem in entities)
            {
                UpdateToCache(elem);
            }
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        #endregion 修改相关

        #region 删除相关

        /// <summary>
        /// 根据ID将实体在缓存中删除（在调用SubmitToDb()前不提交给数据库）
        /// </summary>
        /// <param name="id"></param>
        public virtual void DeleteToCache(string id)
        {
            T entity = System.Activator.CreateInstance<T>();
            entity.ID = id;
            DeleteToCache(entity);
        }

        /// <summary>
        /// 将实体在缓存中删除（在调用SubmitToDb()前不提交给数据库）
        /// </summary>
        /// <param name="entity"></param>
        public virtual void DeleteToCache(T entity)
        {
            if (entity == null)
            {
                RunMessage.Append(SysLang.GetWords(SysLang.ENTITY_DELETED));
                return;
            }
            DbSet.Attach(entity);
            DbContext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;//.ChangeObjectState(entity, System.Data.EntityState.Deleted);
        }

        /// <summary>
        /// 根据一组ID在缓存中删除包含这些ID的记录（在调用SubmitToDb()前不提交给数据库）
        /// </summary>
        /// <param name="ids"></param>
        public virtual void DeleteToCache(IEnumerable<string> ids)
        {
            foreach (var elem in ids)
            {
                DeleteToCache(elem);
            }
        }

        /// <summary>
        /// 根据一组实体集合在缓存中删除记录（在调用SubmitToDb()前不提交给数据库）
        /// </summary>
        /// <param name="entities"></param>
        public virtual void DeleteToCache(IEnumerable<T> entities)
        {
            foreach (var elem in entities)
            {
                DeleteToCache(elem);
            }
        }

        /// <summary>
        /// 快速删除
        /// </summary>
        /// <param name="ids"></param>
        public virtual bool DeleteFast(IEnumerable<string> ids)
        {
            var tableName = (typeof(T).GetCustomAttributes(false).First(m => m as EdmEntityTypeAttribute != null) as EdmEntityTypeAttribute).Name;
            var primaryKeyName = GetPrimaryKey().Name;

            string inSqlIDs = String.Join(",", ids.Select(m => "'" + m + "'"));
            string query = String.Format("DELETE FROM {0} WHERE {1} in (@ids)", tableName, primaryKeyName);
            DbParameter param = new SqlParameter("@ids", System.Data.SqlDbType.VarChar, 8000)
            {
                Value = inSqlIDs
            };
            try
            {
                DbContext.Database.ExecuteSqlCommand(query, param);
            
                return true;
            }
            catch (Exception e)
            {
                RunMessage.Append(e);
                return false;
            }
        }

        /// <summary>
        /// 快速删除
        /// </summary>
        /// <param name="entities"></param>
        public virtual bool DeleteFast(IEnumerable<T> entities)
        {
            return DeleteFast(entities.Select(m => m.ID));
        }

        /// <summary>
        /// 根据ID删除一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(string id)
        {
            DeleteToCache(id);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        /// <summary>
        /// 根据实体删除一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Delete(T entity)
        {
            DeleteToCache(entity);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        /// <summary>
        /// 根据一组ID删除包含这些ID的记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool Delete(IEnumerable<string> ids)
        {
            DeleteToCache(ids);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        /// <summary>
        /// 根据一组实体集合删除记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual bool Delete(IEnumerable<T> entities)
        {
            DeleteToCache(entities);
            SubmitToDb();
            return !RunMessage.HasMessage;
        }

        #endregion 删除相关

        /// <summary>
        /// 提交至数据库，若出错则获取错误信息
        /// </summary>
        public void SubmitToDb()
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var msg = string.Empty;
                var errors = (from u in ex.EntityValidationErrors select u.ValidationErrors).ToList();
                foreach (var item in errors)
                    msg += item.FirstOrDefault().ErrorMessage;
                SysUtil.WriteLog("SubmitToDb:err " + msg);
                RunMessage.Append("SubmitToDb:err " + msg);
            }
            catch (Exception e)
            {
              
                SysUtil.WriteLog("SubmitToDb:err" + e.Message);
                RunMessage.Append(e);
            }
}

#endregion 数据操作相关

System.Collections.IEnumerable Interface.IDataAccess<T>.GetData()
        {
            return base.GetData();
        }
    }
}