using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.DataRefinement
{
    public interface IRefineDataAsyncDelegate<T>
    {
        Task RefineDataAsync(IEnumerable<T> sourceCollection, IStatus status);
    }

    public interface IDataRefinementController<T>:
        IRefineDataAsyncDelegate<T>
    {
        // ...
    }
}
