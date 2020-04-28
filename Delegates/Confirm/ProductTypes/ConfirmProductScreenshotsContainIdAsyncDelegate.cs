using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmProductScreenshotsContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetProductScreenshotsByIdAsyncDelegate,Delegates")]
        public ConfirmProductScreenshotsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ProductScreenshots, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}