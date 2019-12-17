using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ValidationResultsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ValidationResultsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ValidationResultsRecordsIndexController(
            IDataController<ProductRecords> validationResultsRecordsController,
            IStatusController statusController) :
            base(
                validationResultsRecordsController,
                statusController)
        {
            // ...
        }
    }
}