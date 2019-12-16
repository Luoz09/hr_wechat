
using Sgms.Frame.Entities;
using Sgms.Frame.Rights.Entities;
using System;

namespace HR.Models
{
     
    public partial class HR_ApplyInfo : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.ApplyID;
            }
            set
            {
                _ID = value;
            }
        }
    }

    public partial class HR_Check : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.CheckID;
            }
            set
            {
                _ID = value;
            }
        }
    }
     

    public partial class ORGANIZATIONS : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.GUID;
            }
            set
            {
                _ID = value;
            }
        }
    }


 
    public partial class MATERIAL : IEntity
    { 

    }

    public partial class OU_USERS : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.USER_GUID;
            }
            set
            {
                _ID = value;
            }
        }
         

    }

    public partial class USERS : IEntity ,IUser
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.GUID;
            }
            set
            {
                _ID = value;
            }
        }

        public string DepName
        {
            get;
            set; 
        }

        public string UserName
        {
            get
            {
                return this.LOGON_NAME;
            }
            set
            {
                LOGON_NAME = value;
            }
        }

          

        public string DisplayName
        {
            get
            {
                return this.LOGON_NAME;
            }
            set
            {
                LOGON_NAME = value;
            }
        }
         
        public string LoginIp
        {
            get;
            set;
        }

        public DateTime? LoginTime
        {
            get;
            set;
        }

    }


    public partial class GROUPS : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.GUID;
            }
            set
            {
                _ID = value;
            }
        } 
    }

    public partial class GROUP_USERS : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.USER_GUID;
            }
            set
            {
                _ID = value;
            }
        }
    }


    public partial class HR_WeChatBind : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.GUID;
            }
            set
            {
                _ID = value;
            }
        }
    }

    public partial class ROLES : IEntity
    {
          
    }

    public partial class APPLICATIONS : IEntity
    {

    }

    public partial class EXPRESSIONS : IEntity
    { 
        public string ParentID { get; set; }

    }

    public partial class FUNCTIONS : IEntity
    { 

    }

    public partial class ROLE_TO_FUNCTIONS : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.ROLE_ID;
            }
            set
            {
                _ID = value;
            }
        }

    }


    public partial class TASK_LIST_VIEW : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return this.SORT_ID.ToString();
            }
            set
            {
                _ID = value;
            }
        }


    }

    public partial class SYS_DICTIONARY : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return ItemID;
            }
            set
            {
                _ID = value;
            }
        }
    }


    public partial class KQ_DayoffType : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return DayoffTypeSysID;
            }
            set
            {
                _ID = value;
            }
        }
    }

    public partial class KQ_OtType : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return OtTypeSysID;
            }
            set
            {
                _ID = value;
            }
        }
    }

    public partial class KQ_KqData : IEntity
    {
        private string _ID;
        public string ID
        {
            get
            {
                return GUID;
            }
            set
            {
                _ID = value;
            }
        }
    }  
}