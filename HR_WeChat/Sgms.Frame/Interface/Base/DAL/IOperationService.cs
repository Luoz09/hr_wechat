using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Interface.DAL
{
    /// <summary>
    /// 带数据操作的数据访问层类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOperationService<T> : IService<T>, IDataOperation<T> where T : class,IEntity
    {
    }
}
