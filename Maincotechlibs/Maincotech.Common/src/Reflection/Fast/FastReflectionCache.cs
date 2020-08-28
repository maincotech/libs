using System.Collections.Generic;

namespace MaincoTech.Reflection.Fast
{
    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _mCache = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key)
        {
            var value = default(TValue);
            if (_mCache.TryGetValue(key, out value))
            {
                return value;
            }

            lock (_mCache)
            {
                if (!_mCache.TryGetValue(key, out value))
                {
                    value = Create(key);
                    _mCache[key] = value;
                }
            }

            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract TValue Create(TKey key);
    }
}