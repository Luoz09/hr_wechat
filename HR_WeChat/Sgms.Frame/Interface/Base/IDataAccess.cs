using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sgms.Frame.Interface
{
    public interface IDataAccess<T>
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        IEnumerable GetData();

        /// <summary>
        /// 根据ID获取单个实体
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        T GetEntity(string id);
    }
}
