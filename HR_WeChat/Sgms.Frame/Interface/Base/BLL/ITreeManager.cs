using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Interface.BLL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeManager<T> : IManager<T> where T : class ,IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetTree();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetTreeWithTop(string topName, string filterID = null);
    }
}