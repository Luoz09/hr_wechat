using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using System.Reflection;
using Sgms.Frame.Sys;
using Sgms.Frame.Interface.BLL;

namespace Sgms.Frame.Page
{
    /// <summary>
    /// 
    /// </summary>
    public static class PageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IManager<T> GetManager<T>() where T : class,IEntity
        {
            var tType = typeof(T);

            String billAssemblyName = tType.Assembly.FullName.Replace(SysKeys.SUFFIX_MODEL, SysKeys.SUFFIX_BLL);


                if (!SysCache.BLLAssemblyDic.ContainsKey(billAssemblyName))
                {
                /*
                  if (SysPara.BLLAssemblyName != String.Empty)
                   {
                       SysCache.BLLAssembly = Assembly.Load(SysPara.BLLAssemblyName);
                   }
                   else
                   {
                       SysCache.BLLAssembly = Assembly.Load(tType.Assembly.FullName.Replace(SysKeys.SUFFIX_MODEL, SysKeys.SUFFIX_BLL));
                   }
                   */
                SysCache.BLLAssemblyDic.Add(billAssemblyName, Assembly.Load(billAssemblyName));
                }

            Assembly billAssembly = SysCache.BLLAssemblyDic[billAssemblyName];
            var managerType = billAssembly.GetTypes().FirstOrDefault(m => m.Name == typeof(T).Name + SysKeys.SUFFIX_BLL_CLASSNAME);
            return System.Activator.CreateInstance(managerType) as IManager<T>;
            //var result = (IManager<T>)SysCache.BLLAssembly.CreateInstance(.FullName);
            //return result;
        }
  
        public static decimal ConvertToDecimal(string data)
        {
            decimal d;
            decimal.TryParse(data, out d);
            return d;
        }

        public static string GetAppConfigValue(string nodeName)
        {
            return System.Configuration.ConfigurationManager.AppSettings[nodeName];
        }

        public static int WeightDigits
        {
            get
            {
                int i; int.TryParse(GetAppConfigValue("Weightdigits"), out i);
                return i;
            }
        }


        /// <summary>
        /// 反射方式获取属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return String.Empty;
            }
            return property.GetValue(obj, null);
        }
        /// <summary>
        /// 反射方式获取属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetDateValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return String.Empty;
            }
            DateTime d;
            if (DateTime.TryParse(property.GetValue(obj, null).ToString(),out d))
            {
                if (d == new DateTime(1900, 1, 1))
                {
                    return string.Empty;
                }
                else
                {
                    return d.ToString("yyyy-MM-dd");
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// 反射方式获取属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetDocStatus(object obj, string propertyName)
        {
    
            return String.Empty;
        }
        /// <summary>
        /// 把数字类型显示为字符，去掉多余的0
        /// </summary>
        /// <param name="obj">实体类</param>
        /// <param name="propertyName">属性名字</param>
        /// <param name="zeroShowEmpty">当数字为0时，是否显示为空字符</param>
        /// <param name="digits">小数位数，负数表示先把小数点向左移的位数，处理后再向右移动回来</param>
        /// <returns></returns>
        public static string GetDecimalToString(object obj, string propertyName, bool zeroShowEmpty, int digits)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return "";
            }
            else
            {
                string str = string.Empty;
                var v = GetValue(obj, propertyName);
                if (v != null)
                {
                    str = v.ToString();
                }
                decimal d = ConvertToDecimal(str);
                str = GetDecimalToString(str, zeroShowEmpty, digits);
                return str;
            }

        }

        /// <summary>
        /// 字符串格式化,当为0是显示为0
        /// </summary>
        /// <param name="d">字符串</param>
        /// <param name="digits">保留小数位数</param>
        /// <returns></returns>
        public static string GetDecimalToString(decimal d, int digits)
        {
            return GetDecimalToString(d, false, digits);
        }



        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="d">字符串</param>
        /// <param name="zeroShowEmpty">0是否显示为空字符串</param>
        /// <param name="digits">保留小数位数</param>
        /// <returns></returns>
        public static string GetDecimalToString(decimal d, bool zeroShowEmpty, int digits)
        {
            string data = string.Empty;
            string c = "";
            for (int i = 0; i < digits; i++)
            {
                c += "#";
            }
            if (zeroShowEmpty)
            {
                #region 当数字为0时，显示为空
                if (digits == 0)
                {
                    data = d.ToString("#");
                }
                else if (digits < 0)
                {
                    int tem = 1;
                    for (int i = 0; i < Math.Abs(digits); i++)
                    {
                        tem *= 10;
                    }
                    data = ConvertToDecimal((d / tem).ToString("0")).ToString("#");
                }
                else
                {
                    data = d.ToString("#0." + c);
                }
                #endregion
            }
            else
            {
                #region 当数字为0时，显示0
                if (digits == 0)
                {
                    data = d.ToString("0");
                }
                else if (digits < 0)
                {
                    int tem = 0;
                    for (int i = 0; i < Math.Abs(digits); i++)
                    {
                        tem *= 10;
                    }
                    data = ConvertToDecimal((d / tem).ToString("0")).ToString("0");
                }
                else
                {
                    data = d.ToString("0." + c);
                }
                #endregion
            }
            return data;
        }
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="data">字符串</param>
        /// <param name="zeroShowEmpty">0是否显示为空字符串</param>
        /// <param name="digits">保留小数位数</param>
        /// <returns></returns>
        public static string GetDecimalToString(string data, bool zeroShowEmpty, int digits)
        {
            decimal d = ConvertToDecimal(data);
            return GetDecimalToString(d, zeroShowEmpty, digits);

        }
        public static string GetDecimalToString(object obj, bool zeroShowEmpty, int digits)
        {
            string typeName = obj.GetType().Name.ToLower();
            if (typeName == "decimal" || typeName == "float" || typeName == "double")
            {
                return GetDecimalToString(obj.ToString(), zeroShowEmpty, digits);
            }
            throw new Exception("不能进行数字格式化!");
        }
        /// <summary>
        /// 把数字类型显示为字符，去掉多余的0（当数字为0时，显示为空字符，去掉小数部分）
        /// </summary>
        /// <param name="obj">实体类</param>
        /// <param name="propertyName">属性名字</param>  
        /// <returns></returns>
        public static string GetDecimalToString(object obj, string propertyName)
        {
            return GetDecimalToString(obj, propertyName, true, 0);
        }


        /// <summary>
        /// 把数字类型显示为字符，去掉多余的0（当数字为0时，显示为0，去掉小数部分）
        /// </summary>
        /// <param name="obj">实体类</param>
        /// <param name="propertyName">属性名字</param>  
        /// <param name="digits">小数位数，负数表示先把小数点向左移的位数，处理后再向右移动回来</param>
        /// <returns></returns>
        public static string GetDecimalToString(object obj, string propertyName, int digits)
        {
            return GetDecimalToString(obj, propertyName, false, digits);
        }
        /// <summary>
        /// 把数字类型显示为字符，去掉多余的0
        /// </summary>
        /// <param name="obj">实体类</param>
        /// <param name="propertyName">属性名字</param>
        /// <param name="zeroShowEmpty">当数字为0时，是否显示为空字符</param>
        /// <returns></returns>
        public static string GetDecimalToString(object obj, string propertyName, bool zeroShowEmpty)
        {
            return GetDecimalToString(obj, propertyName, zeroShowEmpty, 0);
        }

        /// <summary>
        /// 反射方式获取属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static string GetDateTimeValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return String.Empty;
            }
            DateTime date;
            var d = property.GetValue(obj, null);
            if (d == null)
            {
                return String.Empty;
            }
            else
            {
                if (!DateTime.TryParse(d.ToString(), out date))
                {
                    return String.Empty;
                }
                else
                {
                    return date.ToString("yyyy-MM-dd");
                }
            }
        }
    }
}
