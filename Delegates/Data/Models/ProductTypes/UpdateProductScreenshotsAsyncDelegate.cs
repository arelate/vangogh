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
    public class UpdateProductScreenshotsAsyncDelegate: UpdateDataAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(DeleteProductScreenshotsAsyncDelegate),
            typeof(ConvertProductScreenshotsToIndexDelegate),
            typeof(ConfirmProductScreenshotsContainIdAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate))]
        public UpdateProductScreenshotsAsyncDelegate(
            IDeleteAsyncDelegate<ProductScreenshots> deleteAsyncDelegate, 
            IConvertDelegate<ProductScreenshots, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getListProductScreenshotsAsyncDelegate) : 
            base(
                deleteAsyncDelegate,
                convertProductToIndexDelegate, 
                confirmDataContainsIdAsyncDelegate, 
                getListProductScreenshotsAsyncDelegate)
        {
            // ...
        }
    }
}