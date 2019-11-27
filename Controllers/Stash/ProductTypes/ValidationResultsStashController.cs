using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ValidationResults;

namespace Controllers.Stash.ProductTypes
{
    public class ValidationResultsStashController :
        StashController<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ValidationResultsStashController(
            IGetPathDelegate getValidationResultsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getValidationResultsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}