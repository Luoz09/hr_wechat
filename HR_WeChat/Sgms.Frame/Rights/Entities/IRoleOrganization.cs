using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 角色部门关系接口
    /// </summary>
    public interface IRoleOrganization : IEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        string RoleID { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        string OrganizationID { get; set; }
    }
}
