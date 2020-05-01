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
            typeof(Delegates.Data.Models.ProductTypes.DeleteProductDownloadsAsyncDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmProductDownloadsContainIdAsyncDelegate),
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