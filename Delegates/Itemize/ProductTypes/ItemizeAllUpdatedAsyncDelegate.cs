using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Itemize.ProductTypes
{
    public class ItemizeAllUpdatedAsyncDelegate: ItemizeAllDataAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate,Delegates")]
        public ItemizeAllUpdatedAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getDataCollectionAsyncDelegate) : base(getDataCollectionAsyncDelegate)
        {
            // ...
        }
    }
}