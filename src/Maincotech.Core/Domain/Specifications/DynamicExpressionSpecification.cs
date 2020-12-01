using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maincotech.Domain.Specifications
{
    public class DynamicExpressionSpecification<T>
    {
        private readonly string propertyName;

        public DynamicExpressionSpecification(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public Expression<Func<T, dynamic>> GetExpression()
        {
            if (propertyName.IsNullOrEmpty()) return null;
            var left = Expression.Parameter(typeof(T), "c");
            Expression right = left;
            propertyName.Split('.').ToList().ForEach(item => right = Expression.Property(right, item));
            var finalExpression
                = Expression.Lambda<Func<T, dynamic>>(Expression.Convert(right, typeof(object)), left);
            return finalExpression;
        }
    }
}