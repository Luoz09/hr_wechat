using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Interface.DAL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChildTableProcess<T> where T : class,IEntity
    {
        /// <summary>
        /// 0 新增 1 编辑 2 删除
        /// </summary>
        /// <returns></returns>
        List<IEnumerable<T>> GetChanges(string json, string foreignKeyField, string foreignKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="json"></param>
        /// <param name="foreignKeyField"></param>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        bool Save(IOperationService<T> service, string json, string foreignKeyField, string foreignKey);
    }
}
