using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;

namespace Interfaces.Controllers.Index
{
    public interface IIndexController<IndexType>:
        IDataAvailableDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate,
        IItemizeAllAsyncDelegate<IndexType>,
        ICountAsyncDelegate,
        IUpdateAsyncDelegate<IndexType>,
        IRemoveAsyncDelegate<IndexType>,
        IContainsIdAsyncDelegate<IndexType>,
        IGetLastModifiedAsyncDelegate<IndexType>,
        IItemizeAsyncDelegate<DateTime, IndexType> // all items modified on or after a certain date
    {
        // ...
    }
}
