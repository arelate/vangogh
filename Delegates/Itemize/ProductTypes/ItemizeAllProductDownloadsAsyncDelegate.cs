using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Data.Storage.ProductTypes;

namespace Delegates.Itemize.ProductTypes
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