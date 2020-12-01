using System.Collections.Generic;

namespace System.Reflection
{
    /// <summary>
    /// The MemberInfo Extensions
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="findChildAttributes">if set to <c>true</c> [find child attributes].</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo member, bool findChildAttributes)
            where T : Attribute
        {
            var attrs = member.GetCustomAttributes(true);

            if (attrs != null && attrs.Length > 0)
            {
                for (var i = 0; i < attrs.Length; ++i)
                {
                    if (findChildAttributes ? typeof(T).IsAssignableFrom(attrs[i].GetType()) : typeof(T) == attrs[i].GetType())
                    {
                        return (T)attrs[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            return GetAttribute<T>(member, false);
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="findChildAttributes">if set to <c>true</c> [find child attributes].</param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(this MemberInfo member, bool findChildAttributes)
            where T : Attribute
        {
            Check.Require(member, "member");

            var list = new List<T>();

            var attrs = member.GetCustomAttributes(true);

            if (attrs != null && attrs.Length > 0)
            {
                for (var i = 0; i < attrs.Length; ++i)
                {
                    if (findChildAttributes ? typeof(T).IsAssignableFrom(attrs[i].GetType()) : typeof(T) == attrs[i].GetType())
                    {
                        list.Add((T)attrs[i]);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(this MemberInfo member)
            where T : Attribute
        {
            return GetAttributes<T>(member, false);
        }

        /// <summary>
        /// Returns the type of the specified member
        /// </summary>
        /// <param name="memberInfo">member to get type from</param>
        /// <returns>Member type</returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            Check.Require(memberInfo, "memberInfo");

            Check.Require(memberInfo is FieldInfo || memberInfo is PropertyInfo || memberInfo is MethodInfo || memberInfo is ConstructorInfo || memberInfo is Type);

            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;
            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;
            if (memberInfo is MethodInfo)
                return ((MethodInfo)memberInfo).ReturnType;
            if (memberInfo is ConstructorInfo)
                return null;
            if (memberInfo is Type)
                return (Type)memberInfo;

            throw new ArgumentException();
        }

        /// <summary>
        /// Gets a field/property
        /// </summary>
        /// <param name="memberInfo">The memberInfo specifying the object</param>
        /// <param name="o">The object</param>
        public static object GetMemberValue(this MemberInfo memberInfo, object o)
        {
            Check.Require(memberInfo, "memberInfo");
            Check.Require(memberInfo is FieldInfo || memberInfo is PropertyInfo);

            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).GetValue(o);
            return ((PropertyInfo)memberInfo).GetGetMethod().Invoke(o, new object[0]);
        }

        /// <summary>
        /// Sets a field/property
        /// </summary>
        /// <param name="memberInfo">The memberInfo specifying the object</param>
        /// <param name="o">The object</param>
        /// <param name="value">The field/property value to assign</param>
        public static void SetMemberValue(this MemberInfo memberInfo, object o, object value)
        {
            Check.Require(memberInfo, "memberInfo");

            Check.Require(memberInfo is FieldInfo || memberInfo is PropertyInfo);

            if (memberInfo is FieldInfo)
                ((FieldInfo)memberInfo).SetValue(o, value);
            else //if (memberInfo is PropertyInfo)
                ((PropertyInfo)memberInfo).GetSetMethod().Invoke(o, new[] { value });
        }

        /// <summary>
        /// If memberInfo is a method related to a property, returns the PropertyInfo
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static PropertyInfo GetExposingProperty(this MemberInfo memberInfo)
        {
            Check.Require(memberInfo, "memberInfo");
            var reflectedType = memberInfo.ReflectedType;
            foreach (var propertyInfo in reflectedType.GetProperties())
            {
                if (propertyInfo.GetGetMethod() == memberInfo || propertyInfo.GetSetMethod() == memberInfo)
                    return propertyInfo;
            }
            return null;
        }
    }
}