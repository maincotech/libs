using Maincotech.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System
{
    public static class ObjectExtension
    {
        public static string SafeToString(this object source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            if (DBNull.Value.Equals(source))
            {
                return string.Empty;
            }
            if (source.IsSimpleType())
            {
                return source.ToString();
            }

            return SafeToString(source, null);
        }

        public static string SafeToString(this object source, IList<string> excludePropertyNames)
        {
            var type = source.GetType();
            var sb = new StringBuilder();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            sb.Append("{");
            foreach (var property in properties)
            {
                if (excludePropertyNames == null || !excludePropertyNames.Contains(property.Name))
                {
                    var value = property.GetValue(source, null);
                    var valueBuilder = new StringBuilder();
                    if (value != null)
                    {
                        if (property.PropertyType.IsArray || property.PropertyType.IsGenericType)
                        {
                            var enumeralbe = value as IEnumerable;
                            if (enumeralbe != null)
                            {
                                valueBuilder.Append('[');
                                foreach (var objValue in enumeralbe)
                                {
                                    valueBuilder.Append(objValue + ",");
                                }
                                valueBuilder.Remove(valueBuilder.Length - 1, 1);
                                valueBuilder.Append("]");
                            }
                            else
                            {
                                valueBuilder.Append(value);
                            }
                        }
                        else
                        {
                            valueBuilder.Append(value);
                        }
                    }
                    sb.AppendFormat("{0}:{1},", property.Name, valueBuilder);
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///     验证字符串是否有sql注入字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidInput(this object objInput)
        {
            try
            {
                if (objInput.IsNullOrEmpty())
                    return false;
                var input = objInput.ToString();
                //替换单引号
                input = input.Replace("'", "''").Trim();

                //检测攻击性危险字符串
                var testString =
                    "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                var testArray = testString.Split('|');
                foreach (var testStr in testArray)
                {
                    if (input.ToLower().IndexOf(testStr) != -1)
                    {
                        //检测到攻击字符串,清空传入的值
                        input = "";
                        return false;
                    }
                }

                //未检测到攻击字符串
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool IsSimpleType(this object @this)
        {
            var type = @this.GetType();
            if (type.IsPrimitive || type.Equals(typeof(string)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     判断对象是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的对象</param>
        public static bool IsNullOrEmpty(this object data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            if (data is IEnumerable)
            {
                var enumerable = data as IEnumerable;
                return EnumerableExtensions.IsNullOrEmpty(enumerable);
            }

            if (data is Guid)
            {
                var guid = (Guid)data;
                return guid.IsNullOrEmpty();
            }

            //不为空
            return false;
        }

        public static bool IsNotNullOrEmpty(this object data)
        {
            return !IsNullOrEmpty(data);
        }

        public static object GetNullaleTypeValue<T>(this T obj)
        {
            var type = typeof(T);

            if (Nullable.GetUnderlyingType(type) != null)
            {
                var valueProperty = type.GetProperty("Value");
                return valueProperty.GetValue(obj, null);
            }

            return obj;
        }

        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true; // obvious
            var type = typeof(T);
            if (!type.IsValueType)
            {
                return true; // ref-type
            }
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return true; // Nullable<T>
            }
            return false; // value-type
        }

        public static string ToString<TSource>(this TSource source)
        {
            return ToString(source, new List<string>());
        }

        public static T DeepClone<T>(this T @this)
        {
            if (@this == null)
            {
                return default(T);
            }
            var memStr = SerializerHelper.SerializeToBase64String(@this);
            return SerializerHelper.DeserializeFromBase64String<T>(memStr);
        }

        public static void MergeDataFrom(this object @this, object source, IList<string> excludePropertyNames = null)
        {
            var type = @this.GetType();
            var sourceType = source.GetType();
            var properties = type.GetProperties().Where(prop => prop.CanWrite &&
                                                                (excludePropertyNames == null ||
                                                                 !excludePropertyNames.Contains(prop.Name))).ToArray();
            var canMapProperties = sourceType.GetProperties().Where(prop => prop.CanRead &&
                                                                            (excludePropertyNames == null ||
                                                                             !excludePropertyNames.Contains(prop.Name))
                                                                            &&
                                                                            properties.Any(
                                                                                targetProp =>
                                                                                    targetProp.Name == prop.Name &&
                                                                                    targetProp.PropertyType ==
                                                                                    prop.PropertyType));
            foreach (var prop in canMapProperties)
            {
                var targetProperty = properties.First(targetProp => targetProp.Name == prop.Name);
                var value = prop.GetValue(source, null);
                if (value != null)
                {
                    targetProperty.SetValue(@this, value, null);
                }
            }
        }

        public static void MergeDataFrom<T>(this T @this, T source, IList<string> excludePropertyNames = null)
        {
            var type = typeof(T);
            var canMapProperties =
                type.GetProperties()
                    .Where(
                        prop =>
                            prop.CanRead && prop.CanWrite &&
                            (excludePropertyNames == null || !excludePropertyNames.Contains(prop.Name)));

            foreach (var prop in canMapProperties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                {
                    prop.SetValue(@this, value, null);
                }
            }
        }

        public static string ToString<TSource>(this TSource source,
            IEnumerable<Expression<Func<TSource, object>>> excludeProperties)
        {
            IList<string> excludePropertyNames = null;
            if (excludeProperties != null)
            {
                excludePropertyNames = new List<string>();
            }

            foreach (var expression in excludeProperties)
            {
                var memberInfo = FindProperty(expression);
                excludePropertyNames.Add(memberInfo.Name);
            }
            return ToString(source, excludePropertyNames);
        }

        private static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression operand = lambdaExpression;
            var flag = false;
            while (!flag)
            {
                var nodeType = operand.NodeType;
                if (nodeType == ExpressionType.Convert)
                {
                    operand = ((UnaryExpression)operand).Operand;
                }
                else
                {
                    if (nodeType == ExpressionType.Lambda)
                    {
                        operand = ((LambdaExpression)operand).Body;
                    }
                    else
                    {
                        if (nodeType == ExpressionType.MemberAccess)
                        {
                            var memberExpression = (MemberExpression)operand;
                            if (memberExpression.Expression.NodeType == ExpressionType.Parameter ||
                                memberExpression.Expression.NodeType == ExpressionType.Convert)
                            {
                                var member = memberExpression.Member;
                                return member;
                            }
                            throw new ArgumentException(
                                string.Format(
                                    "Expression '{0}' must resolve to top-level member and not any child object's properties. Use a custom resolver on the child type or the AfterMap option instead.",
                                    lambdaExpression), "lambdaExpression");
                        }
                        flag = true;
                    }
                }
            }
            throw new Exception(
                "Custom configuration for members is only supported for top-level individual members on a type.");
        }

        public static string ToString<TSource>(this TSource source, IList<string> excludePropertyNames)
        {
            var type = typeof(TSource);
            if (source == null)
            {
                return string.Format("This is null object of type:{0}.", type.FullName);
            }

            var sb = new StringBuilder();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (excludePropertyNames == null || !excludePropertyNames.Contains(property.Name))
                {
                    sb.Append("[" + property.Name + ": " + property.GetValue(source, null) + "] ");
                }
            }
            return sb.ToString();
        }
    }
}