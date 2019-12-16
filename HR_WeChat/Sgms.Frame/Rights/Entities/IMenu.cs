using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;

namespace Sgms.Frame.Rights.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMenu : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ParentID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Sort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Icon { get; set; }
    }
}
