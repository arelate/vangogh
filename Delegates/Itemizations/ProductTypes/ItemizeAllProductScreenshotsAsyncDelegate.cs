using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage.ProductTypes;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Itemizations.ProductTypes
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