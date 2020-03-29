using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListProductsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetProductsRecordsPathDelegate,Delegates")]
        public GetListProductsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getProductsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}