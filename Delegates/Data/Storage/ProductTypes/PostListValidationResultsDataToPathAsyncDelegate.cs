using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        PostListValidationResultsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.PostListValidationResultsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates")]
        public PostListValidationResultsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ValidationResults>> postListValidationResultsDataAsyncDelegate,
            IGetPathDelegate getListValidationResultsPathDelegate) :
            base(
                postListValidationResultsDataAsyncDelegate,
                getListValidationResultsPathDelegate)
        {
            // ...
        }
    }
}