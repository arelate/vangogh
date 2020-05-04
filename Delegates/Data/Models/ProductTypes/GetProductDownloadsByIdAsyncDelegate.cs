using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetProductDownloadsByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindProductDownloadsDelegate),
            typeof(ConvertProductDownloadsToIndexDelegate))]
        public GetProductDownloadsByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getDataCollectionAsyncDelegate, 
            IFindDelegate<ProductDownloads> findDelegate, 
            IConvertDelegate<ProductDownloads, long> convertProductToIndexDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}