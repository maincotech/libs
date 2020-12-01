using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MaincoTech.Reflection.Fast
{
    public interface IPropertyAccessor
    {
        object GetValue(object instance);

        void SetValue(object instance, object value);
    }

    public class PropertyAccessor : IPropertyAccessor
    {
        private Func<object, object> _mGetter;
        private MethodInvoker _mSetMethodInvoker;

        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            InitializeGet(propertyInfo);
            InitializeSet(propertyInfo);
        }

        private void InitializeGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
                Expression.Convert(instance, propertyInfo.ReflectedType);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            _mGetter = lambda.Compile();
        }

        private void InitializeSet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite) return;
            _mSetMethodInvoker = new MethodInvoker(propertyInfo.GetSetMethod(true));
        }

        public object GetValue(object o)
        {
            if (_mGetter == null)
            {
                throw new NotSupportedException("Get method is not defined for this property.");
            }

            return _mGetter(o);
        }

        public void SetValue(object o, object value)
        {
            if (_mSetMethodInvoker == null)
            {
                throw new NotSupportedException("Set method is not defined for this property.");
            }

            _mSetMethodInvoker.Invoke(o, value);
        }

        #region IPropertyAccessor Members

        object IPropertyAccessor.GetValue(object instance)
        {
            return GetValue(instance);
        }

        void IPropertyAccessor.SetValue(object instance, object value)
        {
            SetValue(instance, value);
        }

        #endregion IPropertyAccessor Members
    }
}