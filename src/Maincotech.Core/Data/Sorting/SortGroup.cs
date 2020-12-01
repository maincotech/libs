using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Maincotech.Data
{
    [Serializable]
    public class SortGroup
    {
        public IList<SortRule> SortRules { get; set; }

        internal IEnumerable<DynamicOrdering> ToDynamicOrderings(Type type, ParameterExpression[] parameters)
        {
            if (SortRules == null || SortRules.Count == 0)
            {
                return null;
            }

            return (from sortRule in SortRules
                    select new DynamicOrdering
                    {
                        SortOrder = sortRule.SortOrder,
                        Selector = ExpressionExtensions.GenerateMemberExpression(type, sortRule.Field, parameters).Body
                    }).ToList();
        }
    }
}