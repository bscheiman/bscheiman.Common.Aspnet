#region
using System.Linq;
using System.Runtime.Caching;
using bscheiman.Common.Aspnet.Interfaces;
using bscheiman.Common.Util;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public static class CacheUtil {
        internal static ObjectCache Cache = MemoryCache.Default;

        public static T Get<T>(string key) {
            key = string.Format("{0}-{1}", typeof (T).Name, key);

            if (!Cache.Contains(key))
                return default(T);

            Log.Debug("Using cached entry for {0}", key);

            return (T) Cache[key];
        }

        public static void Remove<T>(string key) {
            key = key.Contains("-") ? key : string.Format("{0}-{1}", typeof (T).Name, key);

            if (!Cache.Contains(key))
                return;

            Cache.Remove(key);
            Log.Debug("Removed cache entry for {0}", key);
        }

        public static void Remove<T>(T obj) where T : ICacheObject {
            foreach (string key in obj.Keys.Where(s => !string.IsNullOrEmpty(s)).Select(k => string.Format("{0}-{1}", typeof (T).Name, k)))
                Remove<T>(key);
        }

        public static void Set<T>(T obj) where T : ICacheObject {
            foreach (string key in obj.Keys.Where(s => !string.IsNullOrEmpty(s)).Select(k => string.Format("{0}-{1}", typeof (T).Name, k))) {
                if (Cache.Contains(key))
                    Cache.Set(key, obj, null);
                else
                    Cache.Add(key, obj, null);
            }
        }
    }
}