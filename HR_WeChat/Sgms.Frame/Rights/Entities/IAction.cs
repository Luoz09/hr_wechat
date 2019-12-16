using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 功能接口
    /// </summary>
    public interface IAction :IEntity
    {
        /// <summary>
        /// 功能名（key用做对应Action）
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 所属应用
        /// </summary>
        string AppID { get; set; }
    }
}
