using System.Data;

namespace Maincotech.Data
{
    public class DataPageRetriever
    {
        private readonly DbHelper _dbHelper;
        private readonly PagingTranslater _pagingTranslater;

        public DataPageRetriever(DbHelper dbHelper, PagingTranslater pagingTranslater)
        {
            this._dbHelper = dbHelper;
            this._pagingTranslater = pagingTranslater;
        }

        public DataTable GetPagedData(Pagination pagination)
        {
            if (pagination.Total.HasValue == false)
            {
                var countCmdText = _pagingTranslater.GetTotalCommad();
                using (var cmd = _dbHelper.GetSqlStringCommond(countCmdText))
                {
                    pagination.Total = (int)_dbHelper.ExecuteScalar(cmd);
                }
            }

            var getDataCmdText = _pagingTranslater.GetDataCommand();
            using (var cmd = _dbHelper.GetSqlStringCommond(getDataCmdText))
            {
                return _dbHelper.ExecuteDataTable(cmd);
            }
        }
    }
}