using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace Interfaces.DataRefinement
{
    public interface IRefineDataDelegate<T>
    {
        Task RefineData(IEnumerable<T> sourceCollection, ITaskStatus taskStatus);
    }

    public interface IDataRefinementController<T>:
        IRefineDataDelegate<T>
    {
        // ...
    }
}
