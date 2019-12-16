using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;

namespace Sgms.Frame.Utils
{
    /// <summary>
    /// 类型处理工具类
    /// </summary>
    public static class TypeUtil
    {
        /// <summary>
        /// dataTable 转 实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEntity<T>(DataTable dataTable)
        {
            List<T> result = new List<T>();
            var properties = typeof(T).GetProperties();
            foreach (DataRow row in dataTable.Rows)
            {
                var entity = System.Activator.CreateInstance<T>();
                foreach (var elem in properties)
                {
                    try
                    {
                        object value = row[elem.Name];
                        elem.SetValue(entity, Convert.ChangeType(value, Nullable.GetUnderlyingType(elem.PropertyType) ?? elem.PropertyType), null);
                    }
                    catch
                    {
                    }
                }
                result.Add(entity);
            }
            return result;
        }

        /// <summary>
        /// Int 转 枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T IntToEnum<T>(int value) where T : struct
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// 判断一个字符串是否为guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsGUID(string value)
        {
            if (value == null) return false;
            return Regex.IsMatch(value, "^[0-9A-Za-z]{8}-(?:[0-9A-Za-z]{4}-){3}[0-9A-Za-z]{12}$");
        }

        /// <summary>
        /// 数值转bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns>0为false 其他为true</returns>
        public static bool ToBool(double value)
        {
            return value != 0;
        }

        /// <summary>
        /// string 转 bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">defaultValue 转换失败 返回这个值</param>
        /// <returns>"true"返回 true "false" 返回 false 不区分大小写</returns>
        public static bool ToBool(string value, bool defaultValue = false)
        {
            if (value.ToLower() == "true")
            {
                return true;
            }
            else if (value.ToLower() == "false")
            {
                return false;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 字符串转byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败 返回这个值</param>
        /// <returns></returns>
        public static byte ToByte(string value, byte defaultValue = 0)
        {
            byte result = defaultValue;
            try
            {
                result = byte.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败 返回这个值</param>
        /// <returns></returns>
        public static double ToDouble(string value, double defaultValue = 0)
        {
            double result = defaultValue;
            try
            {
                result = double.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 字符串转Int
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败 返回这个值</param>
        /// <returns></returns>
        public static int ToInt(string value, int defaultValue = 0)
        {
            int result = defaultValue;
            try
            {
                result = int.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 字符串转Long
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败 返回这个值</param>
        /// <returns></returns>
        public static long ToLong(string value, long defaultValue = 0)
        {
            long result = defaultValue;
            try
            {
                result = long.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 字符串转Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Decimal ToDecimal(string value, Decimal defaultValue = 0)
        {
            Decimal result = defaultValue;
            try
            {
                result = Decimal.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Sql的日期最小值
        /// </summary>
        public static readonly DateTime SqlMinDateTime = new DateTime(1753, 1, 1);

        /// <summary>
        /// 如果转换不了 默认值为 1753-1-1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string value)
        {
            return ToDateTime(value, SqlMinDateTime);
        }

        /// <summary>
        /// 字符串转DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败 返回这个值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(string value, DateTime defaultValue)
        {
            DateTime result = defaultValue;
            try
            {
                result = DateTime.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 转换成可空的DateTime 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>转换失败返回null string为null 返回null</returns>
        public static DateTime? ToDateTimeNullable(string value)
        {
            DateTime? result = null;
            if (String.IsNullOrWhiteSpace(value)) return result;
            try
            {
                result = DateTime.Parse(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 日期转时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToTimeStamp(DateTime dateTime)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //return (int)(dateTime - startTime).TotalSeconds;
            DateTime dt1 = Convert.ToDateTime("1970-01-01 00:00:00");
            TimeSpan ts = DateTime.Now - dt1;
            return Math.Ceiling(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 同名属性拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copiedObject">被拷贝的对象</param>
        /// <returns>新的 T 类型的实例</returns>
        public static T CopyProperty<T>(object copiedObject) where T : class
        {
            var result = System.Activator.CreateInstance<T>();
            var oldObjType = copiedObject.GetType();
            var newObjType = typeof(T);
            var properties = newObjType.GetProperties();
            foreach (var elem in properties)
            {
                var oldObjProperty = oldObjType.GetProperty(elem.Name, elem.PropertyType);
                if (oldObjProperty == null) continue;
                try
                {
                    elem.SetValue(result, oldObjProperty.GetValue(copiedObject, null), null);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 同名属性拷贝
        /// </summary>
        /// <param name="copyObject">要拷贝的对象</param>
        /// <param name="copiedObject">被拷贝的对象</param>
        /// <param name="ignoreNull">是否忽略null 不忽略则连null 一起拷贝</param>
        public static void CopyProperty(object copyObject, object copiedObject, bool ignoreNull = true)
        {
            var copiedObjectType = copiedObject.GetType();
            var copyObjectType = copyObject.GetType();
            var properties = copyObjectType.GetProperties();
            foreach (var elem in properties)
            {
                var oldObjProperty = copiedObjectType.GetProperty(elem.Name, elem.PropertyType);
                if (oldObjProperty == null) continue;
                var value = oldObjProperty.GetValue(copiedObject, null);
                if (ignoreNull && value == null)
                {
                    continue;
                }
                elem.SetValue(copyObject, value, null);
            }
        }

        /// <summary>
        /// 根据classInfo创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classInfo">格式：程序集,完整类名。  格式不正确Throw  创建失败返回null</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string classInfo) where T : class
        {
            string[] tmpArr = classInfo.Split(',');
            return Assembly.Load(tmpArr[0]).CreateInstance(tmpArr[1]) as T;
        }

        /// <summary>
        /// 根据对象、属性名获取值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
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
        /// 根据对象、属性名获取值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetValue<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }
            return (T)property.GetValue(obj, null);
        }


        /*public static dynamic GetDynamicInstance(Dictionary<string,object> keyValue)
        {
            dynamic result = null;

            var classStrTemplater="namespace sgms_"

            var propertyStrTemplater = " public {0} {1}{{get;set;}}## ##";

            var typeStr = "";
            foreach (DataColumn cloumn in sender.Columns)
            {
                typeStr += "    public string @2{set;get;}## ##".Replace("@2", cloumn.ColumnName);//Replace("@1", cloumn.DataType.Name).
            }
            typeStr = "namespace @1##{##  public class @2##  {##@3##  }##}".Replace("@1", sender.Namespace).Replace("@2", sender.TableName).Replace("@3", typeStr).Replace("##", "\r\n");
            var cr = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new string[] { "System.dll" }), typeStr);
            var type = cr.CompiledAssembly.GetType(string.Format("{0}.{1}", sender.Namespace, sender.TableName));
            var properties = type.GetProperties();

            foreach (DataRow row in sender.Rows)
            {
                var dm = Activator.CreateInstance(type);
                foreach (DataColumn cloumn in sender.Columns)
                {
                    var property = properties.FirstOrDefault(l => IsEnter(l.Name, cloumn.ColumnName));
                    if (property != null)
                    {
                        property.SetValue(dm, row[cloumn].ToString());
                    }
                }
                result.Add(dm);
            }
            return result;
        }*/
    }
}
