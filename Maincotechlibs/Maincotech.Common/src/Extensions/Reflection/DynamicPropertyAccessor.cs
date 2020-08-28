using System.Linq.Expressions;

namespace System.Reflection
{
    /// <summary>
    ///
    /// </summary>
    public class DynamicPropertyAccessor
    {
        public PropertyInfo Property
        {
            get;
        }

        private Func<object, object> _mGetter;

        private DynamicExecutor _mDynamicSetter;

        public DynamicPropertyAccessor(Type type, string propertyName)
            : this(type.GetProperty(propertyName))
        { }

        public DynamicPropertyAccessor(PropertyInfo propertyInfo)
        {
            Property = propertyInfo;
        }

        private void PrepareForGet()
        {
            // target: (object)((({TargetType})instance).{Property})

            // preparing parameter, object type
            var instance = Expression.Parameter(
                typeof(object), "instance");

            // ({TargetType})instance
            Expression instanceCast = Expression.Convert(
                instance, Property.ReflectedType);

            // (({TargetType})instance).{Property}
            Expression propertyAccess = Expression.Property(
                instanceCast, Property);

            // (object)((({TargetType})instance).{Property})
            var castPropertyValue = Expression.Convert(
                propertyAccess, typeof(object));

            // Lambda expression
            var lambda =
                Expression.Lambda<Func<object, object>>(
                    castPropertyValue, instance);

            _mGetter = lambda.Compile();
        }

        private void PrepareForSet()
        {
            var setMethod = Property.GetSetMethod();

            if (setMethod != null)
            {
                _mDynamicSetter = new DynamicExecutor(setMethod);
            }
            else
            {
                throw new NotSupportedException("Cannot set the property.");
            }
        }

        public object GetValue(object o)
        {
            if (null == _mGetter)
            {
                PrepareForGet();
            }

            return _mGetter(o);
        }

        public void SetValue(object o, object value)
        {
            if (_mDynamicSetter == null)
            {
                PrepareForSet();
            }

            _mDynamicSetter.Execute(o, new[] { value });
        }
    }
}