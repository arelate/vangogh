using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        PostListValidationResultsRecordsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(PostListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetValidationResultsRecordsPathDelegate))]
        public PostListValidationResultsRecordsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRecords>> postListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getValidationResultsRecordsPathDelegate) :
            base(
                postListProductRecordsDataAsyncDelegate,
                getValidationResultsRecordsPathDelegate)
        {
            // ...
        }
    }
}