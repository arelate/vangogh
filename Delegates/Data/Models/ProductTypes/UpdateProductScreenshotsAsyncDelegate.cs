using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateProductScreenshotsAsyncDelegate: UpdateDataAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(DeleteProductScreenshotsAsyncDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmProductScreenshotsContainIdAsyncDelegate),
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