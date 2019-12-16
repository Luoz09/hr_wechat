using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Rights.Entities;

namespace Sgms.Frame.Rights
{
    /// <summary>
    /// 权限数据获取接口
    /// </summary>
    public interface IRightsDataObtain
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        IUser CurUser { get; }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        bool Login();
        /// <summary>
        /// 根据钉钉返回用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IUser GetAdminByCode(string code);

        /// <summary>
        /// 获取角色功能列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRoleAction> GetRoleActions();

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <returns></returns>
        IEnumerable<IOrganization> GetOrganizations();

        /// <summary>
        /// 获取部门用户
        /// </summary>
        /// <returns></returns>
        IEnumerable<IOrganizationUser> GetOrganizationUsers();

        /// <summary>
        /// 获取角色部门
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRoleOrganization> GetRoleOrganizations();

        /// <summary>
        /// 获取角色用户
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRoleUser> GetRoleUsers();
    }
}
