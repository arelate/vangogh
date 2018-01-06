using System.Collections.Generic;

namespace Interfaces.Delegates.Itemize
{
    public interface IItemizeDelegate<Input, Output>
    {
        IEnumerable<Output> Itemize(Input item);
    }
}
