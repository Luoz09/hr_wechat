using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Sgms.Frame.Sys;
using System.Collections;

namespace Sgms.Frame.Interface.DAL
{
    public interface IEmptyService
    {
        /// <summary>
        /// 执行中的系统消息
        /// </summary>
        SysMessage RunMessage { get; }
    }
}