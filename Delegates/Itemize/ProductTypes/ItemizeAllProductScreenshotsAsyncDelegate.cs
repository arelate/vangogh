using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Itemize.ProductTypes
{
    public class ItemizeAllProductScreenshotsAsyncDelegate: ItemizeAllDataAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate,Delegates")]
        public ItemizeAllProductScreenshotsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getProductScreenshotsAsyncDelegate) : 
            base(getProductScreenshotsAsyncDelegate)
        {
            // ...
        }
    }
}