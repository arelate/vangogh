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
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmProductDownloadsContainIdAsyncDelegate,Delegates")]
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