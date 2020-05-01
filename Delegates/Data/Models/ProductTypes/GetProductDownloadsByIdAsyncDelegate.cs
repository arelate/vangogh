using System.Collections.Generic;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class GetProductDownloadsByIdAsyncDelegate: 
        GetDataByIdAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate),
            typeof(Delegates.Collections.ProductTypes.FindProductDownloadsDelegate),
            typeof(Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate))]
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