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
    public interface IChildTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="childTableJson"></param>
        /// <returns></returns>
        bool Update(object entity, params string[] childTableJson);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="childTableJson"></param>
        /// <returns></returns>
        bool Insert(object entity, params string[] childTableJson);
    }
}