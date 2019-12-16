using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using HR.BLL;
using HR.Models;
using HR.Wechat.Api;
using Sgms.Frame;
using Sgms.Frame.Page.MVC;
using Sgms.Frame.Sys;
using Sgms.Frame.Utils;
using System;
using System.Web.Mvc;

namespace HR.Web.Areas.Mobile.Controllers
{
    public class GateController : NormalPage
    {
        protected override AuthenticateLevel CurAuthenticateLevel
        {
            get
            {
                return AuthenticateLevel.None;
            }
        } 

        public ActionResult Login()
        {
            WechatUtil util = new WechatUtil();
            if (util.OpenID != null)
            {
                ViewBag.wechatOpenID = util.OpenID;
                USERS info = new HR_WeChatBindManager().GetUserByWechatOpenID(util.OpenID);
                if (info != null)
                {
                    String redirectURl = Server.UrlDecode(Request.Params["RedirectUrl"]);
                    if (redirectURl == null)
                        redirectURl = "~/Mobile/Home/Index";
                    SysPara.CurAdmin = info;
                    new HR_WeChatBindManager().UpdateLoginDate(util.OpenID, info.GUID);
                    Response.Redirect(redirectURl);

                }
            }

            return View(); 
        }


        public ContentResult UserLogin()
        {

            USERSManager userinfo = new USERSManager();
            var user = userinfo.GetUserByNamePwd(Request["username"], Request["password"]);
            if (user==null)
            {
                return DisplayJson(false, "用户名不存在或密码错误");
            }

            SysPara.CurAdmin = user;
             
            //开始绑定微信
            WechatUtil util = new WechatUtil();
            if (util.OpenID != null)
            {
                new HR_WeChatBindManager().BindUserWechat(util.OpenID, user.ID);
            }

            String redirectURl = Request.Params["RedirectUrl"];


            return DisplayJson(true, "登录成功");
        }


        public ActionResult Logout()
        {  
            for (int i = 0; i < this.Request.Cookies.Count; i++)
            {
                this.Response.Cookies[this.Request.Cookies[i].Name].Expires = DateTime.Now.AddDays(-1);
            }

            WechatUtil util = new WechatUtil();
            if (util.OpenID != null && SysPara.CurAdmin != null)
            {
                new HR_WeChatBindManager().UnBindUserWechat(util.OpenID, SysPara.CurAdmin.ID);
            }
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();

            return RedirectToAction("Login");
        }


        public ActionResult SendCode()
        {
            USERSManager userinfo = new USERSManager();
            var user = userinfo.GetUserByNamePwd(Request["username"], Request["password"]);
           
            
            if (user == null)
            {
                return DisplayJson(false, "用户名或密码错误");
            }

            if (String.IsNullOrWhiteSpace(user.MOBILE))
            {
                return DisplayJson(false, "无法获取该账号的手机");
            }

            var code = new Random().Next(1, 999999).ToString("000000");

            String product = "Dysmsapi";//短信API产品名称
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
            String accessKeyId = "K5yXhvWj4A0XaLyq";//你的accessKeyId
            String accessKeySecret = "9AnYaPAgAvzn7qnGflUBydg4aUq7tm";//你的accessKeySecret

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);

            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = user.MOBILE;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "吉大科技";
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = "SMS_80280341";
                //可选:模板中的变量替换JSON串,如模板内容为 "验证码${code}，您正在登录，若非本人操作，请勿泄露",此处的值为
                request.TemplateParam = "{\"code\":\"" + code + "\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = "21212121211";

                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                System.Console.WriteLine(sendSmsResponse.Message);

            }
            catch (Exception e)
            {
                return DisplayJson(false, "短信接口调用失败请重试");
            }

            return DisplayJson(true, code);
        }




        public ActionResult ChangePwd()
        {
            return View();
        }
         

    }
}
