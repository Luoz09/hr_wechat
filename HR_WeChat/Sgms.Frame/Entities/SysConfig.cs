using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 系统配置实体
    /// </summary>
    [XmlRoot(ElementName = "Root")]
    public class SysConfig
    {
        #region 路径配置相关

        private string _TmpTemplateName;
        /// <summary>
        /// 模板路径
        /// </summary>
        [XmlElement]
        public string TemplateName
        {
            set { _TmpTemplateName = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpTemplateName))
                {
                    _TmpTemplateName = "Default";
                }
                return _TmpTemplateName;
            }
        }

        private string _TmpLogPath;
        /// <summary>
        /// 日志路径
        /// </summary>
        [XmlElement]
        public string LogPath
        {
            set { _TmpLogPath = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpLogPath))
                {
                    _TmpLogPath = "~/Log.txt";
                }
                return _TmpLogPath;
            }
        }

        private string _TmpUploadsPath;
        /// <summary>
        /// 上传文件路径
        /// </summary>
        [XmlElement]
        public string UploadsPath
        {
            set { _TmpUploadsPath = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpUploadsPath))
                {
                    _TmpUploadsPath = "/Uploads";
                }
                return _TmpUploadsPath;
            }
        }

        private string _TmpPromptTemplatePage;
        /// <summary>
        /// 提示页面文件名
        /// </summary>
        [XmlElement]
        public string PromptTemplatePage
        {
            set { _TmpPromptTemplatePage = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpPromptTemplatePage))
                {
                    _TmpPromptTemplatePage = "ShowPrompt.htm";
                }
                return _TmpPromptTemplatePage;
            }
        }

        private string _TmpDesktopPage;
        /// <summary>
        /// 进去的欢迎页面
        /// </summary>
        [XmlElement]
        public string DesktopPage
        {
            set { _TmpDesktopPage = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpDesktopPage))
                {
                    _TmpDesktopPage = "Desktop.ashx";
                }
                return _TmpDesktopPage;
            }
        }


        private string _TmpLang;
        /// <summary>
        /// 语言
        /// </summary>
        [XmlElement]
        public string Lang
        {
            set { _TmpLang = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpLang))
                {
                    _TmpLang = "zh-cn";
                }
                return _TmpLang;
            }
        }


        private string _TmpLoginPageName;
        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public string LoginPageName
        {
            set { _TmpLoginPageName = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpLoginPageName))
                {
                    _TmpLoginPageName = "Login.ashx";
                }
                return _TmpLoginPageName;
            }
        }
        private string _TmpPCLoginPageName;
        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public string PCLoginPageName
        {
            set { _TmpPCLoginPageName = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpPCLoginPageName))
                {
                    _TmpPCLoginPageName = "/Admin/Gate/Login";
                }
                return _TmpPCLoginPageName;
            }
        }
        /// <summary>
        /// Mobile登陆页名 默认值：Login.ashx
        /// </summary>
        /// 
        private string _TmpMobileLoginPageName;
        public string MobileLoginPageName
        {
            set { _TmpMobileLoginPageName = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpMobileLoginPageName))
                {
                    _TmpMobileLoginPageName = "/Mobile/Gate/Login";
                }
                return _TmpMobileLoginPageName;
            }
        }
        private string _TmpRebackUrl;
        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public string RebackUrl
        {
            set { _TmpRebackUrl = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpRebackUrl))
                {
                    _TmpRebackUrl = "http://inner.jidait.com:8184";
                }
                return _TmpRebackUrl;
            }
        }


        private string _TmpNoRightsPageName;
        /// <summary>
        /// 登陆页名 默认值：Login.ashx
        /// </summary>
        public string NoRightsPageName
        {
            set { _TmpNoRightsPageName = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpNoRightsPageName))
                {
                    _TmpNoRightsPageName = "NoRights.ashx";
                }
                return _TmpNoRightsPageName;
            }
        }

        #endregion 路径配置相关

        private string _TmpDALAssemblyName;
        /// <summary>
        /// 数据访问层程序集名
        /// </summary>
        [XmlElement]
        public string DALAssemblyName
        {
            set { _TmpDALAssemblyName = value; }
            get
            {
                if (_TmpDALAssemblyName == null)
                {
                    _TmpDALAssemblyName = String.Empty;
                }
                return _TmpDALAssemblyName;
            }
        }

        private string _TmpBLLAssemblyName;
        /// <summary>
        /// 业务逻辑层程序集名
        /// </summary>
        [XmlElement]
        public string BLLAssemblyName
        {
            set { _TmpBLLAssemblyName = value; }
            get
            {
                if (_TmpBLLAssemblyName == null)
                {
                    _TmpBLLAssemblyName = String.Empty;
                }
                return _TmpBLLAssemblyName;
            }
        }
 
        /// <summary>
        /// 默认认证级别
        /// </summary>
        [XmlElement]
        public AuthenticateLevel DefaultAuthenticateLevel
        {
            get;
            set;
        }

        private string _TmpGUIDKey;
        /// <summary>
        /// GUIDKey
        /// </summary>
        [XmlElement]
        public string GUIDKey
        {
            set { _TmpGUIDKey = value; }
            get
            {
                if (String.IsNullOrWhiteSpace(_TmpGUIDKey))
                {
                    _TmpGUIDKey = Guid.NewGuid().ToString();
                }
                return _TmpGUIDKey;
            }
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [XmlElement]
        public string ConnectionString { get; set; }

        /// <summary>
        /// 允许操作的时间差
        /// </summary>
        [XmlElement]
        public int AllowOperaTimeDiff
        {
            set;
            get;
        }

        /// <summary>
        /// 权限数据获取类信息
        /// </summary>
        [XmlElement]
        public string RightsDataObtainClassInfo
        {
            get;
            set;
        }

        private int _TmpAllowLoginFailureTimes = -1;
        /// <summary>
        /// 允许登录失败次数
        /// </summary>
        [XmlElement]
        public int AllowLoginFailureTimes
        {
            set { _TmpAllowLoginFailureTimes = value; }
            get
            {
                if (_TmpAllowLoginFailureTimes == -1)
                {
                    _TmpAllowLoginFailureTimes = 5;
                }
                return _TmpAllowLoginFailureTimes;
            }
        }

        private int _TmpLoginFailureInterval = -1;
        /// <summary>
        /// 登录出错时间间隔单位 分钟
        /// </summary>
        [XmlElement]
        public int LoginFailureInterval
        {
            set { _TmpLoginFailureInterval = value; }
            get
            {
                if (_TmpLoginFailureInterval == -1)
                {
                    _TmpLoginFailureInterval = 120;
                }
                return _TmpLoginFailureInterval;
            }
        }
    }
}
