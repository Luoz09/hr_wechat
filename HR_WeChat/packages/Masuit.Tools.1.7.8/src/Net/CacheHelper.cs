﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Masuit.Tools.Net
{
    /// <summary>
    /// 全局统一的缓存类
    /// </summary>
    public static class CacheHelper
    {
        #region  获取数据缓存

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="CacheKey">键</param>
        public static T GetCache<T>(this Cache cache, string CacheKey)
        {
            return (T)cache[CacheKey];
        }
        #endregion

        #region  设置数据缓存

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="CacheKey">键</param>
        /// <param name="objObject">值</param>
        public static void SetCache(this Cache cache, string CacheKey, object objObject)
        {
            cache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="cacheKey">键</param>
        /// <param name="objObject">值</param>
        /// <param name="Timeout">过期时间</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheKey"/>"/> is <c>null</c>.</exception>
        public static void SetCache(this Cache cache, string cacheKey, object objObject, TimeSpan Timeout)
        {
            if (cacheKey == null) throw new ArgumentNullException(nameof(cacheKey));
            cache.Insert(cacheKey, objObject, null, DateTime.MaxValue, Timeout, CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="CacheKey">键</param>
        /// <param name="objObject">值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        /// <param name="slidingExpiration">滑动过期时间</param>
        public static void SetCache(this Cache cache, string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            cache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }
        #endregion

        #region   移除缓存

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="CacheKey">键</param>
        public static void RemoveAllCache(this Cache cache, string CacheKey) => cache.Remove(CacheKey);

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache(this Cache cache)
        {
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                cache.Remove(CacheEnum.Key.ToString());
            }
        }
        #endregion

        private static SortedDictionary<string, object> dic = new SortedDictionary<string, object>();
        private static volatile Cache instance = null;
        private static object lockHelper = new object();

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Add(string key, object value)
        {
            dic.Add(key, value);
        }

        /// <summary>
        /// 缓存实例
        /// </summary>
        public static Cache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new Cache();
                        }
                    }
                }
                return instance;
            }
        }
    }
}