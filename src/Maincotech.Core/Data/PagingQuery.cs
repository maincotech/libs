namespace Maincotech.Data
{
    public class PagingQuery
    {
        public FilterCondition FilterCondition { get; set; }

        public SortGroup SortGroup { get; set; }

        public Pagination Pagination { get; set; }
    }
}