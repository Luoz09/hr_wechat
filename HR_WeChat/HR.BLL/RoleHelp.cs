using Jeedaa.Framework.DataAccess;
using Jeedaa.Support.SecuritySupport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace HR.BLL
{
    public class RoleHelp
    {

        //public static bool hasRoles(String userID, String appCode, String roleCodeNames)
        //{

        //    if (IsWebApp)
        //    {
        //        HttpContext context = HttpContext.Current;

        //        DataSet dsRole = (DataSet)context.Items[userID + "|" + appCode + "|role"];
        //        string[] roles = roleCodeNames.Split(',');
        //        if (dsRole == null)
        //        {
        //            dsRole = SecurityCheck.GetUserRoles(userID, appCode, UserValueType.Guid);
        //            context.Items[userID + "|" + appCode + "|role"] = dsRole;
        //        }
        //        foreach (DataRow dr in dsRole.Tables[0].Rows)
        //        {
        //            for (int i = 0; i < roles.Length; i++)
        //            {
        //                if (roles[i] == dr["CODE_NAME"].ToString())
        //                {
        //                    return true;
        //                }
        //            }
        //        }

        //    }

        //    else
        //    {
        //        return Jeedaa.Framework.Security.OU.DefaultPermissionFactory.GetFactory().HasRoles(userID, appCode, roleCodeNames);

        //    }

        //    return false;



        //}


        public bool hasRoles(String userID, String appCode, String roleCodeNames)
        {
            var application = new APPLICATIONSManager().GetApplicationByCodeName(appCode).FirstOrDefault();
            if (application != null) {
                var user = new OU_USERSManager().GetDataByUserID(userID);
                var rolesNames = roleCodeNames.Split(',');
                foreach (var item in rolesNames)
                {
                    var roles = new ROLESManager().GetRolesData().Where(m => m.APP_ID == application.ID && m.CODE_NAME == item).FirstOrDefault();
                    if (roles == null) return false;
                    var express = new EXPRESSIONSManager().GetExpressData().Where(m => m.DESCRIPTION == user.ALL_PATH_NAME && m.ROLE_ID == roles.ID).FirstOrDefault();
                    if (express == null) return false;
                }
                return true;
               
            }
            return false; 
        }


        /// <summary>
        /// 判断当前应用是否是Web应用
        /// </summary>
        private static bool IsWebApp
        {
            get
            {
                bool bResult = true;

                try
                {
                    HttpContext.Current.GetType();
                }
                catch (System.Exception)
                {
                    bResult = false;
                }

                return bResult;
            }
        }


        //public static Dictionary<String,String> GetRoleUserInDep(String appCode, String roleID, String depFullPahth)
        //{

        //    IDsUser[] users =Jeedaa.Framework.Security.OU.DefaultPermissionFactory.GetFactory().GetRolesUsers(depFullPahth, appCode, roleID, String.Empty);
        //    Dictionary<String, String> dicUser = new Dictionary<string, string>();

        //    for (int i = 0; i < users.Length; i++)
        //    {
        //        IDsUser user = users[i];
        //        if (!dicUser.ContainsKey(user.ID))
        //        {
        //            dicUser.Add(user.ID, user.DisplayName);
        //        }
        //    }
        //    return dicUser;
        //}

        public static List<String> GetRoleUserDepIDs(String userID, String appCode, String roleID)
        {


            List<string> list = new List<string>();

            string sql = string.Format(@"
with t as
(
 select REPLACE(REPLACE(REPLACE(d.Expression,'" + "\"" + @"',''),' ',''),'BelongTo(','') as Expression from functions a join
 ROLE_TO_FUNCTIONS b on a.ID=b.FUNC_ID join
 ROLES c on c.ID=b.ROLE_ID join EXPRESSIONS d on  c.ID=d.ROLE_ID
 where a.CODE_NAME  ='{0}'  and  a.APP_ID 
 in( select ID from APPLICATIONS where  CODE_NAME= '{1}')
) 
select  ParentID from 
(
---角色里直接加人员
union
---角色里加人员组
select USER_GUID as userID, USER_PARENT_GUID as ParentID from GROUP_USERS where 
 GROUP_GUID in
 (
 select  substring(Expression,8,36)  from t where EXPRESSION like 'GROUPS,%)')
 union 
 ---人员组里加机构
select c.USER_GUID as userID,c.PARENT_GUID as ParentID from
(
select  substring(Expression,15,36) as  OrganID from t    where EXPRESSION like 'ORGANIZATIONS,%'
)a join ORGANIZATIONS b
on b.GUID=a.OrganID join OU_USERS c on  c.ALL_PATH_NAME like b.ALL_PATH_NAME+'\%' 
and c.Status=1
)a   where a.UserID='{2}'
", roleID, appCode, userID);
            DataTable dt = ExecSql.LoadDataTable("Jeedaa_Base", sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(dt.Rows[i]["ParentID"].ToString());
                }
            }

            return list;
        }



    }
}
