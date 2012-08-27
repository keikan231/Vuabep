using System;
using System.Web;
using System.Web.Caching;
using Microsoft.Practices.Unity;

namespace CRS.Common.Caching
{
    /// <summary>
    /// Performs caching operations
    /// </summary>
    public static class Cacher
    {
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets inner cacher
        /// </summary>
        private static Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }

        /// <summary>
        /// Gets cached item. Inserts new one first if not exist in cache
        /// </summary>
        public static T GetData<T>(string key) where T : ICacheItem
        {
            object item = Cache.Get(key);

            // Insert new item to cache if there's no one exist
            if (item == null)
            {
                lock (_lockObject) // Doubled-check locking
                {
                    item = Cache.Get(key);
                    if (item == null)
                    {
                        ICacheItem cacheItem = IoC.UnityContainer.Resolve<T>();
                        cacheItem.Execute();
                        if (cacheItem.IsPopulated) // Add to cache if the item has been executed successfully
                            AddToCache(key, cacheItem);
                        item = cacheItem;
                    }
                }
            }

            return (T)item;
        }

        /// <summary>
        /// Gets cached item. Returns null if not exist
        /// </summary>
        public static object GetData(string key)
        {
            return Cache[key];
        }

        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Adds an item to cache with absolute time expiration
        /// </summary>
        public static void AddToCache(string key, object value, CacheItemPriority cacheItemPriority, DateTime absoluteExpiration)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            lock (_lockObject)
            {
                Cache.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration, cacheItemPriority, null);
            }
        }

        /// <summary>
        /// Adds an item to cache with sliding time expiration
        /// </summary>
        public static void AddToCache(string key, object value, CacheItemPriority cacheItemPriority, TimeSpan slidingExpiration)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            lock (_lockObject)
            {
                Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration, cacheItemPriority, null);
            }
        }

        /// <summary>
        /// Adds an item to cache with absolute time expiration whose value is read from config file
        /// </summary>
        public static void AddToCache(string key, object value)
        {
            AddToCache(key, value, CacheItemPriority.Normal, CacheHelper.GetAbsoluteExpirationFromConfig(key));
        }
    }
}