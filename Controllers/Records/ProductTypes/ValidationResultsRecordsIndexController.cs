using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class ValidationResultsRecordsIndexController : IndexRecordsController
    {
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