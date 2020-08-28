using SRSoft.Infrastructure.Extensions.Linq.Expressions;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two given expressions by using the AND semantics.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="first">The first part of the expression.</param>
        /// <param name="second">The second part of the expression.</param>
        /// <returns>The combined expression.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<TComparable, TComparable, TComparable, bool>> Between<TComparable>(this TComparable thisComparable) where TComparable : IComparable
        {
            return (value, lower, upper) => lower.CompareTo(value) <= 0 && value.CompareTo(upper) <= 0;
        }

        public static Expression<Func<T, bool>> Between<T, TValue>(Expression<Func<T, TValue>> selector,
                TValue startValue, TValue endValue)
        {
            var p = selector.Parameters.Single();
            var left = Expression.LessThanOrEqual(selector.Body, Expression.Constant(startValue, typeof(TValue)));
            var right = Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(startValue, typeof(TValue)));
            return Expression.Lambda<Func<T, bool>>(Expression.And(left, right), p);
        }

        public static Expression Between<T>(string propertyName, object startValue, object endValue, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return Between(type, propertyName, startValue, endValue, parameters);
        }

        public static Expression Between(Type type, string propertyName, object startValue, object endValue, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            var p = selector.Parameters.Single();
            var left = Expression.LessThanOrEqual(selector.Body, Expression.Constant(endValue, propertyInfo.PropertyType));
            var right = Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(startValue, propertyInfo.PropertyType));
            var lambda = Expression.Lambda(Expression.And(left, right), p);
            return lambda.Body;
        }

        public static Expression Contains<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return Contains(type, propertyName, value, parameters);
        }

        public static Expression Contains(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.Call(selector.Body,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                        Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression Equal<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return Equal(type, propertyName, value, parameters);
        }

        public static Expression Equal(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.Equal(selector.Body, Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        public static LambdaExpression GenerateMemberExpression<T>(string propertyName, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return GenerateMemberExpression(type, propertyName, parameters);
        }

        public static LambdaExpression GenerateMemberExpression(Type type, string propertyName, ParameterExpression[] parameters)
        {
            //   var columnPropInfo = type.GetProperty(propertyName);
            var entityParam = parameters[0];                         // {e}
            Expression propertySelector = entityParam;
            propertyName.Split('.').ToList().ForEach(item => propertySelector = Expression.Property(propertySelector, item));
            // var columnExpr = Expression.MakeMemberAccess(entityParam, columnPropInfo); // {e.column}
            return Expression.Lambda(propertySelector, parameters);                   // {e => e.column}
        }

        public static Expression GreaterThan<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return GreaterThan(type, propertyName, value, parameters);
        }

        public static Expression GreaterThan(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.GreaterThan(selector.Body, Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression GreaterThanOrEqual<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return GreaterThanOrEqual(type, propertyName, value, parameters);
        }

        public static Expression GreaterThanOrEqual(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression<Func<T, bool>> In<T, TValue>(Expression<Func<T, TValue>> selector,
                params TValue[] collection)
        {
            var p = selector.Parameters.Single();
            var equals = collection.Select(value => (Expression)Expression.Equal(selector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate(Expression.Or);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression In<T>(string propertyName, ParameterExpression[] parameters, object[] collection)
        {
            var type = typeof(T);
            return In(type, propertyName, parameters, collection);
        }

        public static Expression In(Type type, string propertyName, ParameterExpression[] parameters, object[] collection)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            var p = selector.Parameters.Single();
            var equals = collection.Select(value => (Expression)Expression.Equal(selector.Body, Expression.Constant(value, propertyInfo.PropertyType)));
            var body = equals.Aggregate(Expression.Or);
            return Expression.Lambda(body, p).Body;
        }

        public static Expression LessThan<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return LessThan(type, propertyName, value, parameters);
        }

        public static Expression LessThan(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.LessThan(selector.Body, Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression LessThanOrEqual<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return LessThanOrEqual(type, propertyName, value, parameters);
        }

        public static Expression LessThanOrEqual(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.LessThanOrEqual(selector.Body, Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression NotEqual<T>(string propertyName, object value, ParameterExpression[] parameters)
        {
            var type = typeof(T);
            return NotEqual(type, propertyName, value, parameters);
        }

        public static Expression NotEqual(Type type, string propertyName, object value, ParameterExpression[] parameters)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            return Expression.NotEqual(selector.Body, Expression.Constant(value, propertyInfo.PropertyType));
        }

        public static Expression NotIn(Type type, string propertyName, ParameterExpression[] parameters, params object[] collection)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var selector = GenerateMemberExpression(type, propertyName, parameters);
            var p = selector.Parameters.Single();
            var equals = collection.Select(value => (Expression)Expression.NotEqual(selector.Body, Expression.Constant(value, propertyInfo.PropertyType)));
            var body = equals.Aggregate(Expression.And);
            return Expression.Lambda<Func<bool>>(body, p);
        }

        public static Expression<Func<T, bool>> NotIn<T, TValue>(Expression<Func<T, TValue>> selector,
                params TValue[] collection)
        {
            var p = selector.Parameters.Single();
            var equals = collection.Select(value => (Expression)Expression.NotEqual(selector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate(Expression.And);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        /// <summary>
        /// Combines two given expressions by using the OR semantics.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="first">The first part of the expression.</param>
        /// <param name="second">The second part of the expression.</param>
        /// <returns>The combined expression.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static bool TryGetEntityAndFieldNameFromExpression(this Expression expression, out object entity, out string fieldName)
        {
            entity = null;
            fieldName = null;

            try
            {
                var memberExpression = expression as MemberExpression;

                if (memberExpression != null)
                {
                    var entityLambda = Expression.Lambda<Func<object>>(memberExpression.Expression);
                    entity = entityLambda.Compile()();
                    fieldName = memberExpression.Member.Name;
                    return true;
                }

                var unaryExpression = expression as UnaryExpression;

                if (unaryExpression != null)
                    if (unaryExpression.NodeType == ExpressionType.Convert || unaryExpression.NodeType == ExpressionType.MemberAccess)
                        return TryGetEntityAndFieldNameFromExpression(unaryExpression.Operand, out entity, out fieldName);
            }
            catch (Exception) { }

            return false;
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}