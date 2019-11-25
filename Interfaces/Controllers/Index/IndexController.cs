using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

namespace Interfaces.Controllers.Index
{
    public interface IIndexController<IndexType>:
        IItemizeAllAsyncDelegate<IndexType>,
        ICountAsyncDelegate,
        ICreateAsyncDelegate<IndexType>,
        IDeleteAsyncDelegate<IndexType>,
        IContainsIdAsyncDelegate<IndexType>,
        ICommitAsyncDelegate
    {
        // ...
    }
}
