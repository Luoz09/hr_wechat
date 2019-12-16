using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 部门用户对应关系接口
    /// </summary>
    public interface IOrganizationUser : IEntity
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        string OrganizationID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        string UserID { get; set; }
    }
}
