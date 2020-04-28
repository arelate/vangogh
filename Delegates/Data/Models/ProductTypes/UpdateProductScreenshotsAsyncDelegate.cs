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
            "Delegates.Data.Models.ProductTypes.DeleteProductScreenshotsAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmProductScreenshotsContainIdAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate,Delegates")]
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