using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUser : IEntity
    {

       
        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; set; }
         

        /// <summary>
        /// 中文名
        /// </summary>
        string DisplayName { get; set; }

         
        /// <summary>
        /// 当前最后登录IP
        /// </summary>
        string LoginIp { get; set; }

        /// <summary>
        /// 当前最后登录时间
        /// </summary>
        DateTime? LoginTime { get; set; }
         
         
    }
}
