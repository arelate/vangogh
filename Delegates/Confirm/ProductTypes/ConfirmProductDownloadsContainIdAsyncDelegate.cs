using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmProductDownloadsContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetProductDownloadsByIdAsyncDelegate,Delegates")]
        public ConfirmProductDownloadsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ProductDownloads, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}