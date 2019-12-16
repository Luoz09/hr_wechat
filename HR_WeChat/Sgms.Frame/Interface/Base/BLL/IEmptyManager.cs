using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Sys;

namespace Sgms.Frame.Interface.BLL
{
    public interface IEmptyManager
    {
        /// <summary>
        /// 运行的消息
        /// </summary>
        SysMessage RunMessage { get; }
    }
}
