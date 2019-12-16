using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sgms.Frame.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Sgms.Frame.Utils
{
    public abstract class ChildTableUtil
    {

        /// <summary>
        /// 填充子表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void ProcessChildTable<T>(DbContext dc, string json, string pkName, string fkName, string fkValue) where T : class, IEntity
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            var updated = jObject["updated"].ToString();
            var deleted = jObject["deleted"].ToString();
            var inserted = jObject["inserted"].ToString();
            var set = dc.Set<T>();
            Insert<T>(set, inserted, pkName, fkName, fkValue);
            Update<T>(set, updated, pkName);
            Delete<T>(dc, set, deleted, pkName);
        }

        private static void Insert<T>(DbSet set, string insertedJson, string pkName, string fkName, object fkValue) where T : class, IEntity
        {
            var insertEntities = JsonConvert.DeserializeObject<T[]>(insertedJson);

            PropertyInfo pkProperty = null;
            PropertyInfo fkProperty = null;

            foreach (var elem in insertEntities)
            {
                if (fkProperty == null)
                {
                    pkProperty = elem.GetType().GetProperty(pkName);
                    fkProperty = elem.GetType().GetProperty(fkName);
                }
                pkProperty.SetValue(elem, Guid.NewGuid().ToString(), null);
                fkProperty.SetValue(elem, Convert.ChangeType(fkValue, fkProperty.PropertyType), null);
            }
            set.AddRange(insertEntities);
        }

        private static void Delete<T>(DbContext dbc, DbSet set, string deletedJson, string pkName) where T : class, IEntity
        {
            var deletedEntities = JsonConvert.DeserializeObject<JArray>(deletedJson);
            foreach (var elem in deletedEntities)
            {
                var entity = default(T);
                entity.ID = elem[pkName].Value<string>();
                dbc.Entry<T>(entity).State = EntityState.Deleted;
                set.Attach(entity);
            }
            /*var tType = typeof(T);

            ParameterExpression pE = Expression.Parameter(tType);
            var primaryMember = tType.GetMember(pkName).First();

            MemberExpression mE = Expression.MakeMemberAccess(pE, primaryMember);
            var gType = deletedEntities.GetType().GetElementType();
            var inMethod = typeof(QueryBuilderFuncHelper).GetMethod("In", new Type[] { typeof(T2), typeof(object), typeof(object[]) });

            var conE = Expression.Constant(deletedEntities);
            var nullConE = Expression.Constant(new object[0]);
            var cE = Expression.Call(null, inMethod, mE, conE, nullConE);
            var lamda = Expression.Lambda<Func<T, bool>>(cE, pE);
            QueryBuilder<T>.Create().Where(lamda).Delete(EQueryOption.IgnoreBeginAndEnd);*/
        }

        private static void Update<T>(DbSet set, string updatedJson, string pkName) where T : class, IEntity
        {
            JArray jArray = JsonConvert.DeserializeObject<JArray>(updatedJson);
            var tType = typeof(T);
            var properties = tType.GetProperties();
            foreach (var elem in jArray)
            {
                var entity = set.Find(elem[pkName].Value<string>());
                foreach (var elem1 in properties)
                {
                    if (elem1.Name == pkName || elem[elem1.Name] == null) continue;
                    try
                    {
                        elem1.SetValue(entity, Convert.ChangeType(elem[elem1.Name], Nullable.GetUnderlyingType(elem1.PropertyType) ?? elem1.PropertyType), null);
                    }
                    catch
                    {

                    }
                }
            }

            /*JArray jArray = JsonConvert.DeserializeObject<JArray>(updatedJson);
            if (jArray.Count == 0) return;

            //获取JSON的子节点
            var children = jArray[0].Children();

            var tType = typeof(T);
            //参数表达式
            ParameterExpression pE = Expression.Parameter(tType);

            List<Expression<Func<T, object>>> lamdas = new List<Expression<Func<T, object>>>();

            var primaryMember = tType.GetMember(pkName).First();
            MemberExpression mE = Expression.MakeMemberAccess(pE, primaryMember);

            foreach (var elem in jArray)
            {
                var idE = Expression.Constant(elem[pkName].Value<T2>());
                var pkEqualsE = Expression.Equal(mE, idE);
                var pkEqualsLamad = Expression.Lambda<Func<T, bool>>(pkEqualsE, pE);

                var query = QueryBuilder<T>.Create().Where(pkEqualsLamad);

                //获取节点成员
                foreach (JProperty elem1 in children)
                {
                    if (elem1.Name == pkName) continue;
                    var memberInfo = tType.GetMember(elem1.Name).FirstOrDefault();
                    if (memberInfo == null) continue;
                    MemberExpression memberExpression = Expression.MakeMemberAccess(pE, memberInfo);
                    object value = elem[elem1.Name];
                    if (((PropertyInfo)memberInfo).PropertyType == typeof(bool) && value == null)
                    {
                        value = false;
                    }
                    var convetE = Expression.Convert(memberExpression, typeof(object));
                    var lamda = Expression.Lambda<Func<T, object>>(convetE, pE);
                    query = query.Update(lamda, value);
                }
                query.Update();
            }

            //((Newtonsoft.Json.Linq.JObject)(jArray[0])).ChildrenTokens
            //var fields = jArray[0].*/
        }
    }
}