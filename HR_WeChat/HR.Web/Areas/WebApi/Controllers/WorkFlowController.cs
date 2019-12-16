using System;
using System.Collections.Generic; 
using System.Web.Mvc; 
using Sgms.Frame.Sys;
using Jeedaa.Framework.WFManagement;
using System.Xml;
using System.Data;
using Jeedaa.Framework.Tools;
using Jeedaa.Framework.DataAccess;
using Jeedaa.Framework.WFManagement.DataEntity;
using Jeedaa.Framework.WFManagement.DataAdapter;
using Jeedaa.Framework.Security.OU;
using System.Collections;
using Jeedaa.Framework.Security;
using HR.Web.Areas.WebApi.Codes;
using Newtonsoft.Json.Linq;
using Jeedaa.Framework.WFManagement.InterFace;

namespace HR.Web.Areas.WebApi.Controllers
{
    public class WorkflowController: WebApiNormalPage
    {

        public const string OPERATIONS_TAG = "OPERATIONS";
        public const string USERS_TAG = "USERS";
        public const string ORGANIZATIONS_TAG = "ORGANIZATIONS";
        public const string ROLES_TAG = "ROLES";
        protected const string ACTIVITY_WITHDRAW_TYPE = "Withdraw";
        protected const string NEXT_ACTIVITY_LABEL_TEXT = "选择下一步 ";
        protected const string NEXT_USER_LABEL_TEXT = "选择用户 ";
        protected const string C_WITHDRAW_TAG = "返回";


         

        //获取当前工作流状态
        public ContentResult GetCurrentActivity()
        {
            try
            {
                String activityInstanceID = Request.Params["activityID"];
                String processInstanceID = String.Empty;
                String processDKey = String.Empty;
                String activityDCode = String.Empty;
                String activityName = string.Empty;
                String activityUrl = String.Empty;
                bool allowSelectNextActivity = true;
                bool isExitNode = false;
                bool isOpening = true;
                int CurrentActivityCount = 0;
                //没有传入活动实例，为新启流程
                if (!String.IsNullOrEmpty(activityInstanceID))
                {
                    String sql = "select top 1 a.Activity_Desc_Code,P.Process_ID,p.Process_Desc_Code from wf_Activity a inner join wf_process p on a.process_ID = p.process_ID where activity_ID = {0} and a.STATE = 'open.running'";
                    sql = string.Format(sql, Jeedaa.Framework.DataAccess.GetSqlStr.AddCheckQuotationMark(activityInstanceID));
                    DataTable dt = Jeedaa.Framework.DataAccess.ExecSql.LoadDataTable("Jeedaa_Base", sql);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        processDKey = dr["Process_Desc_Code"].ToString();
                        activityDCode = dr["Activity_Desc_Code"].ToString();
                        processInstanceID = dr["Process_ID"].ToString();
                        WfActivityDescriptor actDesc = WfFactory.GetFacade().Query.GetActivityDescriptorByKey(
                        processDKey, activityDCode);
                        isExitNode = actDesc.IsExitNode();
                        activityName = actDesc.Name;
                        allowSelectNextActivity = actDesc.AllowSelectNextActivity;
                        activityUrl = actDesc.ActivityURL;
                    }
                    CurrentActivityCount = dt.Rows.Count;

                }

                JObject jObject = new JObject();
                jObject.Add("ProcessDKey", processDKey);
                jObject.Add("ProcessInstanceID", processInstanceID);
                jObject.Add("ActivityDCode", activityDCode);
                jObject.Add("ActivityInstanceID", activityInstanceID);
                jObject.Add("ActivityName", activityName);
                jObject.Add("ActivityUrl", activityUrl);
                jObject.Add("IsExitNode", isExitNode);
                jObject.Add("AllowSelectNextActivity", allowSelectNextActivity);
                jObject.Add("IsCurrentActivity", CurrentActivityCount);
                return DisplayJson(jObject.ToString());
            }
            catch (Exception ex)
            {
               // SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }

        }




        //获取当前工作流状态
        public ContentResult GetCurrentUser()
        {
            try
            {
                String resourceID = Request.Params["resourceID"];
                JArray jUser = new JArray();
                //没有传入活动实例，为新启流程
                if (!String.IsNullOrEmpty(resourceID))
                {
                    String sql = "select * from Wf_WorkItem where Resource_ID ={0} and DealState=0";
                    sql = string.Format(sql, GetSqlStr.AddCheckQuotationMark(resourceID));


                    DataTable dt = Jeedaa.Framework.DataAccess.ExecSql.LoadDataTable("Jeedaa_Base", sql);
                    foreach (DataRow dr in dt.Rows)

                    {
                        JObject jo = new JObject();
                        jo.Add("Purpose", dr["PURPOSE"].ToString());
                        string uid = dr["DESTINATION"].ToString();
                        IDsObject ds = DefaultOUFactory.GetFactory().GetObject(uid, String.Empty);
                        jo.Add("UID", uid);
                        jo.Add("UName", ds.DisplayName);
                        jo.Add("FullPath", ds.FullPath);
                        jUser.Add(jo);


                    }



                }


                return DisplayJson(jUser.ToString());
            }
            catch (Exception ex)
            {
                //SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }

        }
        public ContentResult GetCurrentUserHistory()
        {
            try
            {
                String resourceID = Request.Params["resourceID"];
                JArray jUser = new JArray();
                //没有传入活动实例，为新启流程
                if (!String.IsNullOrEmpty(resourceID))
                {
                    String sql = "select * from WF_WorkItemHistory where Resource_ID ={0}";
                    sql = string.Format(sql, GetSqlStr.AddCheckQuotationMark(resourceID));


                    DataTable dt = Jeedaa.Framework.DataAccess.ExecSql.LoadDataTable("Jeedaa_Base", sql);
                    foreach (DataRow dr in dt.Rows)

                    {
                        JObject jo = new JObject();
                        jo.Add("Purpose", dr["PURPOSE"].ToString());
                        string uid = dr["DESTINATION"].ToString();
                        IDsObject ds = DefaultOUFactory.GetFactory().GetObject(uid, String.Empty);
                        jo.Add("UID", uid);
                        jo.Add("UName", ds.DisplayName);
                        jo.Add("FullPath", ds.FullPath);
                        jUser.Add(jo);


                    }



                }


                return DisplayJson(jUser.ToString());
            }
            catch (Exception ex)
            {
               // SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }


        }


        //获取当前工作流状态
        public ContentResult GetWFlog()
        {
            try
            {
                String resourceID = Request.Params["ResourceID"];
                String activityInstanceID = Request.Params["ActivityID"];

                WhereCondition wc = new WhereCondition();
                wc.AppendCondition("RESOURCE_ID", resourceID);
                OrderByCondition obc = new OrderByCondition();
                obc.AppendCondition("SORT_ID");

                JArray jWFLogs = new JArray();
                if (!String.IsNullOrEmpty(activityInstanceID))//流转过程中
                {
                    //把CurrentActivityID改成CurrentActivityDescKey，胡敏2008-1-10修改
                    WFLog[] wflogs = WFLogAdapter.LoadObjectList(wc, obc);
                    for (int i = 0; i < wflogs.Length; i++)
                    {
                        WFLog log = wflogs[i];
                        JObject jObject = new JObject();
                        jObject.Add("ProcessInstanceID", log.PROCESSID);
                        jObject.Add("ActivityInstanceID", log.ACTIVITYID);
                        jObject.Add("ProcessDKey", log.ProcessKey);
                        jObject.Add("ActivityDCode", log.ActivtyKey);
                        jObject.Add("ActDate", log.ActDate);
                        jObject.Add("WorkItemID", log.WorkItemID);
                        jObject.Add("DeliverTime", log.DeliverTime);
                        XmlDocument wfLogXml = XMLHelper.CreateDomDocument(log.TemplateDataXML);
                        XmlNode rootNode = wfLogXml.DocumentElement;
                        XmlNode logNode = rootNode.ChildNodes[0];
                        jObject.Add("Title", XMLHelper.GetSingleNodeText(logNode, "Title"));
                        jObject.Add("Opinion", XMLHelper.GetSingleNodeText(logNode, "Opinion"));
                        jObject.Add("AppendOpinion", XMLHelper.GetSingleNodeText(logNode, "AppendOpinion"));
                        bool isProcessInstanceEnd = false;
                        bool isActivityInstanceBackWard = false;
                        if (!String.IsNullOrEmpty(XMLHelper.GetSingleNodeText(logNode, "ProcessInstanceEnd")))
                        {
                            isProcessInstanceEnd = bool.Parse(XMLHelper.GetSingleNodeText(logNode, "ProcessInstanceEnd"));
                        }
                        if (!String.IsNullOrEmpty(XMLHelper.GetSingleNodeText(logNode, "ActivityInstanceBackWard")))
                        {
                            isActivityInstanceBackWard = bool.Parse(XMLHelper.GetSingleNodeText(logNode, "ActivityInstanceBackWard"));
                        }
                        jObject.Add("ProcessInstanceEnd", isProcessInstanceEnd);
                        jObject.Add("ActivityInstanceBackWard", isActivityInstanceBackWard);
                        jObject.Add("OperatorUID", XMLHelper.GetSingleNodeText(logNode, "OperatorUID"));
                        jObject.Add("OperatorUName", XMLHelper.GetSingleNodeText(logNode, "OperatorUName"));
                        jWFLogs.Add(jObject);
                    }
                }
                else// if(this.DefaultProcessDescCode == string.Empty)//流程结束
                {
                    WFLogHistory[] wflogHiss = WFLogHistoryAdapter.LoadObjectList(wc, obc);
                    for (int i = 0; i < wflogHiss.Length; i++)
                    {
                        WFLogHistory log = wflogHiss[i];
                        JObject jObject = new JObject();
                        jObject.Add("ProcessInstanceID", log.PROCESSID);
                        jObject.Add("ActivityInstanceID", log.ACTIVITYID);
                        jObject.Add("ProcessDKey", log.ProcessKey);
                        jObject.Add("ActivityDCode", log.ActivtyKey);
                        jObject.Add("ActDate", log.ActDate);
                        jObject.Add("WorkItemID", log.WorkItemID);
                        jObject.Add("DeliverTime", log.DeliverTime);
                        XmlDocument wfLogXml = XMLHelper.CreateDomDocument(log.TemplateDataXML);
                        XmlNode rootNode = wfLogXml.DocumentElement;
                        XmlNode logNode = rootNode.ChildNodes[0];
                        jObject.Add("Title", XMLHelper.GetSingleNodeText(logNode, "Title"));
                        jObject.Add("Opinion", XMLHelper.GetSingleNodeText(logNode, "Opinion"));
                        jObject.Add("AppendOpinion", XMLHelper.GetSingleNodeText(logNode, "AppendOpinion"));
                        bool isProcessInstanceEnd = false;
                        bool isActivityInstanceBackWard = false;
                        if (!String.IsNullOrEmpty(XMLHelper.GetSingleNodeText(logNode, "ProcessInstanceEnd")))
                        {
                            isProcessInstanceEnd = bool.Parse(XMLHelper.GetSingleNodeText(logNode, "ProcessInstanceEnd"));
                        }
                        if (!String.IsNullOrEmpty(XMLHelper.GetSingleNodeText(logNode, "ActivityInstanceBackWard")))
                        {
                            isActivityInstanceBackWard = bool.Parse(XMLHelper.GetSingleNodeText(logNode, "ActivityInstanceBackWard"));
                        }
                        jObject.Add("ProcessInstanceEnd", isProcessInstanceEnd);
                        jObject.Add("ActivityInstanceBackWard", isActivityInstanceBackWard);
                        jObject.Add("OperatorUID", XMLHelper.GetSingleNodeText(logNode, "OperatorUID"));
                        jObject.Add("OperatorUName", XMLHelper.GetSingleNodeText(logNode, "OperatorUName"));
                        jWFLogs.Add(jObject);
                    }
                }

                return DisplayJson(jWFLogs.ToString());
            }
            catch (Exception ex)
            {
               // SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }


        }

        public  ContentResult GetNextActivityResource()
        {

            try
            {
                string processDKey = Request.Params["processDKey"];
                String nextActivityKey = Request.Params["nextActivityKey"];
                string userID = "";
                try
                {
                    userID = SysPara.CurAdmin.ID;
                }
                catch
                {
                    userID = Request.Params["userID"];
                }
                String activityID = Request.Params["activityID"];
                String activityKey = Request.Params["activityKey"];

                String otherDepID = Request.Params["otherDepID"] == null ? string.Empty : Request.Params["otherDepID"];
                string parallelDept = Request.Params["parallelDept"];
                string processID = string.Empty;
                String appName = Request.Params["appName"];

                //没有传入活动实例，为新启流程
                if (!String.IsNullOrEmpty(activityID))
                {
                    String sql = "select top 1 a.Activity_Desc_Code,P.Process_ID,p.Process_Desc_Code from wf_Activity a inner join wf_process p on a.process_ID = p.process_ID where activity_ID = {0}";
                    sql = string.Format(sql, Jeedaa.Framework.DataAccess.GetSqlStr.AddCheckQuotationMark(activityID));
                    DataTable dt = Jeedaa.Framework.DataAccess.ExecSql.LoadDataTable("Jeedaa_Base", sql);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        processDKey = dr["Process_Desc_Code"].ToString();
                        activityKey = dr["Activity_Desc_Code"].ToString();
                        processID = dr["Process_ID"].ToString();
                    }

                }

                if (String.IsNullOrEmpty(appName))
                {
                    WfProcessDescriptor processDesc = GetProcessDesc(processDKey);
                    appName = processDesc.AppName;

                }
                XmlDocument resultDoc = CreateResourceDoc(nextActivityKey);
                XmlNode resultRoot = resultDoc.DocumentElement;
                WfActivityDescriptor activityDesc = GetActivityDesc(processDKey, nextActivityKey);
                XmlDocument resDoc = WfResource.GetSequencePerformer(activityDesc, userID, "", otherDepID);
                //动态形成资源定义
                if (!String.IsNullOrEmpty(parallelDept))
                {
                    XmlDocument parallelDoc = XMLHelper.CreateDomDocument(parallelDept);

                    XmlNode deptNode = parallelDoc.DocumentElement.FirstChild;
                    XmlNode tempNode;

                    while (deptNode != null)
                    {
                        XmlNodeList rolesNodeList = resDoc.DocumentElement.SelectNodes("Resource[@type = '" + ResourceType.Role.ToString() + "']");

                        foreach (XmlNode roleNode in rolesNodeList)
                        {
                            //形成角色结点<object OBJECTCLASS="role" key="角色标识" department="departmentDN串" name="角色名称" />
                            tempNode = XMLHelper.AppendNode(deptNode, "object");

                            XMLHelper.AppendAttr(tempNode, "OBJECTCLASS", ROLES_TAG);
                            XMLHelper.AppendAttr(tempNode, "KEY", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(roleNode, "value"));
                            XMLHelper.AppendAttr(tempNode, "department", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(deptNode, "ALL_PATH_NAME"));
                            XMLHelper.AppendAttr(tempNode, "DISPLAY_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(roleNode, "name"));
                        }

                        deptNode = deptNode.NextSibling;
                    }

                    resultRoot.InnerXml = parallelDoc.DocumentElement.OuterXml;
                }
                else
                {
                    //正常情况，不需动态生成资源定义，取出资源
                    XmlNode resNode = resDoc.DocumentElement.FirstChild;
                    XmlNode node;
                    Dictionary<string, string> existUsers = new Dictionary<string, string>();
                    while (resNode != null)
                    {
                        switch (XSDHelper.GetXSDColumnAttr(resNode, "type"))
                        {
                            case "BeforeActivity":
                                string beforeActivityKey = Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "BeforeActivityKey");
                                WhereCondition wc = new WhereCondition();
                                wc.AppendCondition("Process_ID", processID);
                                wc.AppendCondition("ActivtyKey", beforeActivityKey);
                                OrderByCondition obc = new OrderByCondition();
                                obc.AppendCondition("ActDate", "desc");
                                WFLog[] logs = WFLogAdapter.LoadObjectList(wc, obc);
                                for (int i = 0; i < logs.Length; i++)
                                {

                                    WFLog log = logs[i];

                                    IDsObject user = DefaultOUFactory.GetFactory().GetObject(log.ActorID, Jeedaa.Framework.Security.OU.SchemaType.Users, string.Empty);
                                    if (!existUsers.ContainsKey(user.ID))
                                    {
                                        node = XMLHelper.AppendNode(resultRoot, USERS_TAG, "");
                                        XMLHelper.AppendAttr(node, "GUID", log.ActorID);
                                        XMLHelper.AppendAttr(node, "OBJECTCLASS", USERS_TAG);
                                        XMLHelper.AppendAttr(node, "OBJ_NAME", user.DisplayName);
                                        XMLHelper.AppendAttr(node, "DISPLAY_NAME", user.DisplayName);
                                        XMLHelper.AppendAttr(node, "ALL_PATH_NAME", user.FullPath);
                                        existUsers.Add(user.ID, user.ID);
                                    }
                                }
                                break;
                            case "User":
                                string tUserID = Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "guid");
                                if (!existUsers.ContainsKey(tUserID))
                                {
                                    node = XMLHelper.AppendNode(resultRoot, USERS_TAG, "");
                                    XMLHelper.AppendAttr(node, "GUID", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "guid"));
                                    XMLHelper.AppendAttr(node, "OBJECTCLASS", USERS_TAG);
                                    XMLHelper.AppendAttr(node, "OBJ_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "name"));
                                    XMLHelper.AppendAttr(node, "DISPLAY_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "name"));
                                    XMLHelper.AppendAttr(node, "ALL_PATH_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "pathName"));
                                    existUsers.Add(tUserID, tUserID);
                                }
                                break;

                            case "Department":
                                string depID = Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "guid");
                                if (!existUsers.ContainsKey(depID))
                                {
                                    DsDepartment dep = new DsDepartment();
                                    dep = (DsDepartment)DefaultOUFactory.GetFactory().GetObject(depID, String.Empty);
                                    String[] userIDs = DefaultOUFactory.GetFactory().GetAllSubObjectsIDs(dep, Jeedaa.Framework.Security.OU.SchemaType.Users);
                                    IDsObject[] users = DefaultOUFactory.GetFactory().GetDsUsersByMultiID(userIDs, String.Empty, true);
                                    foreach (IDsObject user in users)
                                    {
                                        if (!existUsers.ContainsKey(user.ID))
                                        {
                                            node = XMLHelper.AppendNode(resultRoot, ORGANIZATIONS_TAG, "");
                                            XMLHelper.AppendAttr(node, "ALL_PATH_NAME", user.FullPath);
                                            XMLHelper.AppendAttr(node, "GUID", user.ID);
                                            XMLHelper.AppendAttr(node, "OBJECTCLASS", USERS_TAG);
                                            XMLHelper.AppendAttr(node, "OBJ_NAME", dep.DisplayName);
                                            XMLHelper.AppendAttr(node, "DISPLAY_NAME", user.DisplayName);
                                            existUsers.Add(user.ID, user.ID);
                                        }
                                    }

                                    /*
                                    node = XMLHelper.AppendNode(resultRoot, ORGANIZATIONS_TAG, "");
                                    XMLHelper.AppendAttr(node, "ALL_PATH_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "value"));
                                    XMLHelper.AppendAttr(node, "GUID", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "guid"));
                                    XMLHelper.AppendAttr(node, "OBJECTCLASS", ORGANIZATIONS_TAG);
                                    XMLHelper.AppendAttr(node, "OBJ_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "name"));
                                    XMLHelper.AppendAttr(node, "DISPLAY_NAME", Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "name"));
                                    */
                                    existUsers.Add(depID, depID);
                                }
                                break;
                            case "Role":
                                string roleKey = Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "value");
                                string roleName = Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "name");
                                string rootOU = Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(resNode, "department");

                                if (rootOU.Length == 0)
                                    rootOU = OUAndPermissionsConfig.GetConfig().RootOUPath;

                                node = resultRoot.SelectSingleNode("ROLES[@KEY='" + roleKey + "']");

                                /*
                                if (node != null)
                                {
                                    XMLHelper.AppendAttr(node, "department", rootOU + "^" + Jeedaa.Framework.Tools.XSDHelper.GetXSDColumnAttr(node, "department"));
                                }
                                else
                                {

                                    node = XMLHelper.AppendNode(resultRoot, ROLES_TAG, "");

                                    XMLHelper.AppendAttr(node, "OBJECTCLASS", ROLES_TAG);
                                    XMLHelper.AppendAttr(node, "KEY", roleKey);
                                    XMLHelper.AppendAttr(node, "department", rootOU);
                                    XMLHelper.AppendAttr(node, "DISPLAY_NAME", roleName);
                                    XMLHelper.AppendAttr(node, "ALL_PATH_NAME", roleName);
                                }
                                */
                                //IDsObject[] objectCollection = DefaultPermissionFactory.GetFactory().GetChildrenInRoles(rootOU, appName, roleKey, true, true);
                                IDsObject[] objectCollection = DefaultPermissionFactory.GetFactory().GetDepartmentAndUserInRoles(rootOU, appName, roleKey, true, true);
                                //IDsObject[] objectCollection = DefaultPermissionFactory.GetFactory().GetRolesUsers(rootOU, appName,roleKey,string.Empty);
                                //GetUnitNodeAttrValue(unitNode, "GUID", attribType), string.Empty);

                                for (int i = 0; i < objectCollection.Length; i++)
                                {

                                    if (objectCollection[i].ObjectType == Jeedaa.Framework.Security.OU.SchemaType.Organizations)
                                    {
                                        String[] idStringS = DefaultOUFactory.GetFactory().GetAllSubObjectsIDs((IDsDepartment)objectCollection[i], Jeedaa.Framework.Security.OU.SchemaType.Users);
                                        IDsObject[] ids = DefaultOUFactory.GetFactory().GetDsUsersByMultiID(idStringS, String.Empty, false);
                                        foreach (IDsObject ds in ids)
                                        {
                                            if (!existUsers.ContainsKey(ds.ID))
                                            {
                                                XmlNode currentNode = XMLHelper.AppendNode(resultRoot, ROLES_TAG, "");


                                                XMLHelper.AppendAttr(currentNode, "OBJECTCLASS", objectCollection[i].ObjectType.ToString().ToUpper());
                                                XMLHelper.AppendAttr(currentNode, "ALL_PATH_NAME", ds.FullPath);
                                                XMLHelper.AppendAttr(currentNode, "DISPLAY_NAME", ds.DisplayName);
                                                XMLHelper.AppendAttr(currentNode, "OBJ_NAME", roleName);
                                                XMLHelper.AppendAttr(currentNode, "GUID", ds.ID);
                                                existUsers.Add(ds.ID, ds.ID);


                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (!existUsers.ContainsKey(objectCollection[i].ID))
                                        {
                                            XmlNode currentNode = XMLHelper.AppendNode(resultRoot, ROLES_TAG, "");


                                            XMLHelper.AppendAttr(currentNode, "OBJECTCLASS", objectCollection[i].ObjectType.ToString().ToUpper());
                                            XMLHelper.AppendAttr(currentNode, "ALL_PATH_NAME", objectCollection[i].FullPath);
                                            XMLHelper.AppendAttr(currentNode, "DISPLAY_NAME", objectCollection[i].DisplayName);
                                            XMLHelper.AppendAttr(currentNode, "OBJ_NAME", roleName);
                                            XMLHelper.AppendAttr(currentNode, "GUID", objectCollection[i].ID);
                                            existUsers.Add(objectCollection[i].ID, objectCollection[i].ID);
                                        }
                                    }
                                }

                                break;
                        }

                        resNode = resNode.NextSibling;
                    }
                }

                JArray jArray = new JArray();
                XmlNode rootNode = resultDoc.DocumentElement;
                for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                {

                    XmlNode node = rootNode.ChildNodes[i];
                    JObject jObject = new JObject();
                    jObject.Add("UserFromType", node.Name);
                    jObject.Add("ObjectClass", XMLHelper.GetAttributeText(node, "OBJECTCLASS"));
                    jObject.Add("DisplayName", XMLHelper.GetAttributeText(node, "DISPLAY_NAME"));
                    jObject.Add("ObjName", XMLHelper.GetAttributeText(node, "OBJ_NAME"));
                    jObject.Add("AllPathName", XMLHelper.GetAttributeText(node, "ALL_PATH_NAME"));
                    jObject.Add("GUID", XMLHelper.GetAttributeText(node, "GUID"));

                    jArray.Add(jObject);
                }
                return DisplayJson(jArray.ToString());
            }
            catch (Exception ex)
            {
               // SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }

        }

        private XmlDocument CreateResourceDoc(string actName)
        {
            XmlDocument resultDoc = XMLHelper.CreateDomDocument("<OPERATIONS/>");

            resultDoc.DocumentElement.SetAttribute("OBJECTCLASS", OPERATIONS_TAG);
            resultDoc.DocumentElement.SetAttribute("ALL_PATH_NAME", actName);
            resultDoc.DocumentElement.SetAttribute("OBJ_NAME", actName);
            resultDoc.DocumentElement.SetAttribute("DISPLAY_NAME", actName);

            return resultDoc;
        }

        private WfActivityDescriptor GetActivityDesc(string processDescKey, string activityDescKey)
        {
            WfWorkflowMgr wfMgr = new WfWorkflowMgr();
            WfActivityDescriptor actDesc = wfMgr.GetProcessMgr(processDescKey).GetProcessDescriptor().GetActivity(activityDescKey);

            return actDesc;
        }


        private WfProcessDescriptor GetProcessDesc(string processDescKey)
        {
            WfWorkflowMgr wfMgr = new WfWorkflowMgr();
            WfProcessDescriptor processDesc = wfMgr.GetProcessMgr(processDescKey).GetProcessDescriptor();

            return processDesc;
        }






        public ContentResult GetNextActivitiesList()
        {
            try
            {
                IList activitiesList;
                string processDKey = Request.Params["processDKey"];
                String userID = Request.Params["userID"];
                String activityID = Request.Params["activityID"];
                String appName = Request.Params["appName"];

                String activityKey = "N1";
                String otherDepID = Request.Params["otherDepID"];
                string parallelDept = Request.Params["parallelDept"];
                string processID = string.Empty;

                //没有传入活动实例，为新启流程
                if (!String.IsNullOrEmpty(activityID))
                {
                    String sql = "select top 1 a.Activity_Desc_Code,P.Process_ID,p.Process_Desc_Code from wf_Activity a inner join wf_process p on a.process_ID = p.process_ID where activity_ID = {0}";
                    sql = string.Format(sql, Jeedaa.Framework.DataAccess.GetSqlStr.AddCheckQuotationMark(activityID));
                    DataTable dt = Jeedaa.Framework.DataAccess.ExecSql.LoadDataTable("Jeedaa_Base", sql);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        processDKey = dr["Process_Desc_Code"].ToString();
                        activityKey = dr["Activity_Desc_Code"].ToString();
                        processID = dr["Process_ID"].ToString();
                    }

                }
                WfContext wfContext = new WfContext();
                wfContext.ProcessID = processID;
                wfContext.ActivityDescKey = activityKey;
                wfContext.AppName = appName;
                wfContext["WorkflowInfo"] = new Hashtable();
                Hashtable workflowInfoHT = (Hashtable)wfContext["WorkflowInfo"];
                workflowInfoHT["AppName"] = appName;
                workflowInfoHT["ProcessDescCode"] = processDKey;
                //WfCo[]
                if (String.IsNullOrEmpty(activityID))
                {
                    ExceptionTools.TrueThrow(String.IsNullOrEmpty(processDKey), "起始流程ProcessDescKey不能为空！");
                    //activitiesList = WfAppFacade.GetNextActivitiesInfo(DefaultProcessDescCode, this.WfContext);
                    activitiesList = WfFactory.GetFacade().DescriptionReader.GetNextActivitiesInfo(
                        processDKey, wfContext);
                }
                else
                    //activitiesList = WfAppFacade.GetProcessNextActivitiesInfo(this.CurrentActivityID, this.WfContext);
                    activitiesList = WfFactory.GetFacade().DescriptionReader.GetProcessNextActivitiesInfo(
                        activityID, wfContext);



                if (activitiesList.Count == 0) return null;

                JArray jArray = new JArray();
                for (int i = 0; i < activitiesList.Count; i++)
                {
                    WfPackingObject act = (WfPackingObject)activitiesList[i];
                     
                    string itemNodeName = act.ActivityObject.Descriptor.Name.ToString();
                    if (act.ActivityType == ACTIVITY_WITHDRAW_TYPE)
                        itemNodeName = C_WITHDRAW_TAG + itemNodeName;

                    JObject jObject = new JObject();
                    jObject.Add("nodeName", itemNodeName);
                    jObject.Add("activityKey", act.ActivityObject.Descriptor.Key);
                    jObject.Add("codeName", act.ActivityObject.Descriptor.CodeName == null ?
                        string.Empty : act.ActivityObject.Descriptor.CodeName);
                    jObject.Add("usesrID", act.UserID);
                    jObject.Add("userPath", act.UserPath);
                    jObject.Add("displayName", act.DisplayName);
                    jObject.Add("pointFlag", act.SpecialPoint);
                    jObject.Add("dynamicResource", act.DynamicResource);
                    //				AddAttribute(currentItem, "isExit", act.ActivityObject.IsExitNode().ToString().ToLower());
                    jObject.Add("isExit", "false");
                    jObject.Add("defaultSelect",
                       act.ActivityObject.Descriptor.DefaultSelectUsers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
                    jObject.Add("selectSingleUser",
                        act.ActivityObject.Descriptor.SelectSingleUser ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
                    jArray.Add(jObject);
                }



                return DisplayJson(jArray.ToString());
            }
            catch (Exception ex)
            {
              //  SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }


        }


        private struct WFLogStruct
        {
           public string Title;
           public String Opinion;
           public   String AppendOpinion;
           public  Boolean ProcessInstanceEnd ;
            public Boolean ActivityInstanceBackWard;
            public String OperatorUID;
            public String OperatorUName;
           
            public String TemplateName;
            public String ResourceID;
            public String processID;
            public String ProcessKey;
            public String ActivityID;
            public String ActivityDCode;
          
            public String WorkItemID;
            public DateTime ActDate;
           
        }


        private void WriteWFLog(WFLogStruct log)
        {
            //插入永久历史日志
            WFLog wflog = new WFLog();
            wflog.RESOURCEID = log.ResourceID;
            wflog.PROCESSID = log.processID;

            wflog.ProcessKey = log.ProcessKey;
            wflog.ACTIVITYID = log.ActivityID;
            wflog.ActivtyKey = log.ActivityDCode;
            wflog.ActorID = log.OperatorUID;
            wflog.ActorName = log.OperatorUName;
            wflog.ActDate = log.ActDate;

            wflog.WorkItemID = log.WorkItemID;
           
            wflog.TemplateID = "TemplateName";
           

            XmlDocument doc = XMLHelper.CreateDomDocument("<Template />");
            XmlNode rootNode = doc.DocumentElement;
            
            XmlNode templateNode = XMLHelper.AppendNode(rootNode, wflog.TemplateID);
            XMLHelper.AppendNode(templateNode, "Title", log.Title);
            if (!String.IsNullOrEmpty(log.Opinion))
            {
                XMLHelper.AppendNode(templateNode,"Opinion", log.Opinion);
                XMLHelper.AppendNode(templateNode, "AppendOpinionTextBoxLines", "5");
            }
            XMLHelper.AppendNode(templateNode, "AppendOpinion", log.AppendOpinion);
            XMLHelper.AppendNode(templateNode, "ProcessInstanceEnd", log.ProcessInstanceEnd.ToString());
            XMLHelper.AppendNode(templateNode, "ActivityInstanceBackWard", log.ActivityInstanceBackWard.ToString());
            XMLHelper.AppendNode(templateNode, "OperatorUID", log.OperatorUID);
            XMLHelper.AppendNode(templateNode, "OperatorUName", log.OperatorUName);
            XMLHelper.AppendNode(templateNode, "OperateTime", log.ActDate.ToString("yyyy-MM-dd HH:mm"));
            wflog.TemplateDataXML = doc.DocumentElement.OuterXml;
            wflog.DeliverTime = log.ActDate;
            WFLogAdapter.InsertObject(wflog);

        }


        private void WriteWFLogHistory(WFLogStruct log)
        {
            //插入永久历史日志
            WFLogHistory wflog = new WFLogHistory();
            wflog.RESOURCEID = log.ResourceID;
            wflog.PROCESSID = log.processID;

            wflog.ProcessKey = log.ProcessKey;
            wflog.ACTIVITYID = log.ActivityID;
            wflog.ActivtyKey = log.ActivityDCode;
            wflog.ActorID = log.OperatorUID;
            wflog.ActorName = log.OperatorUName;
            wflog.ActDate = log.ActDate;

            wflog.WorkItemID = log.WorkItemID;

            wflog.TemplateID = "TemplateName";


            XmlDocument doc = XMLHelper.CreateDomDocument("<Template />");
            XmlNode rootNode = doc.DocumentElement;

            XmlNode templateNode = XMLHelper.AppendNode(rootNode, wflog.TemplateID);
            XMLHelper.AppendNode(templateNode, "Title", log.Title);
            if (!String.IsNullOrEmpty(log.Opinion))
            {
                XMLHelper.AppendNode(templateNode, "Opinion", log.Opinion);
                XMLHelper.AppendNode(templateNode, "AppendOpinionTextBoxLines", "5");
            }
            XMLHelper.AppendNode(templateNode, "AppendOpinion", log.AppendOpinion);
            XMLHelper.AppendNode(templateNode, "ProcessInstanceEnd", log.ProcessInstanceEnd.ToString());
            XMLHelper.AppendNode(templateNode, "ActivityInstanceBackWard", log.ActivityInstanceBackWard.ToString());
            XMLHelper.AppendNode(templateNode, "OperatorUID", log.OperatorUID);
            XMLHelper.AppendNode(templateNode, "OperatorUName", log.OperatorUName);
            XMLHelper.AppendNode(templateNode, "OperateTime", log.ActDate.ToString("yyyy-MM-dd HH:mm"));
            wflog.TemplateDataXML = doc.DocumentElement.OuterXml;
            wflog.DeliverTime = log.ActDate;
            WFLogHistoryAdapter.InsertObject(wflog);

        }

        public ContentResult WriteWFEngine()
        {



            try
            {
                JObject engineData = JObject.Parse(Request.Params["EngineData"]);
                String guid = Guid.NewGuid().ToString();

                WFEntryData entryData = new WFEntryData();
                entryData.WFPoolEntry = new EnginePool();
                EnginePool ePool = entryData.WFPoolEntry;

                String userID = engineData.Value<String>("UserID");
                String userName = engineData.Value<String>("UserName");
                String resourceID = engineData.Value<String>("ResourceID");
                String processInstanceID = engineData.Value<String>("ProcessInstanceID");
                String activityInstanceID = engineData.Value<String>("ActivityInstanceID");
                String activityName = engineData.Value<String>("ActivityName");
                String workItemID = engineData.Value<String>("WorkItemID");
                if (workItemID == null)
                {
                    workItemID = "0";
                }
                Boolean isWithdraw = engineData.Value<Boolean>("IsWithdraw");
                Boolean isProcessInstanceEnd = engineData.Value<Boolean>("IsProcessInstanceEnd");
                String opinion = engineData.Value<String>("Opinion");
                String appendOpinion = engineData.Value<String>("AppendOpinion");

                Boolean isExitNode = engineData.Value<Boolean>("IsExitNode");
                string processDefineKey = engineData.Value<String>("ProcessDinfineKey");
                String activityDCode = engineData.Value<String>("ActivityDCode");
                ePool.ActivityID = activityDCode;
                ePool.ActivityInstanceID = activityInstanceID;
                ePool.AppDataID = resourceID;
                ePool.ApplicationName = engineData.Value<String>("AppName");
                ePool.AutoSendFeedback = engineData.Value<int>("AutoSendFeedback");
                ePool.CurrentProcessDepartmentID = engineData.Value<String>("DepID");
                ePool.EngineGuid = guid;
                ePool.EngineState = 0;
                ePool.OperatorUID = userID;
                ePool.ProcessID = processDefineKey;
                ePool.ProcessInstanceID = processInstanceID;



                ePool.UserFullPath = engineData.Value<String>("UserFullPath");
                ePool.WorkItemID = workItemID;



                JArray arrayActivity = engineData.Value<JArray>("Activities");




                XmlDocument dom = XMLHelper.CreateDomDocument("<Activities/>");
                XmlNode root = dom.DocumentElement;

                if (!isExitNode)
                {
                    for (int i = 0; i < arrayActivity.Count; i++)
                    {
                        JObject jActivity = (JObject)arrayActivity[i];
                        XmlNode ActivityInfo = XMLHelper.AppendNode(root, "ActivityInfo");
                        XmlAttribute ActivityDefineKey = XMLHelper.AppendAttr(ActivityInfo, "ActivityDefineKey");
                        ActivityDefineKey.Value = jActivity.Value<String>("SelectedNextActivity");

                        XmlNode SelectedDataNode = XMLHelper.AppendNode(ActivityInfo, "SelectedData");
                        XmlNode OUNode = XMLHelper.AppendNode(SelectedDataNode, "OUData");
                        XmlNode selectedNode = XMLHelper.AppendNode(OUNode, "NodesSelected");
                        JArray jUsers = jActivity.Value<JArray>("Users");
                        for (int j = 0; j < jUsers.Count; j++)
                        {
                            JObject jUser = (JObject)jUsers[j];
                            XmlNode userNode = XMLHelper.AppendNode(selectedNode, "object");
                            XMLHelper.AppendAttr(userNode, "OBJECTCLASS", "USERS");
                            XMLHelper.AppendAttr(userNode, "ALL_PATH_NAME", jUser.Value<String>("AllPathName"));
                            XMLHelper.AppendAttr(userNode, "DISPLAY_NAME", jUser.Value<String>("UserName"));
                            XMLHelper.AppendAttr(userNode, "OBJ_NAME", jUser.Value<String>("UserName"));
                            XMLHelper.AppendAttr(userNode, "GUID", jUser.Value<String>("UserID"));


                        }

                    }
                    ePool.SelectedNextActivityAndResourceData = dom.InnerXml;
                }
                else
                {
                    ePool.SelectedNextActivityAndResourceData = "";
                }
                MessagePreInfo wfMessage = new MessagePreInfo();
                wfMessage.EngineGuid = guid;
                String taskSubject = engineData.Value<String>("TaskSubject");
                String taskUrl = engineData.Value<String>("TaskUrl");
                wfMessage.TaskSubject = taskSubject;
                wfMessage.TaskUrl = engineData.Value<String>("TaskUrl");
                wfMessage.TaskEmergency = "0";

                XmlDocument messageDoc = XMLHelper.CreateDomDocument("<MessageData/>");
                XmlNode msRoot = messageDoc.DocumentElement;
                XMLHelper.AppendNode(msRoot, "FromUserID", userID);
                XMLHelper.AppendNode(msRoot, "ResourceID", resourceID);
                XMLHelper.AppendNode(msRoot, "Title", taskSubject);
                XMLHelper.AppendCDataNode(msRoot, "Content", "");
                XMLHelper.AppendNode(msRoot, "URL", taskUrl);
                String appName = engineData.Value<String>("AppName");
                XMLHelper.AppendNode(msRoot, "AppID", appName);
                XMLHelper.AppendNode(msRoot, "ProgramID", "");
                string device = string.Empty;

                XMLHelper.AppendNode(msRoot, "Device", device);
                wfMessage.TaskBody = messageDoc.InnerXml;
                entryData.WFMessage = wfMessage;

                JArray arrayJRelativeData = engineData.Value<JArray>("WFRelativeDatas");

                int relativeCount = arrayJRelativeData.Count;
                if (isProcessInstanceEnd || isWithdraw)
                {
                    relativeCount++;
                }

                EngineRelativeData[] arrayRelative = new EngineRelativeData[relativeCount];

                for (int k = 0; k < arrayJRelativeData.Count; k++)
                {
                    JObject jRelativedata = (JObject)arrayJRelativeData[k];
                    EngineRelativeData data = new EngineRelativeData();
                    data.EngineGuid = guid;

                    int relativeType = jRelativedata.Value<int>("RelativeType");
                    if (relativeType == (int)EnumRelativeType.ProcessInstance)
                    {
                        data.RelativeID = processInstanceID;
                        data.RelativeType = EnumRelativeType.ProcessInstance;
                    }
                    else if (relativeType == (int)EnumRelativeType.ActivityInstance)
                    {
                        data.RelativeID = activityInstanceID;
                        data.RelativeType = EnumRelativeType.ProcessInstance;
                    }

                    else if (relativeType == (int)EnumRelativeType.WorkItem)
                    {
                        data.RelativeID = workItemID;
                        data.RelativeType = EnumRelativeType.WorkItem;
                    }

                    data.VariableID = Guid.NewGuid().ToString();
                    data.VariableName = jRelativedata.Value<String>("VariableName");
                    data.VariableType = jRelativedata.Value<String>("VariableType");
                    data.VariableValue = jRelativedata.Value<String>("VariableValue");
                    arrayRelative[k] = data;
                }
                //bool ActivityInstanceEnd 强制结束活动实例的变量
                //bool ActivityInstanceBackWard 是否退回上一级的变量
                //bool ProcessInstanceEnd 强制结束流程实例的变量
                if (isWithdraw)
                {
                    EngineRelativeData data = new EngineRelativeData();

                    data.VariableID = Guid.NewGuid().ToString();
                    data.VariableName = "ActivityInstanceBackWard";
                    data.VariableValue = "True";
                    data.VariableType = "bool";
                    data.RelativeType = EnumRelativeType.ActivityInstance;
                    arrayRelative[relativeCount - 1] = data;
                }

                if (isProcessInstanceEnd)
                {
                    EngineRelativeData data = new EngineRelativeData();

                    data.VariableID = Guid.NewGuid().ToString();
                    data.VariableName = "ProcessInstanceEnd";
                    data.VariableValue = "True";
                    data.VariableType = "bool";
                    data.RelativeType = EnumRelativeType.ProcessInstance;
                    arrayRelative[relativeCount - 1] = data;
                }


                entryData.WFRelativeDataList = arrayRelative;


                IPoolProcess ipp = new PoolProcess();

                ipp.WriteWFEntry(entryData);



                //更新workitem为userdo
                if (!String.IsNullOrEmpty(workItemID))
                {
                    String updateWorkItemSql = "update WF_WorkItem set DealState = {0} where  dealState=0 and  Sort_ID=" + workItemID;
                    updateWorkItemSql = String.Format(updateWorkItemSql, (int)EnumDealState.UserDo);
                    ExecSql.RunSql("Jeedaa_Base", CommandType.Text, updateWorkItemSql);
                }

                WFLogStruct log = new WFLogStruct();

                log.ActDate = DateTime.Now;
                log.ActivityDCode = activityDCode;
                log.ActivityID = activityInstanceID;
                log.ActivityInstanceBackWard = isWithdraw;
                log.AppendOpinion = appendOpinion;

                log.OperatorUID = userID;
                log.OperatorUName = userName;
                log.Opinion = opinion;
                log.processID = processInstanceID;
                log.ProcessKey = processDefineKey;
                log.ResourceID = resourceID;
                log.ProcessInstanceEnd = isProcessInstanceEnd;
                if (activityDCode == "N1")
                {
                    log.TemplateName = "WfDefaultTemplate";
                }
                else
                {
                    log.TemplateName = "WfRadioButton";
                }
                log.Title = activityName;
                log.WorkItemID = workItemID;



                // wflog.TemplateDataXML = this.TemplateDataXML;

                WriteWFLog(log);


                if (isExitNode)
                {
                    if (!isWithdraw)
                    {
                        WFLogAdapter.DeleteObjects(resourceID);
                    }

                }


                WriteWFLogHistory(log);
                //编写审批日记

                return DisplayJson("success");

            }
            catch (Exception ex)
            {
                SysUtil.WriteLog(ex);
                return DisplayJson(ex);
            }
        }


        private void InsertWfLogHistory(WFLog log)
        {
            //插入永久历史日志
            WFLogHistory wflogHis = new WFLogHistory();
            wflogHis.RESOURCEID = log.RESOURCEID;
            wflogHis.PROCESSID = log.PROCESSID;

            wflogHis.ProcessKey = log.ProcessKey;
            wflogHis.ACTIVITYID = log.ACTIVITYID;
            wflogHis.ActivtyKey = log.ActivtyKey;
            wflogHis.ActorID = log.ActorID;
            wflogHis.ActorName = log.ActorName;
            wflogHis.ActDate = DateTime.Now;
          
            wflogHis.WorkItemID = log.WorkItemID;
            if (log.ActivtyKey == "N1")
            {
                wflogHis.TemplateID = "WfDefaultTemplate";
            }
            else
            {
                wflogHis.TemplateID = "WfRadioButton";
            }

            XmlDocument doc = XMLHelper.CreateDomDocument("< Template />");
            XmlNode rootNode = doc.DocumentElement;
            XmlNode templateNode = XMLHelper.AppendNode(rootNode, wflogHis.TemplateID);


            wflogHis.DeliverTime = log.DeliverTime;
            WFLogHistoryAdapter.InsertObject(wflogHis);
        }

        


        /// <summary>
        /// 获取工作项DataRow
        /// </summary>
        /// <param name="workItemID"></param>
        /// <returns></returns>
        private DataRow GetWorkItemRow(int workItemID)
        {
            string sql = " select Deliver_Time from wf_workItem where Sort_ID = {0} union select Deliver_Time from wf_workItemhistory  where Sort_ID ={0}";
            sql = string.Format(sql, workItemID);
            DataTable dt = ExecSql.LoadDataTable("Jeedaa_Base", sql);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            return null;


        }






}



}
