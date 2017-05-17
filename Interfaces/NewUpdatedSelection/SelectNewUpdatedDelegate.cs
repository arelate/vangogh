using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.NewUpdatedSelection
{
    public interface ISelectNewUpdatedDelegate<T>
    {
        Task SelectNewUpdatedAsync(IEnumerable<T> collection, IStatus status);
    }
}
