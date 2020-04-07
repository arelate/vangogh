using System.Collections.Generic;

using Interfaces.Controllers.Records;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Activities;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class UpdatedDataController : DataController<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListUpdatedDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.UpdatedRecordsIndexController,Controllers",
            "Delegates.Collections.System.FindLongDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public UpdatedDataController(
            IGetDataAsyncDelegate<List<long>> getUpdatedDataFromPathAsyncDelegate,
            IPostDataAsyncDelegate<List<long>> postUpdatedDataToPathAsyncDelegate,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> updatedRecordsIndexController,
            IFindDelegate<long> findLongDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getUpdatedDataFromPathAsyncDelegate,
                postUpdatedDataToPathAsyncDelegate,
                convertPassthroughIndexDelegate,
                updatedRecordsIndexController,
                findLongDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}