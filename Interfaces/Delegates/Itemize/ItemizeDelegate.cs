using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeDelegate<Input, Output>
    {
        IEnumerable<Output> Itemize(Input item);
    }

    public interface IItemizeAsyncDelegate<Input, Output>
    {
        // TODO: IAsyncEnumerable
        Task<IEnumerable<Output>> ItemizeAsync(Input item);
    }    
}
