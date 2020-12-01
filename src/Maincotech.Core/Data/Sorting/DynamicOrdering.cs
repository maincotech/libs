using System.Linq.Expressions;

namespace Maincotech.Data
{
    public class DynamicOrdering
    {
        public Expression Selector { get; set; }

        public SortOrder SortOrder { get; set; }
    }
}