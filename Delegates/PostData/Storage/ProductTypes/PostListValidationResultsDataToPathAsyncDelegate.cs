using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostListValidationResultsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostListValidationResultsDataAsyncDelegate,Delegates",
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