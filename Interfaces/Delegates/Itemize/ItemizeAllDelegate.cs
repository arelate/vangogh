using System.Collections.Generic;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeAllAsyncDelegate<Output>
    {
        IAsyncEnumerable<Output> ItemizeAllAsync();
    }

    public interface IItemizeAllDelegate<Output>
    {
        IEnumerable<Output> ItemizeAll();
    }

}
