using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetProductScreenshotsByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindProductScreenshotsDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate))]
        public GetProductScreenshotsByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getDataCollectionAsyncDelegate, 
            IFindDelegate<ProductScreenshots> findDelegate, 
            IConvertDelegate<ProductScreenshots, long> convertProductToIndexDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}