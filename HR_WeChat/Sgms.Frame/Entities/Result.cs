using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sgms.Frame.Entities
{
    /// <summary>
    /// 结果
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string Message { get; set; }
    }
}
