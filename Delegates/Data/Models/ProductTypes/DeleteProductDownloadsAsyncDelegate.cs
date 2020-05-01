using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteProductDownloadsAsyncDelegate : DeleteAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmProductDownloadsContainIdAsyncDelegate))]
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