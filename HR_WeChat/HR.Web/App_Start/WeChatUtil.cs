using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Web;
using Sgms.Frame.Utils;
using HR.BLL;
using System.Linq;

namespace HR.Wechat.Api
{


    public class WechatPushDefine
    {
        public string AppID;
        public string Secret;
        public string TemplateID;
        public string WMSInTemplateID;
        public string WMSOutTemplateID;
        public string WechatHostHeader;
        public string Token;
        public string GetAccessTokenURL;
        public string OAuth20URL;
        public string OAuth20GetAccessToken;
        public string GetJsTicketURL;

    }

    public class WechatResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    
    }


    public class AccessToken
    {
        public String access_token { get; set; }
        public int expires_in { get; set; }
        public DateTime create_time { get; set; }
    }


    public class JSTicket
    {
        public String ticket { get; set; }
        public int expires_in { get; set; }
        public DateTime create_time { get; set; }
    }
    /// <summary>
    /// 系统API请求
    /// </summary>
    public class WechatUtil
    {


        /// <summary>
        /// 获取Token的地址
        /// </summary>
        private static int TotalTokenExpiresIn = 7200;
        private static int TotalJsTicketExpiresIn = 7200;
        private WechatPushDefine define;
        private HttpRequest Request = HttpContext.Current.Request;
        private HttpResponse Response = HttpContext.Current.Response;
        private string _TmpAccessToken;



        public string GetAppSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public string OpenID
        {
            get
            {
                string userAgent = Request.UserAgent; 
                if (!userAgent.ToLower().Contains("micromessenger"))
                {

                    return "nowechat";
                }

                //if (HttpContext.Current.Session[define.AppID] == null)
                if (HttpContext.Current.Session[define.AppID] == null)
                {
                    var code = HttpContext.Current.Request.Params["code"];
                    if (code == null)
                    {
                        String webHost = GetAppSetting("Web_Host");
                        //String redirectUrl = "http://crmweixin.gx-logistics.com/Mobile/Gate/Login";
                        //String redirectUrl = webHost + HttpContext.Current.Request.RawUrl;
                        //< add key = "OAuth20URL" value = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={{AppID}}&amp;response_type=code&amp;scope=snsapi_base&amp;state=STATE&amp;redirect_uri={0}" />

                        String redirectUrl = "http://kq.nbhs.xyz/Mobile/Gate/Login";
                        //redirectUrl = HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri);

                        WebUtil.WriteLog("Code=null: "+ String.Format(define.OAuth20URL, redirectUrl));

                        HttpContext.Current.Response.Redirect(String.Format(define.OAuth20URL,redirectUrl));
                        // HttpContext.Current.Request.Url.
                      
                

                    }
                    else
                    {
                        string url = String.Format(define.OAuth20GetAccessToken, code);
                        var weChatResult = WebUtil.Post(url, String.Empty);

                        WebUtil.WriteLog("Code!=null: " + url);

                        JObject jObject = (JObject)JsonConvert.DeserializeObject(weChatResult);
                        if (jObject["errcode"] != null)
                        {
                            HttpContext.Current.Session[define.AppID] = String.Empty;
                        }
                        else
                        {
                            HttpContext.Current.Session[define.AppID] = jObject["openid"].Value<string>();
                        }
                    }
                }

                return HttpContext.Current.Session[define.AppID].ToString();
            }
        }

         

        public WechatUtil()
        {
            define = new WechatPushDefine();
            define.AppID = GetAppSetting("WXAppID");
            define.Secret = GetAppSetting("WXSecret");
            define.TemplateID = GetAppSetting("WXTemplateID");
            define.WMSInTemplateID = GetAppSetting("WXWMSInTemplateID");
            define.WMSOutTemplateID = GetAppSetting("WXWMSOutTemplateID");
            define.Token = GetAppSetting("WXToken");
            define.GetAccessTokenURL = GetAppSetting("GetAccessTokenURL");
            define.GetJsTicketURL = GetAppSetting("GetJsTicketURL");
            if (define.GetAccessTokenURL != null)
            {
                define.GetAccessTokenURL = define.GetAccessTokenURL.Replace("{{AppID}}", define.AppID);
                define.GetAccessTokenURL = define.GetAccessTokenURL.Replace("{{AppSecret}}", define.Secret);
            }
            define.OAuth20GetAccessToken = GetAppSetting("OAuth20GetAccessTokenURL");
            if (define.OAuth20GetAccessToken != null)
            {
                define.OAuth20GetAccessToken = define.OAuth20GetAccessToken.Replace("{{AppID}}", define.AppID);
                define.OAuth20GetAccessToken = define.OAuth20GetAccessToken.Replace("{{AppSecret}}", define.Secret);
            }
            define.OAuth20URL = GetAppSetting("OAuth20URL");
            if (define.OAuth20URL != null)
            {
                define.OAuth20URL = define.OAuth20URL.Replace("{{AppID}}", define.AppID);
                define.OAuth20URL = define.OAuth20URL.Replace("{{AppSecret}}", define.Secret);
            }
        }

         

        public AccessToken GetWechatAccessToken()
        {
           
            AccessToken accessToken = null;
            if (HttpRuntime.Cache["WX_AccessToken"] == null)
            {

                string result = WebUtil.Post(define.GetAccessTokenURL, string.Empty);
                 WebUtil.WriteLog(string.Format("GetAccessTokenURL:{0}",result));
                JObject jObject = (JObject)JsonConvert.DeserializeObject(result);
                String token = jObject["access_token"].Value<String>();
                int expires_in = jObject["expires_in"].Value<int>();
             
                if (token == null)
                {
                    WebUtil.WriteLog(result);                  
                    return null;
                }

                accessToken = new AccessToken();
                accessToken.access_token = token;
                accessToken.expires_in = expires_in;
                accessToken.create_time = DateTime.UtcNow;
                TotalTokenExpiresIn = expires_in;
                DateTime expiredTime = DateTime.UtcNow.AddSeconds(expires_in);

                HttpRuntime.Cache.Insert("WX_AccessToken", accessToken, null, expiredTime, System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                  accessToken = (AccessToken)HttpRuntime.Cache["WX_AccessToken"];
                  TimeSpan ts = DateTime.UtcNow - accessToken.create_time;
                  accessToken.expires_in = TotalTokenExpiresIn - (int)ts.TotalSeconds;
            }
            return accessToken;
        }


        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns>获取成功返回 AccessToken   获取失败（并写日志）返回 ""  </returns>
        public string GetAccessToken()
        {
            AccessToken accessToken = GetWechatAccessToken();
            if (accessToken == null)
            {
                return "";
            }
            return GetWechatAccessToken().access_token;
        }


        public string SendMessage(string userID, string url, params string[] value)
        { 
            string totalResult = "["; 
            string sendUrl = string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", GetAccessToken());
            var openIDs = new HR_WeChatBindManager().GetOpenIDsByUserID(userID);
            foreach(var item in openIDs)
            {
                var jsonStr = "{{\"touser\":\"{touser}\",\"template_id\":\"{template_id}\",\"url\":\"{url}\",\"topcolor\":\"#FF0000\",\"data\":{{\"first\":{{\"value\":\"{0}\",\"color\":\"#173177\"}},\"keyword1\":{{\"value\":\"{1}\",\"color\":\"#173177\"}},\"keyword2\":{{\"value\":\"{2}\",\"color\":\"#173177\"}},\"keyword3\":{{\"value\":\"{3}\",\"color\":\"#173177\"}},\"remark\":{{\"value\":\"{4}\",\"color\":\"#173177\"}}}}}}";
                jsonStr = jsonStr.Replace("{touser}", item).Replace("{template_id}", define.TemplateID).Replace("{url}", url);
                jsonStr = String.Format(jsonStr, value);
                totalResult += "\n" + WebUtil.Post(sendUrl, jsonStr) + ",";
            }
            totalResult = totalResult.TrimEnd(",".ToArray()) + "]";
            return totalResult;

            /*var tmp = CommHelper.Post(url, "{\"touser\":\"oYeCTjlNrpiuTB8BKKKNIOkOQw2s\",\"template_id\":\"" + GetTemplateID() + "\",\"url\":\"http://weixin.qq.com/download\",\"topcolor\":\"#FF0000\",\"data\":{\"first\":{\"value\":\"你有一条新的待办事项！\",\"color\":\"#173177\"},\"keyword1\":{\"value\":\"巧克力\",\"color\":\"#173177\"},\"keyword2\":{\"value\":\"39.8元\",\"color\":\"#173177\"},\"remark\":{\"value\":\"欢迎再次购买！\",\"color\":\"#173177\"}}}");*/
        }


    }
}