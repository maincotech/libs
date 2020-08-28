using System.Collections.Generic;

namespace System.Reflection
{
    public class DynamicPropertyAccessorCache
    {
        private readonly object _mMutex = new object();

        private readonly Dictionary<Type, Dictionary<string, DynamicPropertyAccessor>> _mCache =
            new Dictionary<Type, Dictionary<string, DynamicPropertyAccessor>>();

        public DynamicPropertyAccessor GetAccessor(Type type, string propertyName)
        {
            DynamicPropertyAccessor accessor;
            Dictionary<string, DynamicPropertyAccessor> typeCache;

            if (_mCache.TryGetValue(type, out typeCache))
            {
                if (typeCache.TryGetValue(propertyName, out accessor))
                {
                    return accessor;
                }
            }

            lock (_mMutex)
            {
                if (!_mCache.ContainsKey(type))
                {
                    _mCache[type] = new Dictionary<string, DynamicPropertyAccessor>();
                }

                accessor = new DynamicPropertyAccessor(type, propertyName);
                _mCache[type][propertyName] = accessor;

                return accessor;
            }
        }
    }
}