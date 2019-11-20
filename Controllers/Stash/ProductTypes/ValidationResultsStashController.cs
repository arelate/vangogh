using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.ValidationResults;

namespace Controllers.Stash.ProductTypes
{
    public class ValidationResultsStashController :
        StashController<Dictionary<long, ValidationResults>>
    {
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