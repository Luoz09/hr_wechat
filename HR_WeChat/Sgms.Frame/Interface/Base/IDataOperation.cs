using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using System.Collections;

namespace Sgms.Frame.Interface
{
    /// <summary>
    /// 数据操作
    /// </summary>
    public  interface IDataOperation<T> where T : class,IEntity
    {
        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Insert(T entity);

        /// <summary>
        /// 插入一组实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Insert(IEnumerable entities);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        bool Update(IEnumerable entities);

        /// <summary>
        /// 根据ID删除一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(string id);

        /// <summary>
        /// 根据一组ID删除包含这些ID的记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(IEnumerable<string> ids);

        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete(T entity);
    }
}
