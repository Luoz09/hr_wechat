using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sgms.Frame.EDM.DAL;
using Sgms.Frame.Interface.DAL;

namespace Sgms.Frame.EDM.DAL
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EDMChildTableProcess<T> : IChildTableProcess<T> where T : class,IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="foreignKeyField"></param>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        public List<IEnumerable<T>> GetChanges(string json, string foreignKeyField, string foreignKey)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            var inserted = jObject["inserted"] as JArray;
            var updated = jObject["updated"] as JArray;
            var deleted = jObject["deleted"] as JArray;
            var result = new List<IEnumerable<T>>();
            result.Add(JArrayToEntities(inserted, foreignKeyField, foreignKey));
            result.Add(JArrayToEntities(updated, foreignKeyField, foreignKey));
            result.Add(JArrayToEntities(deleted));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="data"></param>
        public void SaveToCache(IOperationService<T> service,List<IEnumerable<T>> data)
        {
            var edmService = service as EDMOperationService<T>;
            edmService.InsertToCache(data[0]);
            edmService.ModifyToCache(data[1]);
            edmService.DeleteToCache(data[2].Select(m => m.ID));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="json"></param>
        /// <param name="foreignKeyField"></param>
        /// <param name="foreignKey"></param>
        public void SaveToCache(IOperationService<T> service, string json, string foreignKeyField, string foreignKey)
        {
            var edmService = service as EDMOperationService<T>;
            var data = GetChanges(json, foreignKeyField, foreignKey);
            edmService.InsertToCache(data[0]);
            edmService.ModifyToCache(data[1]);
            edmService.DeleteToCache(data[2].Select(m=>m.ID));
        }

        private List<T> JArrayToEntities(JArray jArray, string foreignKeyField = null, string foreignKey = null)
        {
            List<T> result = new List<T>();
            var tType = typeof(T);
            var properties = tType.GetProperties();

            foreach (var elem in jArray)
            {
                if (elem.HasValues)
                {
                    var entity = System.Activator.CreateInstance<T>();
                    foreach (var elem1 in properties)
                    {
                        if (elem[elem1.Name] == null) continue;
                        try
                        {
                            elem1.SetValue(entity, Convert.ChangeType(elem[elem1.Name], Nullable.GetUnderlyingType(elem1.PropertyType) ?? elem1.PropertyType), null);
                        }
                        catch
                        {
                        }
                    }
                    if (elem["ID"] != null)
                    {
                        entity.ID = elem["ID"].Value<string>();
                    }
                    else
                    {
                        entity.ID = Guid.NewGuid().ToString();
                    }
                    if (foreignKeyField != null)
                    {
                        var property = tType.GetProperty(foreignKeyField);
                        property.SetValue(entity, foreignKey, null);
                    }
                    result.Add(entity);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="json"></param>
        /// <param name="foreignKeyField"></param>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        public bool Save(IOperationService<T> service, string json, string foreignKeyField, string foreignKey)
        {
            SaveToCache(service, json, foreignKeyField, foreignKey);
            var edmService = service as EDMOperationService<T>;
            edmService.SubmitToDb();
            return !edmService.RunMessage.HasMessage;
        }
    }
}