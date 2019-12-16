using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Sys;
using System.IO;
using System.Web;

namespace Sgms.Frame.Sys
{
    /// <summary>
    /// 系统语言
    /// </summary>
    public static class SysLang
    {
        /// <summary>
        /// 未获取用户IP
        /// </summary>
        public const string COULD_NOT_GET_IP = "CouldNotGetIP";

        /// <summary>
        /// 非法参数
        /// </summary>
        public const string ILLEGAL_PARAMETER = "IllegalParameter";

        /// <summary>
        /// 实体可能已被删除
        /// </summary>
        public const string ENTITY_DELETED = "EntityDeleted";

        /// <summary>
        /// 存在子节点，无法删除
        /// </summary>
        public const string EXIST_CHILD_NODE_CAN_NOT_DELETE = "ExistChildNodeCanNotDelete";

        /// <summary>
        /// 系统内部错误原因为：
        /// </summary>
        public const string PREFIX_SYSTEM_ERROR_REASON = "Prefix_SystemErrorReason";

        /// <summary>
        /// Action【{0}】不存在
        /// </summary>
        public const string FORMATTER_ACTION_NOT_EXIST = "Formatter_ActionNotExist";

        /// <summary>
        /// 返回列表页
        /// </summary>
        public const string GO_TO_LIST_PAGE = "GoToListPage";

        /// <summary>
        /// 返回首页
        /// </summary>
        public const string GO_BACK_DESKTOP = "GoBackDesktop";

        /// <summary>
        /// 没有权限
        /// </summary>
        public const string NO_RIGHTS = "NoRights";

        /// <summary>
        /// 顶级
        /// </summary>
        public const string TEXT_TOP = "Text_Top";

        /// <summary>
        /// 无法获取ID
        /// </summary>
        public const string COULD_NOT_GET_ID = "CouldNotGetID";

        #region 相关方法

        private static Dictionary<string, string> _TmpLangCache;
        private static Dictionary<string, string> LangCache
        {
            get
            {
                if (_TmpLangCache == null)
                {
                    _TmpLangCache = new Dictionary<string, string>();
                    var langs = File.ReadAllText(HttpContext.Current.Server.MapPath("/Lang/" + SysPara.Lang + ".txt")).Replace("\r", String.Empty).Split('\n');
                    foreach (var elem in langs)
                    {
                        var tmp = elem.Split('=');
                        if (tmp.Length == 2)
                        {
                            _TmpLangCache.Add(tmp[0], tmp[1].Replace("%3d", "="));
                        }
                    }
                }
                return _TmpLangCache;
            }
        }

        /// <summary>
        /// 获取文字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWords(string key)
        {
            if (LangCache.ContainsKey(key))
            {
                return LangCache[key];
            }
            return String.Empty;
        }

        #endregion 相关方法
    }
}