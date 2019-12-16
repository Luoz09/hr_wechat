using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sgms.Frame.Exs;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sgms.Frame.Entities;
using Sgms.Frame.DAL;
using Sgms.Frame.Interface;

namespace Sgms.Frame.EDM.DAL
{
    /// <summary>
    /// 
    /// </summary>
    public class EDMFilter : IDataFilter
    {
        private int _TmpLastCount = 0;
        public int LastCount
        {
            get { return _TmpLastCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="filterInfo"></param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(IQueryable<T> srcData, DataFilterInfo filterInfo) where T : class
        {
            return Filter(srcData, filterInfo.SearchStr, filterInfo.AdvancedSearchJson, filterInfo.SortField, filterInfo.IsDesc, filterInfo.PageSize, filterInfo.CurPage, filterInfo.AdvancedUseOrSearch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="searchStr"></param>
        /// <param name="sortField"></param>
        /// <param name="isDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(IQueryable<T> srcData, string searchStr, string sortField, bool isDesc, int pageSize, int curPage) where T : class
        {
            return Paging(OrderBy(Search(srcData, searchStr), sortField, isDesc), pageSize, curPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="advancedSearchJson"></param>
        /// <param name="sortField"></param>
        /// <param name="isDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="advancedUseOrSearch"></param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(IQueryable<T> srcData, string advancedSearchJson, string sortField, bool isDesc, int pageSize, int curPage, bool advancedUseOrSearch = false) where T : class
        {
            return Paging(OrderBy(AdvancedSearch(srcData, advancedSearchJson, advancedUseOrSearch), sortField, isDesc), pageSize, curPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="searchStr"></param>
        /// <param name="advancedSearchJson"></param>
        /// <param name="sortField"></param>
        /// <param name="isDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="advancedUseOrSearch"></param>
        /// <returns></returns>
        public IQueryable<T> Filter<T>(IQueryable<T> srcData, string searchStr, string advancedSearchJson, string sortField, bool isDesc, int pageSize, int curPage, bool advancedUseOrSearch = false) where T : class
        {
            var result = srcData;
            if (!String.IsNullOrEmpty(searchStr))
            {
                result = Filter(result, searchStr, sortField, isDesc, pageSize, curPage);
            }
            else if (!String.IsNullOrEmpty(advancedSearchJson))
            {
                result = Filter(result, advancedSearchJson, sortField, isDesc, pageSize, curPage, advancedUseOrSearch);
            }
            else
            {
                _TmpLastCount = srcData.Count();
                result = Paging(OrderBy(result, sortField, isDesc), pageSize, curPage);
            }
            return result;
        }

        /// <summary>
        /// 普通查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        public IQueryable<T> Search<T>(IQueryable<T> srcData, string searchStr) where T : class
        {
            Type tType = null;
            var genericType = typeof(T);
            if (genericType == typeof(object))
            {
                var first = srcData.FirstOrDefault();
                if (first == null)
                    return srcData;
                tType = first.GetType();
            }
            var result = srcData.Where(GetSearchExpression<T>(searchStr, tType));
            _TmpLastCount = result.Count();
            return result;
        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="advancedSearchJson"></param>
        /// <param name="useOrSearch"></param>
        /// <returns></returns>
        public IQueryable<T> AdvancedSearch<T>(IQueryable<T> srcData, string advancedSearchJson, bool useOrSearch = false) where T : class
        {
            Type tType = null;
            var genericType = typeof(T);
            if (genericType == typeof(object))
            {
                var first = srcData.FirstOrDefault();
                if (first == null)
                    return srcData;
                tType = first.GetType();
            }
            var result = srcData.Where(GetAdvancedSearchExpression<T>(advancedSearchJson, tType, useOrSearch));
            _TmpLastCount = result.Count();
            return result;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="field"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        public IQueryable<T> OrderBy<T>(IQueryable<T> srcData, string field, bool isDesc) where T : class
        {
            var genericArgumentType = srcData.GetType().GetGenericArguments()[0];
            var result = srcData;
            if (!String.IsNullOrWhiteSpace(field))
            {
            }
            else
            {
                var propertyInfo = genericArgumentType.GetProperties().LastOrDefault();
                if (propertyInfo != null)
                {
                    field = propertyInfo.Name;
                }
            }
            if (field.Contains(","))
            {
                result = result.OrderUsingSortExpression(field, genericArgumentType);
            }
            else
            {
                if (isDesc)
                {
                    result = result.OrderByDescending(field, genericArgumentType);
                }
                else
                {
                    result = result.OrderBy(field, genericArgumentType);
                }
            }
            return result;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcData"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public IQueryable<T> Paging<T>(IQueryable<T> srcData, int pageSize, int curPage) where T : class
        {
            return srcData.Skip(pageSize * (curPage - 1)).Take(pageSize);
        }

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchStr">查询的内容</param>
        /// <param name="tType"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetSearchExpression<T>(string searchStr, Type tType)
        {
            /*//泛型类型
            var genericType = dataSource.GetType().GetGenericArguments()[0];*/
            var genericType = tType ?? typeof(T);
            //参数表达式
            var parameter = Expression.Parameter(typeof(object), "m");

            Expression body = null;
            var str = searchStr.Split(',');
            if (str.Length > 1)
            {
                for (var i = 0; i < str.Length / 4; i++)
                {
                    Type stringType = typeof(string);

                    Expression memberExpression = null;
                    var changeTypeExpression = Expression.Convert(parameter, genericType);
                    var elem = genericType.GetProperties().Where(m => m.Name == str[1 + i * 4]).ToArray();
                    memberExpression = Expression.MakeMemberAccess(changeTypeExpression, elem[0]);
                    var likeOfMethod = stringType.GetMethod(str[2 + i * 4], new Type[] { stringType });
                    var likeOfExpression = Expression.Constant(str[3 + i * 4], typeof(string));
                    if (elem[0].PropertyType != typeof(string))
                    {
                        var toStringMethod = typeof(object).GetMethod("ToString");
                        memberExpression = Expression.Call(memberExpression, toStringMethod); 
                       
                    }
                    var likeCallExpression = Expression.Call(memberExpression, likeOfMethod, likeOfExpression);



                    if (body == null)
                    {
                        body = likeCallExpression;// greaterThanOrEqualExpression;
                    }
                    else
                    {
                        if (str[0 + i * 4] == "And")
                        {
                            body = Expression.AndAlso(body, likeCallExpression);
                        }
                        else {
                            body = Expression.OrElse(body, likeCallExpression);
                        }
                    }
                }
            }
            else
            {
                var likeExpression = Expression.Constant("%" + searchStr + "%", typeof(string));

                //string类型
                Type stringType = typeof(string);

                var genericProperties = genericType.GetProperties().Where(m => m.Name != "ID");
                    foreach (var elem in genericProperties)
                    {
                        var propertyType = elem.PropertyType;
                        Expression memberExpression = null;
                        if (propertyType != stringType)
                            continue;
                        //要搜索的字段
                        var changeTypeExpression = Expression.Convert(parameter, genericType);
                        memberExpression = Expression.MakeMemberAccess(changeTypeExpression, elem);
                        var likeOfMethod = stringType.GetMethod("Contains", new Type[] { stringType });
                        var likeOfExpression = Expression.Constant(searchStr, typeof(string));
                        //System.Data.Objects.EntityFunctions
                        var likeCallExpression = Expression.Call(memberExpression, likeOfMethod, likeOfExpression);

                        //var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(likeCallExpression, Expression.Constant(0, typeof(int)));
                        if (body == null)
                        {
                            body = likeCallExpression;// greaterThanOrEqualExpression;
                        }
                        else
                        {
                            body = Expression.OrElse(body, likeCallExpression);
                        }
                    }
             }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchJson"></param>
        /// <param name="tType"></param>
        /// <param name="useOrSearch"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetAdvancedSearchExpression<T>(string searchJson, Type tType, bool useOrSearch = false) where T : class
        {
            /*
            [
                {
                    'Field':'Title',
                    'IsRange':false,
                    'SearchStr':'我'
                },
                {
                    'Field':'Opera',
                    'IsRange':false,
                    'SearchStr':'起'
                },
                {
                    'Field':'AddTime',
                    'IsRange':true,
                    'MinValue':'2012-1-1',
                    'MaxValue':'2016-4-3'
                }
            ] 
            */

            JArray jArray = (JArray)JsonConvert.DeserializeObject(searchJson);

            var stringType = typeof(string);

            //泛型类型
            var genericType = tType ?? typeof(T);

            //参数表达式
            var paramExpression = Expression.Parameter(typeof(object), "m");
            Expression body = null;
            foreach (var elem in jArray)
            {
                if (elem["Field"] == null)
                {
                    continue;
                }
                var propertyInfo = genericType.GetProperty(elem["Field"].Value<string>());
                if (propertyInfo == null)
                    continue;
                var srcType = propertyInfo.PropertyType;
                var propertyType = Nullable.GetUnderlyingType(srcType) ?? srcType;

                #region 范围搜索

                if (elem["IsRange"] != null && elem["IsRange"].Value<bool>())//范围搜索
                {
                    if (propertyType.IsValueType)
                    {
                        object maxValue = null, minValue = null;

                        //设置最小值
                        if (elem["MinValue"] != null)
                        {
                            try
                            {
                                minValue = Convert.ChangeType(elem["MinValue"].Value<object>().ToString(), propertyType);
                            }
                            catch
                            {
                                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                                {
                                    minValue = new DateTime(1753, 1, 1);
                                }
                                else
                                {
                                    var minValueField = propertyType.GetField("MinValue");
                                    if (minValueField == null) continue;
                                    minValue = minValueField.GetValue(null);
                                }
                            }
                        }
                        else
                        {
                            var minValueField = propertyType.GetField("MinValue");
                            if (minValueField == null) continue;
                            minValue = minValueField.GetValue(null);
                        }

                        //设置最大值
                        if (elem["MaxValue"] != null)
                        {
                            try
                            {
                                maxValue = Convert.ChangeType(elem["MaxValue"].Value<object>().ToString(), propertyType);
                                if (propertyType == typeof(DateTime))
                                {
                                    var dt = (DateTime)maxValue;
                                    if (dt.ToString("HHmmss") == "000000")
                                    {
                                        maxValue = dt.AddDays(1);
                                    }
                                }
                            }
                            catch
                            {
                                var maxValueField = propertyType.GetField("MaxValue");
                                if (maxValueField == null) continue;
                                maxValue = maxValueField.GetValue(null);
                            }
                        }
                        else
                        {
                            var maxValueField = propertyType.GetField("MaxValue");
                            if (maxValueField == null) continue;
                            maxValue = maxValueField.GetValue(null);
                        }
                        //最小值表达式
                        var minExpression = Expression.Constant(minValue);
                        //最大值表达式
                        var maxExpression = Expression.Constant(maxValue);

                        var convertExpression = Expression.Convert(paramExpression, genericType);

                        //字段信息
                        Expression memberExpression = Expression.MakeMemberAccess(convertExpression, propertyInfo);

                        Expression resultExpression;

                        if (srcType != propertyType)
                        {
                            var notNullExpression = Expression.Not(Expression.Equal(memberExpression, Expression.Constant(null, typeof(object))));

                            convertExpression = Expression.Convert(memberExpression, propertyType);

                            //小于
                            var lessExpression = Expression.LessThan(minExpression, convertExpression);
                            //大于
                            var greaterExpression = Expression.GreaterThan(maxExpression, convertExpression);
                            resultExpression = Expression.AndAlso(notNullExpression, lessExpression);
                            resultExpression = Expression.AndAlso(resultExpression, greaterExpression);
                        }
                        else
                        {
                            //小于
                            var lessExpression = Expression.LessThan(minExpression, memberExpression);
                            //大于
                            var greaterExpression = Expression.GreaterThan(maxExpression, memberExpression);

                            resultExpression = Expression.AndAlso(lessExpression, greaterExpression);
                        }



                        if (body == null)
                        {
                            body = resultExpression;
                        }
                        else
                        {
                            if (useOrSearch)
                            {
                                body = Expression.OrElse(body, resultExpression);
                            }
                            else
                            {
                                body = Expression.AndAlso(body, resultExpression);
                            }
                        }
                    }
                }
                #endregion 范围搜索

                #region 一个字段多个值
                else if (elem["MoreValue"]!=null && elem["MoreValue"].Value<bool>())
                {
                    var searchList = elem["SearchStr"].ToString().Split(',');
                    var convertExpression = Expression.Convert(paramExpression, genericType);
                    var memberExpression = Expression.MakeMemberAccess(convertExpression, propertyInfo);
                    var likeMethod = stringType.GetMethod("Contains", new Type[] { stringType });
                    if (elem["IncludeEqual"] != null)
                    {
                        likeMethod = stringType.GetMethod(elem["IncludeEqual"].ToString(), new Type[] { stringType });
                    }  
                    foreach (var item in searchList)
                    {
                        var likeOfExpression = Expression.Constant(item.ToString(), typeof(string));
                        if (propertyType == stringType)
                        {
                            var likeCallExpression = Expression.Call(memberExpression, likeMethod, likeOfExpression);
                            //var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(likeCallExpression, Expression.Constant(0, typeof(int)));
                            if (body == null)
                            {
                                body = likeCallExpression;// greaterThanOrEqualExpression;
                            }
                            else
                            {
                                body = Expression.OrElse(body, likeCallExpression);
                            }
                        }
                        else
                        {
                            var toStringMethod = typeof(object).GetMethod("ToString");
                            var stringEmptyExpression = Expression.Constant(String.Empty, stringType);

                            var toStringExpression = Expression.Call(memberExpression, toStringMethod);
                            var indexOfCallExpression = Expression.Call(toStringExpression, likeMethod, likeOfExpression);
                            var greaterThanOrEqualExpression = indexOfCallExpression;
                            if (body == null)
                            {
                                body = greaterThanOrEqualExpression;
                            }
                            else
                            {
                                if (useOrSearch || elem["OrAnd"].ToString() == "Or")
                                {
                                    body = Expression.OrElse(body, greaterThanOrEqualExpression);
                                }
                                else
                                {
                                    body = Expression.AndAlso(body, greaterThanOrEqualExpression);
                                }
                            }
                        }
                    } 
                }
                #endregion
                else
                {
                    var convertExpression = Expression.Convert(paramExpression, genericType);
                    var memberExpression = Expression.MakeMemberAccess(convertExpression, propertyInfo);
                    var likeMethod = stringType.GetMethod("Contains", new Type[] { stringType });
                    if (elem["IncludeEqual"] != null)
                    {
                        likeMethod = stringType.GetMethod(elem["IncludeEqual"].ToString(), new Type[] { stringType });
                    }
                    var likeOfExpression = Expression.Constant(elem["SearchStr"].Value<string>(), typeof(string));
                    if (propertyType == stringType)
                    {
                        var likeCallExpression = Expression.Call(memberExpression, likeMethod, likeOfExpression);
                        //var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(likeCallExpression, Expression.Constant(0, typeof(int)));
                        if (body == null)
                        {
                            body = likeCallExpression;// greaterThanOrEqualExpression;
                        }
                        else
                        {
                            if (useOrSearch || elem["OrAnd"].ToString() == "Or")
                            {
                                body = Expression.OrElse(body, likeCallExpression);
                            }
                            else
                            {
                                body = Expression.AndAlso(body, likeCallExpression);
                            }
                        }
                    }
                    else
                    {
                        var toStringMethod = typeof(object).GetMethod("ToString");
                        var stringEmptyExpression = Expression.Constant(String.Empty, stringType);
                        //var stringConcatMethod = stringType.GetMethod("Concat", new Type[] { typeof(object[]) });
                        //var arrayExpress= Expression.NewArrayInit(typeof(object),memberExpression,stringEmptyExpression);
                        //var stringConcatExpression = Expression.Call(stringConcatMethod, arrayExpress);

                        var toStringExpression = Expression.Call(memberExpression, toStringMethod);
                        var indexOfCallExpression = Expression.Call(toStringExpression, likeMethod, likeOfExpression);
                        //var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(indexOfCallExpression, Expression.Constant(0, typeof(int)));
                        var greaterThanOrEqualExpression = indexOfCallExpression;
                        if (body == null)
                        {
                            body = greaterThanOrEqualExpression;
                        }
                        else
                        {
                            if (useOrSearch || elem["OrAnd"].ToString() == "Or")
                            {
                                body = Expression.OrElse(body, greaterThanOrEqualExpression);
                            }
                            else
                            {
                                body = Expression.AndAlso(body, greaterThanOrEqualExpression);
                            }
                        }
                    }
                }
            }
            if (body == null)
                return m => true;
            return Expression.Lambda<Func<T, bool>>(body, paramExpression);
        }
    }
}
