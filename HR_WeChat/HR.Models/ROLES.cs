//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ROLES
    {
        public string ID { get; set; }
        public string APP_ID { get; set; }
        public string NAME { get; set; }
        public string CODE_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string CLASSIFY { get; set; }
        public int SORT_ID { get; set; }
        public string INHERITED { get; set; }
        public string ALLOW_DELEGATE { get; set; }
        public Nullable<System.DateTime> MODIFY_TIME { get; set; }
    }
}
