using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Data.Storage.ProductTypes;

namespace Delegates.Itemize.ProductTypes
{
    public class ItemizeAllProductScreenshotsAsyncDelegate: ItemizeAllDataAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(GetListProductScreenshotsDataFromPathAsyncDelegate))]
        public ItemizeAllProductScreenshotsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getProductScreenshotsAsyncDelegate) : 
            base(getProductScreenshotsAsyncDelegate)
        {
            // ...
        }
    }
}