using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Confirmations.ProductTypes;
using Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateProductDownloadsAsyncDelegate: UpdateDataAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(DeleteProductDownloadsAsyncDelegate),
            typeof(ConvertProductDownloadsToIndexDelegate),
            typeof(ConfirmProductDownloadsContainIdAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate))]
        public UpdateProductDownloadsAsyncDelegate(
            IDeleteAsyncDelegate<ProductDownloads> deleteAsyncDelegate, 
            IConvertDelegate<ProductDownloads, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getListProductDownloadsAsyncDelegate) : 
            base(
                deleteAsyncDelegate,
                convertProductToIndexDelegate, 
                confirmDataContainsIdAsyncDelegate, 
                getListProductDownloadsAsyncDelegate)
        {
            // ...
        }
    }
}