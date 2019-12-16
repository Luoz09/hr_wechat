using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Sys
{
    /// <summary>
    /// 系统常量值
    /// </summary>
    public class SysKeys
    {
        /// <summary>
        /// &lt;br /&gt;
        /// </summary>
        public const string TAG_BR = "<br />";

        /// <summary>
        /// ID
        /// </summary>
        public const string KEY_ID = "ID";

        /// <summary>
        /// IDs
        /// </summary>
        public const string KEY_IDS = "IDs";

        /// <summary>
        /// .Web
        /// </summary>
        public const string SUFFIX_WEB = ".Web";

        /// <summary>
        /// Handler
        /// </summary>
        public const string SUFFIX_WEB_PAGE_CLASSNAME = "Handler";
        
        /// <summary>
        /// Controller
        /// </summary>
        public const string SUFFIX_MVC_PAGE_CLASSNAME = "Controller";

        /// <summary>
        /// .BLL
        /// </summary>
        public const string SUFFIX_BLL = ".BLL";

        /// <summary>
        /// Manager
        /// </summary>
        public const string SUFFIX_BLL_CLASSNAME = "Manager";

        /// <summary>
        /// .DAL
        /// </summary>
        public const string SUFFIX_DAL = ".DAL";

        /// <summary>
        /// MODEL
        /// </summary>
        public const string SUFFIX_MODEL = ".Models";

        /// <summary>
        /// Service
        /// </summary>
        public const string SUFFIX_DAL_CLASSNAME = "Service";

        /// <summary>
        /// _self
        /// </summary>
        public const string TAG_A_DEFAULT_TARGET = "_self";

        /// <summary>
        /// UpUrl
        /// </summary>
        public const string KEY_UP_URL = "UpUrl";

        /// <summary>
        /// Prompt
        /// </summary>
        public const string KEY_PROMPT = "Prompt";

        /// <summary>
        /// ?action=list
        /// </summary>
        public const string URL_LIST_PAGE = "?action=list";

        /// <summary>
        /// View.htm
        /// </summary>
        public const string SUFFIX_TEMPLATE_VIEW = "View.htm";

        /// <summary>
        /// Edit.htm
        /// </summary>
        public const string SUFFIX_TEMPLATE_EDIT = "Edit.htm";

        /// <summary>
        /// Add.htm
        /// </summary>
        public const string SUFFIX_TEMPLATE_ADD = "Add.htm";

        /// <summary>
        /// List.htm
        /// </summary>
        public const string SUFFIX_TEMPLATE_LIST = "List.htm";

        /// <summary>
        /// requestType
        /// </summary>
        public const string KEY_REQUEST_TYPE = "requestType";

        /// <summary>
        /// ajax
        /// </summary>
        public const string STR_AJAX = "ajax";

        /// <summary>
        /// Action
        /// </summary>
        public const string KEY_ACTION = "Action";

        /// <summary>
        /// PageCName
        /// </summary>
        public const string KEY_PAGE_C_NAME = "PageCName";

        /// <summary>
        /// TemplatePath
        /// </summary>
        public const string KEY_TEMPLATE_PATH = "TemplatePath";

        /// <summary>
        /// pagelist
        /// </summary>
        public const string VAR_DEFAULT_ACTION = "pagelist";

        /// <summary>
        /// Templates
        /// </summary>
        public const string DIR_TEMPLATES = "Templates";

        /// <summary>
        /// application/json
        /// </summary>
        public const string MIME_JSON = "application/json";

        /// <summary>
        /// {{\"Message\":\"{0}\",\"Success\":{1}}}
        /// </summary>
        public const string FORMATTER_DISPLAY_RESULT_JSON = "{{\"Message\":\"{0}\",\"Success\":{1}}}";
    }
}
