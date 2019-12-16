using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 包含FULLPATH的树
    /// </summary>
    public interface IFullPathTree : ITree
    {
        /// <summary>
        /// 用 / 隔开
        /// </summary>
        string FullPath { get; set; }

        /// <summary>
        /// 全局排序
        /// </summary>
        string GlobalSort { get; set; }
    }

    /// <summary>
    /// 包含FULLPATH的树的树形实体接口实现
    /// </summary>
    public class FullPathTree : Tree, IFullPathTree
    {
        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get;
            set;
        }

        /// <summary>
        /// 全局排序
        /// </summary>
        public string GlobalSort
        {
            get;
            set;
        }
    }
}
