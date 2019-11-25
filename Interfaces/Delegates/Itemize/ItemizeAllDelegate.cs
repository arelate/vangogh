using System.Collections.Generic;

using Interfaces.Status;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeAllAsyncDelegate<Output>
    {
        IAsyncEnumerable<Output> ItemizeAllAsync(IStatus status);
    }

    public interface IItemizeAllDelegate<Output>
    {
        IEnumerable<Output> ItemizeAll();
    }

}
