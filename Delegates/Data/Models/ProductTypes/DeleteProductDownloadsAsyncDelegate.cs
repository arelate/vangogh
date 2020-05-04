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
    public class DeleteProductDownloadsAsyncDelegate : DeleteAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate),
            typeof(ConvertProductDownloadsToIndexDelegate),
            typeof(ConfirmProductDownloadsContainIdAsyncDelegate))]
        public DeleteProductDownloadsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getDataCollectionAsyncDelegate,
            IConvertDelegate<ProductDownloads, long> convertProductToIndexDelegate,
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate) :
            base(
                getDataCollectionAsyncDelegate,
                convertProductToIndexDelegate,
                confirmContainsAsyncDelegate)
        {
            // ...
        }
    }
}