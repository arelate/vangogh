using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.ForEachAsync
{
    public interface IForEachAsyncDelegate
    {
        Task ForEachAsync<T>(IEnumerable<T> collection, Func<T, Task> operation);
    }
}
