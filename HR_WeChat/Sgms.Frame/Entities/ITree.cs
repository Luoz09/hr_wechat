using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Newtonsoft.Json;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 树形实体接口
    /// </summary>
    public interface ITree : IEntity
    {

        /// <summary>
        /// 节点名称
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        string ParentID { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        string Sort { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        IEnumerable<ITree> Children { get; set; }
    }

    /// <summary>
    /// 树形实体接口实现
    /// </summary>
    public class Tree : ITree
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentID
        {
            get;
            set;
        }

        /// <summary>
        /// 排序号
        /// </summary>
        public string Sort
        {
            get;
            set;
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public IEnumerable<ITree> Children
        {
            get;
            set;
        }
    }
}