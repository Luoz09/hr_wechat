using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sgms.Frame.Rights.Entities;

namespace Sgms.Frame.Rights
{
    /// <summary>
    /// 权限处理
    /// </summary>
    public class RightsProcess
    {
        /// <summary>
        /// Session前缀
        /// </summary>
        public static string SessionPrefix = String.Empty;

        /// <summary>
        /// 角色Action 对象关系
        /// </summary>
        private static IRoleAction[] _RoleAction;

        /// <summary>
        /// 角色ID
        /// </summary>
        private string[] RoleIDs
        {
            get
            {
                var result = HttpContext.Current.Session[SessionPrefix + "RoleIDs"] as string[];
                return result;
            }
            set
            {
                HttpContext.Current.Session[SessionPrefix + "RoleIDs"] = value;
            }
        }

        private IRightsDataObtain _RightsData;
        private IOrganization[] _AllOrgs;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rightsData"></param>
        public RightsProcess(IRightsDataObtain rightsData)
        {
            if (RoleIDs != null && RoleIDs.Length > 0) return;

            _RightsData = rightsData;

            _RoleAction = _RightsData.GetRoleActions().ToArray();

            List<string> roleList = new List<string>();

            if (_RightsData.GetOrganizations() != null && _RightsData.GetRoleOrganizations() != null)
            {
                _AllOrgs = _RightsData.GetOrganizations().ToArray();
                var orgIDs = GetOrganizationIDs(_RightsData.CurUser.ID);
                roleList.AddRange(GetRoles(orgIDs));
            }

            roleList.AddRange(GetRoles());
            RoleIDs = roleList.ToArray();

            _RightsData = null;
            _AllOrgs = null;
        }

        /// <summary>
        /// 根据 userID获取所在部门
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private string[] GetOrganizationIDs(string userID)
        {
            string[] orgIDs = _RightsData.GetOrganizationUsers().Where(m => m.UserID == userID).Select(m => m.OrganizationID).ToArray();
            var result = _AllOrgs.Where(m => orgIDs.Contains(m.ID)).SelectMany(m => GetParentOrganization(m)).ToArray();
            return result;
        }

        /// <summary>
        /// 获取所有的父部门
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        private List<string> GetParentOrganization(IOrganization org)
        {
            List<string> result = new List<string>();
            var curOrg = org;
            while (curOrg != null)
            {
                result.Add(curOrg.ID);
                curOrg = _AllOrgs.FirstOrDefault(m => m.ID == curOrg.ParentID);
            }
            return result;
        }


        /// <summary>
        /// 获取人员角色
        /// </summary>
        /// <returns></returns>
        private string[] GetRoles()
        {
            return _RightsData.GetRoleUsers().Where(m => m.UserID == _RightsData.CurUser.ID).Select(m => m.RoleID).ToArray();
        }

        /// <summary>
        /// 获取部门角色
        /// </summary>
        /// <param name="organizationIDs">部门</param>
        /// <returns></returns>
        private string[] GetRoles(IEnumerable<string> organizationIDs)
        {
            return _RightsData.GetRoleOrganizations().Where(m => organizationIDs.Contains(m.OrganizationID)).Select(m => m.RoleID).ToArray();
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public bool Authenticate(string appName, string actionName)
        {
            appName = appName.ToLower();
            actionName = actionName.ToLower();
            var entities = _RoleAction.Where(m => m.AppName.ToLower() == appName && (m.ActionName == null || m.ActionName.ToLower() == actionName));
            if (entities.All(m => m.ActionName == null))
            {
                return true;
            }
            else
            {
                return entities.Any(m => RoleIDs.Contains(m.RoleID));
            }

            //return _RoleAction.Any(m => m.AppName.ToLower() == appName && (m.ActionName == null || m.ActionName.ToLower() == actionName) && RoleIDs.Contains(m.RoleID));
        }
    }
}
