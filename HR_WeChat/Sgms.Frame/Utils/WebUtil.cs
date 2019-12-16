using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace Sgms.Frame.Utils
{
    /// <summary>
    /// Web相关的工具
    /// </summary>
    public static class WebUtil
    {
        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string ClientIp
        {
            get
            {
                string userIP = String.Empty;

                try
                {
                    if (System.Web.HttpContext.Current == null
                || System.Web.HttpContext.Current.Request == null
                || System.Web.HttpContext.Current.Request.ServerVariables == null)
                        return "";

                    string CustomerIP = "";

                    //CDN加速后取到的IP simone 090805
                    CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                    if (!string.IsNullOrEmpty(CustomerIP))
                    {
                        return CustomerIP;
                    }

                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                    if (!String.IsNullOrEmpty(CustomerIP))
                        return CustomerIP;

                    if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                        if (CustomerIP == null)
                            CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    else
                    {
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    }

                    if (string.Compare(CustomerIP, "unknown", true) == 0)
                        return System.Web.HttpContext.Current.Request.UserHostAddress;
                    return CustomerIP;
                }
                catch { }

                return userIP;
            }
        }

        /// <summary>
        /// 保存cookies
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expires"></param>
        public static void SetCookies(string key, string value, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = expires;
            HttpContext.Current.Response.SetCookie(cookie);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCookies(string key)
        {
            var cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie == null) return String.Empty;
            return cookie.Value;
        }

        /// <summary>
        /// 设置凭证COOKIES
        /// </summary>
        /// <param name="ticket"></param>
        public static void SetTicket(string[] ticket)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, ticket);
                byte[] buffer = ms.ToArray();
                string base64 = Convert.ToBase64String(buffer);
                string cookieValue = EncryptUtil.DesEncrypt(base64);
                ms.Close();
                HttpCookie cookie = new HttpCookie("t", cookieValue);
                cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.SetCookie(cookie);
            }
        }

        /// <summary>
        /// 获取凭证COOKIES
        /// </summary>
        /// <returns></returns>
        public static string[] GetTicket()
        {
            try
            {
                var cookie = HttpContext.Current.Request.Cookies["t"];
                if (cookie == null)
                {
                    return null;
                }
                string cookieValue = cookie.Value;
                string base64 = EncryptUtil.DesDecrypt(cookieValue);
                byte[] buffer = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Position = 0;
                    BinaryFormatter formatter = new BinaryFormatter();
                    string[] ticket = (string[])formatter.Deserialize(ms);
                    return ticket;
                }
            }
            catch { return null; }
        }

        /// <summary>
        /// 设置时间间隔缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absTime">时间间隔</param>
        public static void SetAbsCache(string key, object value, DateTime absTime)
        {
            HttpContext.Current.Cache.Insert(key, value, null, absTime, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 获取缓存的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCacheValue<T>(string key)
        {
            var cacheValue = HttpContext.Current.Cache[key];
            if (cacheValue != null)
            {
                return (T)cacheValue;
            }
            return default(T);
        }


        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Post(string url, string data, Dictionary<string, string> headers = null, ICredentials credentials = null)
        {
            return Post(url, data, Encoding.UTF8, headers, credentials);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Post(string url, string data, Encoding encoding, Dictionary<string, string> headers = null, ICredentials credentials = null)
        {
            using (WebClient wc = new WebClient())
            {
                if (credentials != null)
                {
                    wc.Credentials = credentials;
                }
                wc.Headers.Add("Content-Type:application/x-www-form-urlencoded");
                if (headers != null)
                {
                    foreach (var elem in headers.Keys)
                    {
                        wc.Headers.Add(elem, headers[elem]);
                    }
                }
                byte[] postData = encoding.GetBytes(data);
                var result = String.Empty;
                try
                {
                    result = encoding.GetString(wc.UploadData(url, "POST", postData));
                }
                catch(WebException e)
                {
                    var stream = e.Response.GetResponseStream();
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    result = Encoding.UTF8.GetString(buffer);
                }
                return result;
            }
        }




        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PostJson(string url, string data, Dictionary<string, string> headers = null, ICredentials credentials = null)
        {
            return PostJson(url, data, Encoding.UTF8, headers, credentials);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PostJson(string url, string data, Encoding encoding, Dictionary<string, string> headers = null, ICredentials credentials = null)
        {
            using (WebClient wc = new WebClient())
            {
                if (credentials != null)
                {
                    wc.Credentials = credentials;
                }
                wc.Headers.Add("Content-Type:application/json");
                if (headers != null)
                {
                    foreach (var elem in headers.Keys)
                    {
                        wc.Headers.Add(elem, headers[elem]);
                    }
                }
                byte[] postData = encoding.GetBytes(data);
                var result = String.Empty;
                try
                {
                    result = encoding.GetString(wc.UploadData(url, "POST", postData));
                }
                catch (WebException e)
                {
                    var stream = e.Response.GetResponseStream();
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    result = Encoding.UTF8.GetString(buffer);
                }
                return result;
            }
        }




        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DoGet(string url)
        {
            return DoGet(url, Encoding.UTF8);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DoGet(string url, Encoding encoding)
        {
            return DoRequest(url, null, encoding, "GET");
        }

  


        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DoRequest(string url, string data, Encoding encoding, string method, string contentType = "application/x-www-form-urlencoded")
        {
            byte[] result = null;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            WebClient wc = new WebClient();
            if (method == "POST")
            {
                // wc.Headers.Add("Content-Type:application/x-www-form-urlencoded");
                wc.Headers.Add("Content-Type", contentType);
                //
                byte[] postData;
                if (String.IsNullOrEmpty(data))
                {
                    postData = new byte[] { };
                }
                else
                {
                    postData = encoding.GetBytes(data);
                }
                result = wc.UploadData(url, method, postData);
            }
            else
            {
                result = wc.DownloadData(url);
            }
            return encoding.GetString(result);
        }

        /// <summary>
        /// 把信息写入日志文件
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(string msg)
        {
            //string fileName = @"d:\log.txt"; //System.Web.HttpContext.Current.Server.MapPath("~/log.txt");
            //FileInfo fi = new FileInfo(fileName);
            //StreamWriter sw = fi.AppendText();
            ////try
            ////{
            //    sw.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff").ToString(),msg));
            ////}
            ////catch (Exception)
            ////{

            ////}
            ////finally
            ////{
            //    sw.Flush();
            //    sw.Close();
            //    sw.Dispose();

            ////}
#if(DEBUG)
           

            string directoryPath = "C:\\Logs";///HttpContext.Current.Server.MapPath(@"\Logs");
            string fileName = directoryPath + @"\log" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            StreamWriter sr = null;
            try
            {
                if (!File.Exists(fileName))
                {
                    sr = File.CreateText(fileName);
                }
                else
                {
                    sr = File.AppendText(fileName);
                }
                sr.WriteLine(DateTime.Now + ": " + msg);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
#endif

        }
    }


}