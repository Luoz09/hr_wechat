using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.DAL;
using Sgms.Frame.Sys;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;
using System.Data.Linq.Mapping;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;
using Sgms.Frame.Interface.DAL;
using Sgms.Frame.Interface.BLL;
using System.Collections;

namespace Sgms.Frame.BLL
{
    /// <summary>
    /// 仅包含查询操作的数据访问层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Manager<T> : IManager<T>, IDisposable where T : class, IEntity
    {
        #region 成员相关

        #region 消息回传

        /// <summary>
        /// 最后一次查询的长度
        /// </summary>
        public int LastCount
        {
            get { return Service.LastCount; }
        }

        /// <summary>
        /// 执行中的系统消息
        /// </summary>
        public SysMessage RunMessage
        {
            get { return Service.RunMessage; }
        }

        #endregion 消息回传

        #region 数据访问层实例相关

        private IService<T> _TmpService;
        /// <summary>
        /// 数据访问层的实例
        /// </summary>
        protected virtual IService<T> Service
        {
            get
            {
                if (_TmpService == null)
                {
                    _TmpService = BLLHelper.GetService<T>();
                    _TmpService.FilterInfo = FilterInfo;
                }
                return _TmpService;
            }
        }

        public DataFilterInfo FilterInfo
        {
            get { return Service.FilterInfo; }
            set { Service.FilterInfo = value; }
        }

        #endregion 数据访问层实例相关

        #endregion 成员相关

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(string id)
        {
            return Service.GetEntity(id);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable GetData()
        {
            return Service.GetData();
        }

        /// <summary>
        /// 过滤数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable GetFilteredData()
        {
            return Service.GetFilteredData();
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable GetListData()
        {
            return Service.GetFilteredData();
        }

        public void Dispose()
        {
            Service.Dispose();
        }
    }
}