using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Maincotech.Data
{
    public interface ISelectableHierarchyDataSource<T> where T : IHierarchyData
    {
        ObservableCollection<T> Roots { get; }

        T Current { get; set; }

        Task<T> Find(string fullPath);

        event EventHandler<EventArgs> SelectionChanged;

    }
}