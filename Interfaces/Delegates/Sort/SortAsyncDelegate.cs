using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Status;

namespace Interfaces.Delegates.Sort
{
    public interface ISortAsyncDelegate<T>
    {
        Task SortAsync(List<T> collection, IStatus status);
    }
}