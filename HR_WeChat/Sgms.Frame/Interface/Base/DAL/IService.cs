using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Collections;

namespace Sgms.Frame.Interface.DAL
{
    /// <summary>
    /// 数据访问层类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IService<T> : IDisposable, IDataAccess<T>, IEmptyService
    {
        /// <summary>
        /// 过滤信息
        /// </summary>
        DataFilterInfo FilterInfo { get; set; }

        /// <summary>
        /// 最后一条记录长度
        /// </summary>
        int LastCount { get; }

        /// <summary>
        /// 获取过滤好的数据
        /// </summary>
        /// <returns></returns>
        IEnumerable GetFilteredData();
    }
}