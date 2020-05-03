using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        GetListProductRoutesRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(GetListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetProductRoutesRecordsPathDelegate))]
        public GetListProductRoutesRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductRoutesRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getProductRoutesRecordsPathDelegate)
        {
            // ...
        }
    }
}