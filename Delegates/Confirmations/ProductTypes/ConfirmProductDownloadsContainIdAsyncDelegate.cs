using Attributes;
using Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Confirmations.ProductTypes
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