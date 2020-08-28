using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Maincotech.Caching
{
    public class RuntimeCacheProvider : ILocker, IStaticCacheManager
    {
        private readonly MemoryCache _cacheManager = MemoryCache.Default;

        public void Flush()
        {
            List<string> cacheKeys = _cacheManager.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                _cacheManager.Remove(cacheKey);
            }
        }

        public void Clear()
        {
            Flush();
        }

        public void Dispose()
        {
            _cacheManager.Dispose();
        }

        public T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            if (cacheTime <= 0)
                return acquire();

            if (_cacheManager.Contains(key))
            {
                return (T)_cacheManager[key];
            }
            var result = acquire();
            _cacheManager.Add(key, result, DateTime.UtcNow.AddMinutes(cacheTime ?? CachingDefaults.CacheTime));
            return result;
        }

        public bool IsSet(string key)
        {
            return _cacheManager.Contains(key);
        }

        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            var cacheKeys = _cacheManager.Select(kvp => kvp.Key).Where(key => key.StartsWith(prefix)).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                _cacheManager.Remove(cacheKey);
            }
        }

        public void Set(string key, object data, int cacheTime)
        {
            _cacheManager.Add(key, data, DateTime.UtcNow.AddMinutes(cacheTime));
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            if (cacheTime <= 0)
                return await acquire();
            if (_cacheManager.Contains(key))
            {
                return (T)_cacheManager[key];
            }
            var result = await acquire();
            _cacheManager.Add(key, result, DateTime.UtcNow.AddMinutes(cacheTime ?? CachingDefaults.CacheTime));
            return result;
        }

        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            if (_cacheManager.Contains(key))
                return false;

            try
            {
                _cacheManager.Add(key, key, DateTime.UtcNow.Add(expirationTime));

                //perform action
                action();

                return true;
            }
            finally
            {
                //release lock even if action fails
                Remove(key);
            }
        }
    }
}