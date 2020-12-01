using System.Data;
using System.Diagnostics;

namespace Maincotech.Data
{
    public class DataPage
    {
        private readonly DataTable _table;
        private readonly int _lowestIndexValue;
        private readonly int _highestIndexValue;
        private readonly int _pageSize;

        public object this[int rowIndex, int columnIndex]
        {
            get
            {
                return _table.Rows[MapToTableRowIndex(rowIndex)][columnIndex];
            }
        }

        private int MapToTableRowIndex(int rowIndex)
        {
            return rowIndex - _lowestIndexValue;
        }

        public DataPage(DataTable table, int rowIndex, int pageSize)
        {
            this._table = table;
            Debug.Assert(pageSize > 0);
            this._pageSize = pageSize;

            _lowestIndexValue = MapToLowerBoundary(rowIndex);
            _highestIndexValue = MapToUpperBoundary(rowIndex);
            Debug.Assert(_lowestIndexValue >= 0);
            Debug.Assert(_highestIndexValue >= 0);
        }

        public int LowestIndex
        {
            get
            {
                return _lowestIndexValue;
            }
        }

        public int HighestIndex
        {
            get
            {
                return _highestIndexValue;
            }
        }

        private int MapToLowerBoundary(int rowIndex)
        {
            // Return the lowest index of a page containing the given index.
            return (rowIndex / _pageSize) * _pageSize + rowIndex % _pageSize;
        }

        private int MapToUpperBoundary(int rowIndex)
        {
            // Return the highest index of a page containing the given index.
            return _lowestIndexValue + _table.Rows.Count - 1;
        }
    }
}
