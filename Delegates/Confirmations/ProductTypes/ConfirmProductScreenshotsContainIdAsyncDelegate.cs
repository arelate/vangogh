using Attributes;
using Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Confirmations.ProductTypes
{
    public class ConfirmProductScreenshotsContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(GetProductScreenshotsByIdAsyncDelegate))]
        public ConfirmProductScreenshotsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ProductScreenshots, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}