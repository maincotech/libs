using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maincotech.Data
{
    public interface IHierarchyDataRepository
    {
        Task<IEnumerable<IHierarchyData>> LoadChildren(IHierarchyData current);
    }
}