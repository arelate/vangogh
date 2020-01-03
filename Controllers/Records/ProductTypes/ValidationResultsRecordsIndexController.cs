using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ValidationResultsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Data.Records.ValidationResultsRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsRecordsIndexController(
            IDataController<ProductRecords> validationResultsRecordsController,
            IActionLogController actionLogController) :
            base(
                validationResultsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}