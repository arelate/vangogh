using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Data.Storage.ProductTypes;

namespace Delegates.Itemize.ProductTypes
{
    public class ItemizeAllUpdatedAsyncDelegate: ItemizeAllDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(GetListUpdatedDataFromPathAsyncDelegate))]
        public ItemizeAllUpdatedAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getDataCollectionAsyncDelegate) : base(getDataCollectionAsyncDelegate)
        {
            // ...
        }
    }
}