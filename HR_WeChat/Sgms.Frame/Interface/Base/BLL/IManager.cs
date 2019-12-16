using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Collections;

namespace Sgms.Frame.Interface.BLL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IManager<T> : IDisposable, IDataAccess<T>, IEmptyManager
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
        /// 过滤数据
        /// </summary>
        /// <returns></returns>
        IEnumerable GetFilteredData();

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        IEnumerable GetListData();
    }
}
