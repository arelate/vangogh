using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Itemize.ProductTypes
{
    public class ItemizeAllProductDownloadsAsyncDelegate: ItemizeAllDataAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates")]
        public ItemizeAllProductDownloadsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getProductDownloadsAsyncDelegate) : 
            base(getProductDownloadsAsyncDelegate)
        {
            // ...
        }
    }
}