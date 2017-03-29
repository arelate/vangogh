using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.DataRefinement
{
    public interface IRefineDataDelegate<T>
    {
        Task RefineData(IEnumerable<T> sourceCollection, IStatus status);
    }

    public interface IDataRefinementController<T>:
        IRefineDataDelegate<T>
    {
        // ...
    }
}
