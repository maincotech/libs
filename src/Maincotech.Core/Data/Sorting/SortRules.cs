namespace Maincotech.Data
{
    public static class SortRules
    {
        public static SortRule LastModifiedTime
            => new SortRule { Field = "LastModifiedTime", SortOrder = SortOrder.Descending };
    }
}