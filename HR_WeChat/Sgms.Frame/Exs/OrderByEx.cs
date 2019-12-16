using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sgms.Frame.Exs
{
    /// <summary>
    /// 排序扩展
    /// </summary>
    public static class OrderByEx
    {
        /// <summary>
        /// 顺序排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <param name="genericArgumentsType"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string fieldName, Type genericArgumentsType) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "OrderBy", fieldName, genericArgumentsType);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        /// <summary>
        /// 逆序排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <param name="genericArgumentsType"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string fieldName, Type genericArgumentsType) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "OrderByDescending", fieldName, genericArgumentsType);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, String fieldName, Type genericArgumentsType = null) where TEntity : class
        {
            Type type = source.GetType().GetGenericArguments()[0]; //typeof(TEntity);
            Type selectorResultType;
            LambdaExpression selector = GenerateSelector(type, fieldName, out selectorResultType, genericArgumentsType);
            MethodCallExpression resultExp = Expression.Call(
                typeof(Queryable), methodName,
                new Type[] { type, selectorResultType },
                source.Expression, Expression.Quote(selector));
            return resultExp;
        }

        private static LambdaExpression GenerateSelector(Type entityType, String propertyName, out Type resultType, Type genericArgumentsType = null)
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField). 
            var parameter = Expression.Parameter(entityType, "Entity");

            //真正的类型
            var trueType = entityType;

            Expression convertExpression;
            if (genericArgumentsType != null && genericArgumentsType != entityType)
            {
                convertExpression = Expression.Convert(parameter, genericArgumentsType);
                trueType = genericArgumentsType;
            }
            else
            {
                convertExpression = parameter;
            }

            //  create the selector part, but support child properties 
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                String[] childProperties = propertyName.Split('.');
                property = trueType.GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(convertExpression, property);
                for (int i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i]);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = trueType.GetProperty(propertyName);
                propertyAccess = Expression.MakeMemberAccess(convertExpression, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }

        //#region Private expression tree helpers
        //private static LambdaExpression GenerateSelector(Type entityType, String propertyName, out Type resultType)
        //{
        //    // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField). 
        //    var parameter = Expression.Parameter(entityType, "Entity");
        //    //  create the selector part, but support child properties 
        //    PropertyInfo property;
        //    Expression propertyAccess;
        //    if (propertyName.Contains('.'))
        //    {
        //        // support to be sorted on child fields.
        //        String[] childProperties = propertyName.Split('.');
        //        property = entityType.GetProperty(childProperties[0]);
        //        propertyAccess = Expression.MakeMemberAccess(parameter, property);
        //        for (int i = 1; i < childProperties.Length; i++)
        //        {
        //            property = property.PropertyType.GetProperty(childProperties[i]);
        //            propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
        //        }
        //    }
        //    else
        //    {
        //        property = entityType.GetProperty(propertyName);
        //        propertyAccess = Expression.MakeMemberAccess(parameter, property);
        //    }
        //    resultType = property.PropertyType;
        //    // Create the order by expression.
        //    return Expression.Lambda(propertyAccess, parameter);
        //}
        //private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, String fieldName) where TEntity : class
        //{
        //    Type type = source.GetType().GetGenericArguments()[0]; //typeof(TEntity);
        //    Type selectorResultType;
        //    LambdaExpression selector = GenerateSelector(type, fieldName, out selectorResultType);
        //    MethodCallExpression resultExp = Expression.Call(
        //        typeof(Queryable), methodName,
        //        new Type[] { type, selectorResultType },
        //        source.Expression, Expression.Quote(selector));
        //    return resultExp;
        //}
        //#endregion

        /// <summary>
        /// 顺序排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        /// <summary>
        /// 逆序排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        /// <summary>
        /// ThenBy
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName, Type genericArgumentsType = null) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "ThenBy", fieldName, genericArgumentsType);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        /// <summary>
        /// ThenByDescending
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName, Type genericArgumentsType = null) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "ThenByDescending", fieldName, genericArgumentsType);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        /// <summary>
        /// 采用表达式排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderUsingSortExpression<TEntity>(this IQueryable<TEntity> source, string sortExpression, Type genericArgumentsType) where TEntity : class
        {
            String[] orderFields = sortExpression.Split(',');
            IOrderedQueryable<TEntity> result = null;
            for (int currentFieldIndex = 0; currentFieldIndex < orderFields.Length; currentFieldIndex++)
            {
                String[] expressionPart = orderFields[currentFieldIndex].Trim().Split(' ');
                String sortField = expressionPart[0];
                Boolean sortDescending = (expressionPart.Length == 2) && (expressionPart[1].Equals("DESC", StringComparison.OrdinalIgnoreCase));
                if (sortDescending)
                {
                    result = currentFieldIndex == 0 ? source.OrderByDescending(sortField, genericArgumentsType) : result.ThenByDescending(sortField, genericArgumentsType);
                }
                else
                {
                    result = currentFieldIndex == 0 ? source.OrderBy(sortField, genericArgumentsType) : result.ThenBy(sortField, genericArgumentsType);
                }
            }
            return result;
        }
    }
}