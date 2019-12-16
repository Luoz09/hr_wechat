using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 角色对应应用/操作接口
    /// </summary>
    public interface IRoleAction : IEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        string RoleID { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        string ActionID { get; set; }

        /// <summary>
        /// 功能Key
        /// </summary>
        string ActionName { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        string AppID { get; set; }

        /// <summary>
        /// 应用KEY
        /// </summary>
        string AppName { get; set; }
    }
}
