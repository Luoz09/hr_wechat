using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 应用接口
    /// </summary>
    public interface IApp : IEntity
    {

        /// <summary>
        /// 应用名（key用做对应App）
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        string DisplayName { get; set; }
    }
}
