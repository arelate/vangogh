using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;
using Interfaces.Delegates.Find;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class UpdatedDataController : DataController<long>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.ProductTypes.PostListUpdatedDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.UpdatedRecordsIndexController,Controllers",
            "Delegates.Find.System.FindLongDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdatedDataController(
            IGetDataAsyncDelegate<List<long>> getUpdatedDataFromPathAsyncDelegate,
            IPostDataAsyncDelegate<List<long>> postUpdatedDataToPathAsyncDelegate,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> updatedRecordsIndexController,
            IFindDelegate<long> findLongDelegate,
            IActionLogController actionLogController) :
            base(
                getUpdatedDataFromPathAsyncDelegate,
                postUpdatedDataToPathAsyncDelegate,
                convertPassthroughIndexDelegate,
                updatedRecordsIndexController,
                findLongDelegate,
                actionLogController)
        {
            // ...
        }
    }
}