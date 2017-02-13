using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.ForEachAsync;

namespace Controllers.ForEachAsync
{
    public class ForEachAsyncDelegate : IForEachAsyncDelegate
    {
        public Task ForEachAsync<T>(IEnumerable<T> collection, Func<T, Task> operation)
        {
            return Task.WhenAll(collection.Select(operation));
        }
    }
}
