using System.Collections.Generic;

namespace Interfaces.Delegates.Itemizations
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