using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Interfaces.Controllers.Index
{
    public interface IRecreateAsyncDelegate<Type>
    {
        Task Recreate(IStatus status, params Type[] data);
    }

    public interface IIndexController<IndexType>:
        IDataAvailableDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate,
        IItemizeAllAsyncDelegate<IndexType>,
        ICountAsyncDelegate,
        IUpdateAsyncDelegate<IndexType>,
        IRemoveAsyncDelegate<IndexType>,
        IRecreateAsyncDelegate<IndexType>,
        IContainsIdAsyncDelegate<IndexType>,
        IGetCreatedAsyncDelegate<IndexType>,
        IItemizeAsyncDelegate<DateTime, IndexType> // all items modified on or after a certain date
    {
        // ...
    }
}
