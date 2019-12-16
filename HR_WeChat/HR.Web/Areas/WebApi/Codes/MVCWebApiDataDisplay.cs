using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Sgms.Frame.Page.MVC;
using Newtonsoft.Json.Converters;
using System.Web.Mvc;
using System.Collections;

namespace HR.Web.Areas.WebApi.Codes
{
    public class MVCWebApiDataDisplay : MVCDataDisplay
    {
        private JsonSerializerSettings _TmpCurJsonSerializerSettings;
        protected JsonSerializerSettings CurJsonSerializerSettings
        {
            get
            {
                if (_TmpCurJsonSerializerSettings == null)
                {
                    _TmpCurJsonSerializerSettings = new JsonSerializerSettings();
                    _TmpCurJsonSerializerSettings.ContractResolver = new HideAgeContractResolver(new string[] { "EntityKey", "EntityState" });
                    IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                    timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    _TmpCurJsonSerializerSettings.Converters.Add(timeFormat);

                    /*NullConvert nullConvert = new NullConvert();
                    _TmpCurJsonSerializerSettings.Converters.Add(nullConvert);*/
                }
                return _TmpCurJsonSerializerSettings;
            }
        }

        public override System.Web.Mvc.ContentResult DisplayJson(IEnumerable<object> data)
        {
            //settings.NullValueHandling = NullValueHandling.Ignore;
            return DisplayJson(JsonConvert.SerializeObject((new
            {
                rows = data,
                result = new
                {
                    success = 1,
                    recordCount = data.Count()
                }
            }), CurJsonSerializerSettings));
        }

        public override System.Web.Mvc.ContentResult DisplayJson(object obj)
        {
            return DisplayJson(JsonConvert.SerializeObject((new
            {
                entity = obj,
                result = new
                {
                    success = 1
                }
            }), CurJsonSerializerSettings));
        }

        public override System.Web.Mvc.ContentResult DisplayJson(bool isSuccess, string msg)
        {
            return DisplayJson("{\"result\":{\"success\":" + (isSuccess ? 1 : 0) + ",\"errorMsg\":\"" + msg.Replace("\n", "").Replace("\r", "").Replace("\"", "\\\"") + "\"}}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override ContentResult DisplayDataOfTotal(IEnumerable<object> data)
        {
            if (!(data is IList) && !(data is Array))
            {
                data = data.ToArray();
            }
            return DisplayJson(JsonConvert.SerializeObject(new
            {
                rows = data,
                result = new
                {
                    success = 1,
                    recordCount = data.Count()
                }
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override ContentResult DisplayDataOfTotal(IEnumerable data, int count)
        {
            return DisplayJson(JsonConvert.SerializeObject(new
            {
                rows = data,
                result = new
                {
                    success = 1,
                    recordCount = count
                }
            }));
        }

        /*public System.Web.Mvc.ContentResult DisplayDataOfTotal(IEnumerable<object> data)
        {
            throw new NotImplementedException();
        }

        public System.Web.Mvc.ContentResult DisplayDataOfTotal(IEnumerable<object> data, int count)
        {
            throw new NotImplementedException();
        }*/
    }

    public class HideAgeContractResolver : DefaultContractResolver
    {
        private IEnumerable<string> _FitlerPropertyArr;
        public HideAgeContractResolver(IEnumerable<string> fitlerPropertyArr)
        {
            _FitlerPropertyArr = fitlerPropertyArr;
        }

        protected override JsonProperty CreateProperty(MemberInfo member,
        MemberSerialization memberSerialization)
        {
            JsonProperty p = base.CreateProperty(member, memberSerialization);
            if (_FitlerPropertyArr.Contains(p.PropertyName))
            {
                //依性别决定是否要序列化
                p.ShouldSerialize = instance =>
                {
                    return false;
                };
            }
            if (p.PropertyName == "ID")
            {
                p.ShouldSerialize = m => true;
            }
            return p;
        }
    }

    /*public class NullConvert : JsonConverter
    {
        Type[] canConvertTypes = new Type[] { typeof(int?), typeof(double?), typeof(float?), typeof(decimal?), typeof(long?), typeof(short?), typeof(byte?) };
        Type stringType = typeof(string);

        Type tmpType = null;

        public override bool CanConvert(Type objectType)
        {
            tmpType = objectType;
            return objectType.IsValueType || objectType == stringType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                if (canConvertTypes.Any(m => m == tmpType))
                {
                    writer.WriteValue(0);
                }
                else
                {
                    writer.WriteValue(String.Empty);
                }
            }
            writer.WriteValue(value);
        }
    }*/

}