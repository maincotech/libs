using System;

namespace Maincotech.Data
{
    [Serializable]
    public class SortRule
    {
        public string Field { get; set; }

        public SortOrder SortOrder { get; set; }
    }
}