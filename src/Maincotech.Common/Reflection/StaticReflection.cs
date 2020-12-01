using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MaincoTech.Reflection
{
    /// <summary>
    ///
    /// </summary>
    public class StaticReflection
    {
        private static readonly BindingFlags _flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
        private static readonly List<Assembly> _typeSearchAssemblies = new List<Assembly>();
        private static readonly Hashtable _typeTable = new Hashtable();

        public static void AddEventHandler(object obj, string eventName, MethodInfo method, object methodOwner)
        {
            var eventInfo = obj.GetType().GetEvent(eventName, _flags);
            var eventDeleg = Delegate.CreateDelegate(eventInfo.EventHandlerType, methodOwner, method);
            eventInfo.AddEventHandler(obj, eventDeleg);
        }

        public static void AddTypeSearchAssembly(Assembly asm)
        {
            if (asm == null) return;

            lock (_typeSearchAssemblies)
            {
                if (!_typeSearchAssemblies.Contains(asm))
                {
                    _typeSearchAssemblies.Add(asm);
                }
            }
        }

        public static void AddTypeSearchAssembly(Type seedType)
        {
            AddTypeSearchAssembly(seedType.Assembly);
        }

        public static void AddTypeSearchAssemblyTree(Assembly asm)
        {
            if (asm == null) return;

            lock (_typeSearchAssemblies)
            {
                if (!_typeSearchAssemblies.Contains(asm))
                {
                    _typeSearchAssemblies.Add(asm);

                    // get reference assemblies
                    var childrenAsmNames = asm.GetReferencedAssemblies();
                    foreach (var asmName in childrenAsmNames)
                    {
                        // this may take more space, which is normally OK
                        try
                        {
                            var child = Assembly.Load(asmName);
                            if (!_typeSearchAssemblies.Contains(child))
                                _typeSearchAssemblies.Add(child);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        public static object CallMethod(object obj, string funcName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = GetMethod(type, funcName, GetTypesFromObjects(parameters));
            return method.Invoke(obj, parameters);
        }

        public static object CallStaticMethod(Type type, string funcName, params object[] parameters)
        {
            var method = GetMethod(type, funcName, GetTypesFromObjects(parameters));
            return method.Invoke(null, parameters);
        }

        public static object CreateInstance(Type type, Type[] paramTypes, params object[] parameters)
        {
            return GetConstructor(type, paramTypes).Invoke(parameters);
        }

        public static object CreateInstance(Type type, params object[] parameters)
        {
            return GetConstructor(type, GetTypesFromObjects(parameters)).Invoke(parameters);
        }

        // Methods
        public static ConstructorInfo GetConstructorInfo<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body as NewExpression;
            if (body == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return body.Constructor;
        }

        public static object GetField(object obj, string fieldName)
        {
            var type = obj.GetType();
            return type.GetField(fieldName, _flags).GetValue(obj);
        }

        public static Type GetInnerType(string hiddenTypeName, Type outerType)
        {
            var typeModule = outerType.Module;
            return typeModule.GetType(hiddenTypeName);
        }

        public static MethodInfo GetMethod(Type type, string funcName, Type[] paramTypes)
        {
            if (paramTypes != null)
                return type.GetMethod(funcName, _flags, null, paramTypes, null);
            return type.GetMethod(funcName, _flags);
        }

        public static MethodInfo GetMethod(Type type, string funcName)
        {
            return GetMethod(type, funcName, null);
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }


        public static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            var type = obj.GetType();
            return type.GetProperty(propertyName, _flags);
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, _flags);
        }

        public static object GetProperty(object obj, string propertyName)
        {
            return GetProperty(obj, propertyName, null);
        }

        public static object GetProperty(object obj, string propertyName, params object[] index)
        {
            var type = obj.GetType();
            return type.GetProperty(propertyName, _flags).GetValue(obj, index);
        }

        public static MethodInfo GetPropertyGetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var getMethod = GetPropertyInfo<T, TProperty>(expression).GetGetMethod();
            if (getMethod == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return getMethod;
        }

        public static MethodInfo GetPropertySetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var setMethod = GetPropertyInfo<T, TProperty>(expression).GetSetMethod();
            if (setMethod == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return setMethod;
        }

        public static object GetSaticField(Type type, string fieldName)
        {
            return type.GetField(fieldName, _flags).GetValue(null);
        }

        public static object GetStaticProperty(Type type, string propertyName)
        {
            return GetStaticProperty(type, propertyName, null);
        }

        public static object GetStaticProperty(Type type, string propertyName, params object[] index)
        {
            return type.GetProperty(propertyName, _flags).GetValue(null, index);
        }

        public static Type GetType(Assembly asm, string typeFullName)
        {
            return asm.GetType(typeFullName);
        }

        public static Type GetType(string typeFullName)
        {
            AddTypeSearchAssembly(Assembly.GetCallingAssembly());

            lock (_typeSearchAssemblies)
            {
                if (_typeTable.ContainsKey(typeFullName))
                {
                    return (Type)_typeTable[typeFullName];
                }
                foreach (var asm in _typeSearchAssemblies)
                {
                    var type = asm.GetType(typeFullName, false, true);
                    if (type != null)
                    {
                        _typeTable.Add(typeFullName, type);
                        return type;
                    }
                }
            }
            throw new Exception("Can not find the specified type" + typeFullName);
        }

        public static void RemoveEventHandler(object obj, string eventName, MethodInfo method, object methodOwner)
        {
            var eventInfo = obj.GetType().GetEvent(eventName, _flags);
            var eventDeleg = Delegate.CreateDelegate(eventInfo.EventHandlerType, methodOwner, method);
            eventInfo.RemoveEventHandler(obj, eventDeleg);
        }

        public static void SetField(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            type.GetField(fieldName, _flags).SetValue(obj, value);
        }

        public static void SetProperty(object obj, string propertyName, object value)
        {
            SetProperty(obj, propertyName, value, null);
        }

        public static void SetProperty(object obj, string propertyName, object value, params object[] index)
        {
            var type = obj.GetType();
            type.GetProperty(propertyName, _flags).SetValue(obj, value, index);
        }

        public static void SetStaticField(Type type, string fieldName, object value)
        {
            type.GetField(fieldName, _flags).SetValue(null, value);
        }

        public static void SetStaticProperty(Type type, string propertyName, object value)
        {
            SetStaticProperty(type, propertyName, value, null);
        }

        public static void SetStaticProperty(Type type, string propertyName, object value, params object[] index)
        {
            type.GetProperty(propertyName, _flags).SetValue(null, value, index);
        }

        private static ConstructorInfo GetConstructor(Type type, Type[] paramTypes)
        {
            return type.GetConstructor(paramTypes);
        }

        private static MethodInfo GetMethodInfo(LambdaExpression lambda)
        {
            GuardProperExpressionForm(lambda.Body);
            var body = (MethodCallExpression)lambda.Body;
            return body.Method;
        }

        private static PropertyInfo GetPropertyInfo<T, TProperty>(LambdaExpression lambda)
        {
            var body = lambda.Body as MemberExpression;
            if (body == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            var member = body.Member as PropertyInfo;
            if (member == null)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
            return member;
        }

        private static Type[] GetTypesFromObjects(object[] objs)
        {
            var types = new Type[objs.Length];
            for (var i = 0; i < types.Length; i++)
                types[i] = objs[i].GetType();
            return types;
        }

        private static void GuardProperExpressionForm(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
        }

        #region Some Generic methods

        public static T[] GetMethodAttributes<T>(MethodInfo mi, bool inhert) where T : Attribute
        {
            return (T[])mi.GetCustomAttributes(typeof(T), inhert);
        }

        public static T[] GetParemeterAttributes<T>(ParameterInfo pi, bool inhert) where T : Attribute
        {
            return (T[])pi.GetCustomAttributes(typeof(T), inhert);
        }

        public static T[] GetPropertyAttributes<T>(PropertyInfo pi, bool inhert) where T : Attribute
        {
            return (T[])pi.GetCustomAttributes(typeof(T), inhert);
        }

        public static T[] GetTypeAttributes<T>(Type type, bool inhert) where T : Attribute
        {
            return (T[])type.GetCustomAttributes(typeof(T), inhert);
        }

        #endregion Some Generic methods
    }
}