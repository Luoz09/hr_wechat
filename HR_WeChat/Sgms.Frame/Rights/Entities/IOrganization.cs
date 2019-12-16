using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 部门接口
    /// </summary>
    public interface IOrganization : IEntity
    {
        /// <summary>
        /// 部门名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        string Sort { get; set; }

        /// <summary>
        /// 父部门
        /// </summary>
        string ParentID { get; set; }
    }
}
