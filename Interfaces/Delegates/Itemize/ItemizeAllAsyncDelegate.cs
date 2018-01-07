using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeAllAsyncDelegate<Output>
    {
        Task<IEnumerable<Output>> ItemizeAllAsync(IStatus status);
    }
}
