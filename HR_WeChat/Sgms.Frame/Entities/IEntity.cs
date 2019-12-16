using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 实体类接口
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        string ID { get; set; }
    }
}