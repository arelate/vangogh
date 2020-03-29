using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListAccountProductsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetAccountProductsRecordsPathDelegate,Delegates")]
        public GetListAccountProductsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getAccountProductsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getAccountProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}