using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateProductDownloadsAsyncDelegate: UpdateDataAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.DeleteProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmProductDownloadsContainIdAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates")]
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