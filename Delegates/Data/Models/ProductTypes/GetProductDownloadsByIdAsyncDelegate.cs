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
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductDownloadsDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates")]
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