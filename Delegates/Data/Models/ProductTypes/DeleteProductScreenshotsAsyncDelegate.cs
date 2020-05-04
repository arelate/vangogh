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
    public class DeleteProductScreenshotsAsyncDelegate : DeleteAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate),
            typeof(ConvertProductScreenshotsToIndexDelegate),
            typeof(ConfirmProductScreenshotsContainIdAsyncDelegate))]
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