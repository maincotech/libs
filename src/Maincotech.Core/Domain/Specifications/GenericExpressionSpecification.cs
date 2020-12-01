using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maincotech.Domain.Specifications
{
    public class GenericExpressionSpecification<TEntity, TResult>
    {
        private readonly string propertyName;

        public GenericExpressionSpecification(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public Expression<Func<TEntity, TResult>> GetExpression()
        {
            if (propertyName.IsNullOrEmpty()) return null;
            var left = Expression.Parameter(typeof(TEntity), "c");
            Expression right = left;
            propertyName.Split('.').ToList().ForEach(item => right = Expression.Property(right, item));
            var finalExpression
                = Expression.Lambda<Func<TEntity, TResult>>(right, left);
            return finalExpression;
        }
    }
}