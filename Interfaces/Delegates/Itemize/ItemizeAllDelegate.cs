using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeAllAsyncDelegate<Output>
    {
        // TODO: IAsyncEnumerable
        Task<IEnumerable<Output>> ItemizeAllAsync(IStatus status);
    }

    public interface IItemizeAllDelegate<Output>
    {
        IEnumerable<Output> ItemizeAll();
    }

}
