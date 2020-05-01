using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Data.Models.ProductTypes;

namespace Delegates.Confirm.ProductTypes
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