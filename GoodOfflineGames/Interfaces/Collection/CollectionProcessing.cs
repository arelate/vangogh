using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace Interfaces.Collection
{
    public interface IProcessDelegate<T>
    {
        Task Process(IEnumerable<T> collection, ITaskStatus taskStatus);
    }

    public interface ICollectionProcessingController<T>:
        IProcessDelegate<T>
    {
        // ...
    }
}
