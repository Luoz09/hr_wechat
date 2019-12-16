using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Web.Mvc; 

namespace HR.Web.Areas.Admin.Controllers
{
    public class ChangePwdController : NormalPage
    { 

        public ContentResult ChangePassWord()
        {   
            var entity = new USERSManager().GetUserByNamePwd(SysPara.CurAdmin.DisplayName, Request["oldpwd"]);
            if (entity == null)
            {
                return DisplayJson(false, "密码错误");
            }
            else {
                var newpwd = Request["newpwd"];
                entity.USER_PWD = EncryptUtil.Md5(SysPara.CurAdmin.DisplayName + newpwd); 
                return DisplayJson(new USERSManager().Update(entity),"修改密码成功");
            }
             
        }



    }
}
