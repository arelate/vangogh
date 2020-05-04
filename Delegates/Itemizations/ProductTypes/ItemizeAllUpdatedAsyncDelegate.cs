using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage.ProductTypes;
using Interfaces.Delegates.Data;

namespace Delegates.Itemizations.ProductTypes
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