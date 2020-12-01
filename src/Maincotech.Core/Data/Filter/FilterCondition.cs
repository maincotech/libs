using Maincotech.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Maincotech.Data
{
    public class FilterCondition : List<FilterGroup>
    {
        public bool HasValidFilter()
        {
            return Count > 0 && this.Any(x => x.HasValidFilter());
        }

        public Expression ToExpression(Type type)
        {
            var left = Expression.Parameter(type, "e");
            Expression filterExpression = null;
            foreach (var filterGroup in this)
            {
                Expression groupExpression = null;
                foreach (var filterRule in filterGroup)
                {
                    var currentExpression = ParseOneRule(type, filterRule, new[] { left });

                    if (groupExpression == null)
                    {
                        groupExpression = currentExpression;
                    }
                    else
                    {
                        groupExpression = filterRule.LogicalOperator == LogicalOperator.And ? Expression.And(groupExpression, currentExpression) : Expression.Or(groupExpression, currentExpression);
                    }
                }
                if (filterExpression == null)
                {
                    filterExpression = groupExpression;
                }
                else
                {
                    filterExpression = filterGroup.LogicalOperator == LogicalOperator.And ? Expression.And(filterExpression, groupExpression) : Expression.Or(filterExpression, groupExpression);
                }
            }
            if (filterExpression != null)
            {
                Expression lambda = Expression.Lambda(filterExpression, left);
                return lambda;
            }
            throw new ParseException($"Failed to parse filter group:{this.SafeToString()}");
        }

        private Expression ParseOneRule(Type type, FilterRule filterRule, ParameterExpression[] parameters = null)
        {
            ParameterChecker.ArgumentNotNullOrEmpty(filterRule.PropertyValues, nameof(filterRule.PropertyValues));

            switch (filterRule.FilterOperator)
            {
                case FilterOperator.Between:
                    ParameterChecker.Against<ParseException>(() => filterRule.PropertyValues.Count != 2,
                        $"Failed to parse filter rule:{filterRule.SafeToString()}");
                    return ExpressionExtensions.Between(type, filterRule.Field, filterRule.PropertyValues[0],
                        filterRule.PropertyValues[1], parameters);

                case FilterOperator.Contains:
                    return ExpressionExtensions.Contains(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.Equal:
                    return ExpressionExtensions.Equal(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.GreaterThan:
                    return ExpressionExtensions.GreaterThan(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.GreaterThanOrEqual:
                    return ExpressionExtensions.GreaterThanOrEqual(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.LessThan:
                    return ExpressionExtensions.LessThan(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.LessThanOrEqual:
                    return ExpressionExtensions.LessThanOrEqual(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.NotEqual:
                    return ExpressionExtensions.NotEqual(type, filterRule.Field, filterRule.PropertyValues[0], parameters);

                case FilterOperator.In:
                    return ExpressionExtensions.In(type, filterRule.Field, parameters, filterRule.PropertyValues.ToArray());
            }

            return Expression.Empty();
        }
    }
}