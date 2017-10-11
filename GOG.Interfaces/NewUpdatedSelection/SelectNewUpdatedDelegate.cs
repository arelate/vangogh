using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace GOG.Interfaces.NewUpdatedSelection
{
    public interface ISelectNewUpdatedAsyncDelegate<T>
    {
        Task SelectNewUpdatedAsync(IEnumerable<T> collection, IStatus status);
    }
}
