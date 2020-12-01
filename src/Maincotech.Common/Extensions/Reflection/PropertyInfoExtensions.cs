using System.ComponentModel;

namespace System.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static object GetDefaultValue(this PropertyInfo property)
        {
            var att = property.GetAttribute<DefaultValueAttribute>(false);
            if (null != att)
            {
                return att.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get public field/property value
        /// </summary>
        public static object GetFieldValue(this object instance, string fieldName)
        {
            Check.Require(instance, "instance");
            Check.Require(fieldName, "fieldName", Check.NotNullOrEmpty);

            var t = instance.GetType();

            var pi = t.DeepGetProperty(fieldName);
            if (pi != null)
            {
                return pi.GetValue(instance, null);
            }

            var fi = t.DeepGetField(fieldName, true, false, false);
            if (fi != null)
            {
                return fi.GetValue(instance);
            }

            return null;
        }

        /// <summary>
        /// Set public field/property value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        public static void SetFieldValue(this object instance, string fieldName, object fieldValue)
        {
            Check.Require(instance, "instance");
            Check.Require(fieldName, "fieldName", Check.NotNullOrEmpty);

            var t = instance.GetType();

            var pi = t.DeepGetProperty(fieldName);
            if (pi != null)
            {
                pi.SetValue(instance, fieldValue, null);
            }

            var fi = t.DeepGetField(fieldName, true, false, false);
            if (fi != null)
            {
                fi.SetValue(instance, fieldValue);
            }
        }

        public static bool HasAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
        {
            var attrs = property.GetCustomAttributes(typeof(TAttribute), false);
            return attrs.Length > 0;
        }
    }
}