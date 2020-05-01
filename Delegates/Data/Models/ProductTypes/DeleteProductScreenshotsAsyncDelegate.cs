using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteProductScreenshotsAsyncDelegate : DeleteAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate),
            typeof(Delegates.Confirm.ProductTypes.ConfirmProductScreenshotsContainIdAsyncDelegate))]
        public DeleteProductScreenshotsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getDataCollectionAsyncDelegate,
            IConvertDelegate<ProductScreenshots, long> convertProductToIndexDelegate,
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