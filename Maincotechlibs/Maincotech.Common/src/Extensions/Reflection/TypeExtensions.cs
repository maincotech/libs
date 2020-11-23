using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        #region IsNullableType

        public static bool IsNullableType(this Type theType)
        {
            return (theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }

        #endregion IsNullableType

        #region GetOriginalTypeOfNullableType

        public static Type GetOriginalTypeOfNullableType(this Type type)
        {
            Check.Require(type, "type");

            if (type.ToString().StartsWith("System.Nullable`1["))
            {
                return GetType(type.ToString().Substring("System.Nullable`1[".Length).Trim('[', ']'));
            }

            return type;
        }

        #endregion GetOriginalTypeOfNullableType

        #region DeepGet

        /// <summary>
        /// Deeply get properties of specific types
        /// </summary>
        /// <param name="types">The types</param>
        /// <returns>The property infos</returns>
        public static PropertyInfo[] DeepGetProperties(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return new PropertyInfo[0];
            }
            var list = new List<PropertyInfo>();
            foreach (var t in types)
            {
                if (t != null)
                {
                    foreach (var pi in t.GetProperties())
                    {
                        list.Add(pi);
                    }

                    if (t.IsInterface)
                    {
                        var interfaceTypes = t.GetInterfaces();

                        if (interfaceTypes != null)
                        {
                            foreach (var pi in DeepGetProperties(interfaceTypes))
                            {
                                var isContained = false;

                                foreach (var item in list)
                                {
                                    if (item.Name == pi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(pi);
                                }
                            }
                        }
                    }
                    else
                    {
                        var baseType = t.BaseType;

                        if (baseType != typeof(object) && baseType != typeof(ValueType))
                        {
                            foreach (var pi in DeepGetProperties(baseType))
                            {
                                var isContained = false;

                                foreach (var item in list)
                                {
                                    if (item.Name == pi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(pi);
                                }
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Deeply get fields of specific fields
        /// </summary>
        /// <param name="types">The types</param>
        /// <returns>The field infos</returns>
        public static FieldInfo[] DeepGetFields(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return new FieldInfo[0];
            }
            var list = new List<FieldInfo>();
            foreach (var t in types)
            {
                if (t != null)
                {
                    foreach (var fi in t.GetFields())
                    {
                        list.Add(fi);
                    }

                    if (t.IsInterface)
                    {
                        var interfaceTypes = t.GetInterfaces();

                        if (interfaceTypes != null)
                        {
                            foreach (var fi in DeepGetFields(interfaceTypes))
                            {
                                var isContained = false;

                                foreach (var item in list)
                                {
                                    if (item.Name == fi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(fi);
                                }
                            }
                        }
                    }
                    else
                    {
                        var baseType = t.BaseType;

                        if (baseType != typeof(object) && baseType != typeof(ValueType))
                        {
                            foreach (var fi in DeepGetFields(baseType))
                            {
                                var isContained = false;

                                foreach (var item in list)
                                {
                                    if (item.Name == fi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(fi);
                                }
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Deeply get property info from specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static PropertyInfo DeepGetProperty(this Type type, string propertyName)
        {
            foreach (var pi in DeepGetProperties(type))
            {
                if (pi.Name == propertyName)
                {
                    return pi;
                }
            }

            return null;
        }

        /// <summary>
        /// Deeps the get field from specific type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="includePublic"></param>
        /// <param name="includeNonPublic"></param>
        /// <param name="isStatic"></param>
        /// <returns></returns>
        public static FieldInfo DeepGetField(this Type type, string name, bool includePublic, bool includeNonPublic, bool isStatic)
        {
            var t = type;

            if (t != null)
            {
                var flags = BindingFlags.Instance;
                if (includePublic)
                    flags |= BindingFlags.Public;
                if (includeNonPublic)
                    flags |= BindingFlags.NonPublic;
                if (isStatic)
                    flags |= BindingFlags.Static;
                var fi = t.GetField(name, flags);
                if (fi != null)
                {
                    return fi;
                }

                if (t.IsInterface)
                {
                    var interfaceTypes = t.GetInterfaces();

                    if (interfaceTypes != null)
                    {
                        foreach (var interfaceType in interfaceTypes)
                        {
                            fi = DeepGetField(interfaceType, name, includePublic, includeNonPublic, isStatic);
                            if (fi != null)
                            {
                                return fi;
                            }
                        }
                    }
                }
                else
                {
                    var baseType = t.BaseType;

                    if (baseType != typeof(object) && baseType != typeof(ValueType))
                    {
                        return DeepGetField(baseType, name, includePublic, includeNonPublic, isStatic);
                    }
                }
            }
            return null;
        }

        #endregion DeepGet

        #region GetType

        /// <summary>
        /// Gets a type in all loaded assemblies of current app domain.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public static Type GetType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }

            Type t = null;

            if (fullName.StartsWith("System.Nullable`1["))
            {
                var genericTypeStr = fullName.Substring("System.Nullable`1[".Length).Trim('[', ']');
                if (genericTypeStr.Contains(","))
                {
                    genericTypeStr = genericTypeStr.Substring(0, genericTypeStr.IndexOf(",")).Trim();
                }
                t = typeof(Nullable<>).MakeGenericType(GetType(genericTypeStr));

                if (t != null)
                {
                    return t;
                }
            }

            try
            {
                t = Type.GetType(fullName);
            }
            catch
            {
            }

            if (t == null)
            {
                try
                {
                    if (fullName.Contains(","))
                    {
                        var classNameAssembly = fullName.Split(',');
                        var ass = Assembly.LoadFrom(classNameAssembly[1]);
                        if (ass != null)
                            t = ass.GetType(classNameAssembly[0]);
                    }
                    else
                    {
                        var asses = AppDomain.CurrentDomain.GetAssemblies();

                        for (var i = asses.Length - 1; i >= 0; i--)
                        {
                            var ass = asses[i];
                            try
                            {
                                t = ass.GetType(fullName);
                            }
                            catch
                            {
                            }

                            if (t != null)
                            {
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return t;
        }

        #endregion GetType

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type) where T : Attribute
        {
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(T), false));
            return properties;
        }
    }
}