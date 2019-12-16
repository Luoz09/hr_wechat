using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 角色用户接口
    /// </summary>
    public interface IRoleUser : IEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        string RoleID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        string UserID { get; set; }
    }
}
