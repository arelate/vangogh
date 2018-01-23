using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Interfaces.Controllers.Index
{
    public interface IIndexController<IndexType>:
        IItemizeAllAsyncDelegate<IndexType>,
        ICountAsyncDelegate,
        ICreateAsyncDelegate<IndexType>,
        IDeleteAsyncDelegate<IndexType>,
        IContainsIdAsyncDelegate<IndexType>
    {
        // ...
    }
}
