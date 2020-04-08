using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ValidationResultsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ValidationResultsRecordsDataController,Controllers")]
        public ValidationResultsRecordsIndexController(
            IDataController<ProductRecords> validationResultsRecordsController) :
            base(
                validationResultsRecordsController)
        {
            // ...
        }
    }
}