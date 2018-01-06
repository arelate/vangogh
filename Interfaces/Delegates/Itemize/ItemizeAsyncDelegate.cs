using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeAsyncDelegate<Input, Output>
    {
        Task<IEnumerable<Output>> ItemizeAsync(Input item, IStatus status);
    }
}
