using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeMultipleAsyncDelegate<Output>
    {
        Task<IEnumerable<Output>> ItemizeMulitpleAsync(IStatus status);
    }
}
