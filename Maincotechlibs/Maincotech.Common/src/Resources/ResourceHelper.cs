using MaincoTech.Reflection;
using System;
using System.Reflection;

namespace Maincotech.Resources
{
    public static partial class ResourceHelper
    {
        public static string GetString(Type resourceType, string resourceName, params object[] values)
        {
            var result = resourceName;
            try
            {
                if ((resourceType == null) || (resourceName == null))
                {
                    return result;
                }
                var property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

                if (property != null)
                {
                    result = (string)property.GetValue(null);
                }

                if (!string.IsNullOrEmpty(result) && values != null && values.Length > 0)
                {
                    return string.Format(result, values);
                }
            }
            catch (Exception)
            {
                //Ignore
            }

            return result;
        }

        public static string GetCommonString(string resourceName, params object[] values)
        {
            return GetString(typeof(ResourcesCommon), resourceName, values);
        }
    }
}