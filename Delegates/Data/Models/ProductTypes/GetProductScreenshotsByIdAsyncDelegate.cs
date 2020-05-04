using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetProductScreenshotsByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindProductScreenshotsDelegate),
            typeof(ConvertProductScreenshotsToIndexDelegate))]
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