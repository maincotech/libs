using System.Collections.Generic;

namespace Maincotech.Data
{
    public abstract class PagingTranslater
    {
        private Pagination _pagination;
        private string _tableName;
        private string _innerJoin = string.Empty;
        private string _selectFields = "*";

        public PagingTranslater()
        {
            Parms = new List<FilterParameter>();
        }

        public Pagination Pagination { get; set; }

        public string TableName { get; set; }

        public SortGroup Sort { get; set; }

        public FilterGroup Filter { get; set; }

        public IList<FilterParameter> Parms { get; protected set; }

        public string InnerJoin
        {
            get { return _innerJoin; }
            set { _innerJoin = value; }
        }

        public string SelectFields
        {
            get { return _selectFields; }
            set { _selectFields = value; }
        }

        public abstract string GetTotalCommad();

        public abstract string GetDataCommand();
    }
}
