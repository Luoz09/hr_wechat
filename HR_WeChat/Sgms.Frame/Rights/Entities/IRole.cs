using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 角色接口
    /// </summary>
    public interface IRole : IEntity
    {
        /// <summary>
        /// 角色名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }
    }
}
