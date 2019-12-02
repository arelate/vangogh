using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ValidationResults;
using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class ValidationResultsStashController :
        StashController<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
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