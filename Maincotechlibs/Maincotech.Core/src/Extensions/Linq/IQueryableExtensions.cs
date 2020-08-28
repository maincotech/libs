namespace System.Linq
{
    using Maincotech.Data;
    using Maincotech.Domain;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class IQueryableExtensions
    {
        #region FirstOrDefault

        public static object FirstOrDefault(this IQueryable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var obj in source)
                return obj;

            return null;
        }

        #endregion FirstOrDefault

        #region Sort

        public static IQueryable Sort(this IQueryable source, SortGroup sortGroup)
        {
            var parameters = new[] { Expression.Parameter(source.ElementType, "e") };
            var orderings = sortGroup.ToDynamicOrderings(source.ElementType, parameters);
            var queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";
            foreach (var ordering in orderings)
            {
                queryExpr = Expression.Call(typeof(Queryable),
                    ordering.SortOrder == SortOrder.Ascending ? methodAsc : methodDesc,
                    new[] { source.ElementType, ordering.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(ordering.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, SortGroup sortGroup)
        {
            return (IQueryable<T>)Sort((IQueryable)source, sortGroup);
        }

        public static IOrderedQueryable<TAggregateRoot> SortBy<TAggregateRoot>(this IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, dynamic>> sortPredicate)
    where TAggregateRoot : class, IEntity
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Ascending);
        }

        public static IOrderedQueryable<TAggregateRoot> SortByDescending<TAggregateRoot>(this IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, dynamic>> sortPredicate)
            where TAggregateRoot : class, IEntity
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Descending);
        }

        #endregion Sort

        #region Filtering

        public static IQueryable Filter(this IQueryable source, FilterCondition condition)
        {
            var lambda = condition.ToExpression(source.ElementType);
            return source.Provider.CreateQuery(
               Expression.Call(
                   typeof(Queryable), "Where",
                   new[] { source.ElementType },
                   source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> source, FilterCondition condition)
        {
            return (IQueryable<T>)Filter((IQueryable)source, condition);
        }

        #endregion Filtering

        #region Private Methods

        private static string GetSortingMethodName(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "OrderBy";

                case SortOrder.Descending:
                    return "OrderByDescending";

                default:
                    throw new ArgumentException("Sort Order must be specified as either Ascending or Descending.",
            "sortOrder");
            }
        }

        private static IOrderedQueryable<TAggregateRoot> InvokeSortBy<TAggregateRoot>(IQueryable<TAggregateRoot> query,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
            where TAggregateRoot : class, IEntity
        {
            var param = sortPredicate.Parameters[0];
            string propertyName = null;
            Type propertyType = null;
            Expression bodyExpression = null;
            if (sortPredicate.Body is UnaryExpression)
            {
                var unaryExpression = sortPredicate.Body as UnaryExpression;
                bodyExpression = unaryExpression.Operand;
            }
            else if (sortPredicate.Body is MemberExpression)
            {
                bodyExpression = sortPredicate.Body;
            }
            else
                throw new ArgumentException(@"The body of the sort predicate expression should be
                either UnaryExpression or MemberExpression.", "sortPredicate");
            var memberExpression = (MemberExpression)bodyExpression;
            propertyName = memberExpression.Member.Name;
            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                propertyType = propertyInfo.PropertyType;
            }
            else
                throw new InvalidOperationException(@"Cannot evaluate the type of property since the member expression
                represented by the sort predicate expression does not contain a PropertyInfo object.");

            var funcType = typeof(Func<,>).MakeGenericType(typeof(TAggregateRoot), propertyType);
            var convertedExpression = Expression.Lambda(funcType,
                Expression.Convert(Expression.Property(param, propertyName), propertyType), param);

            var sortingMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var sortingMethodName = GetSortingMethodName(sortOrder);
            var sortingMethod = sortingMethods.Where(sm => sm.Name == sortingMethodName &&
                sm.GetParameters() != null &&
                sm.GetParameters().Length == 2).First();
            return (IOrderedQueryable<TAggregateRoot>)sortingMethod
                .MakeGenericMethod(typeof(TAggregateRoot), propertyType)
                .Invoke(null, new object[] { query, convertedExpression });
        }

        #endregion Private Methods
    }
}