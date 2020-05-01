using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Data.Models.ProductTypes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmProductDownloadsContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(GetProductDownloadsByIdAsyncDelegate))]
        public ConfirmProductDownloadsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ProductDownloads, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}