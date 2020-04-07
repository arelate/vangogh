using System.Threading.Tasks;
using System.Collections.Generic;

namespace Interfaces.Delegates.Collections
{
    public interface ISortAsyncDelegate<T>
    {
        Task SortAsync(List<T> collection);
    }
}