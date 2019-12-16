using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Configuration;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.ComponentModel;

namespace Sgms.Frame.Sys
{
    /// <summary>
    /// 系统工具
    /// </summary>
    public static class SysUtil
    {
        /// <summary>
        /// 日志文件的路径
        /// </summary>
        public static readonly string LogPath = HttpContext.Current == null ? HttpContext.Current.Server.MapPath(SysPara.LogPath) : AppDomain.CurrentDomain.BaseDirectory + "/SysLog.txt";

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLog(string log)
        {
            List<string> logs = new List<string>();
            logs.Add(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]"));
            logs.Add(log);
            File.AppendAllLines(LogPath, logs);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="e"></param>
        public static void WriteLog(Exception e)
        {
            List<string> logs = new List<string>();
            logs.Add(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]"));
            logs.Add(e.Message);
            logs.Add(e.StackTrace);
            File.AppendAllLines(LogPath, logs);
        }

        /// <summary>
        /// 获取AppSettings
        /// </summary>
        /// <param name="key"></param>
        /// <returns>不存在返回null</returns>
        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 获取GetGetSection
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static object GetGetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }
        /// <summary>
        /// 获取数据库连接字符串名称
        /// </summary>
        /// <param name="dbKey"></param>
        public static string GetDBName(string dbKey)
        {
            string str=GetAppSettings("DBName");
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.ToLower().Replace("，",",").Replace(" ","").Replace("：",":");
                dbKey = dbKey.ToLower();
                string[] dbNames = str.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < dbNames.Length; i++)
                {
                    string[] names = dbNames[i].Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (names.Length > 1 && names[0] == dbKey)
                    {
                        return names[1];
                    }
                }
           
            }
            return "Jeedaa_Base";
        }


        /// <summary>
        /// 根据Description获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetValueByDescription<T>( string description) where T : struct
        {
            Type type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (field.Name == description)
                {
                    return (T)field.GetValue(null);
                }

                var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attributes != null && attributes.FirstOrDefault() != null)
                {
                    if (attributes.First().Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
        }


    }
}