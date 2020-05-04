using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage.ProductTypes;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Itemizations.ProductTypes
{
    public class ItemizeAllProductDownloadsAsyncDelegate: ItemizeAllDataAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(GetListProductDownloadsDataFromPathAsyncDelegate))]
        public ItemizeAllProductDownloadsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getProductDownloadsAsyncDelegate) : 
            base(getProductDownloadsAsyncDelegate)
        {
            // ...
        }
    }
}