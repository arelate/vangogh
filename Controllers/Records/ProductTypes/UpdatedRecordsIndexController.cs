using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;


using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class UpdatedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.UpdatedRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdatedRecordsIndexController(
            IDataController<ProductRecords> updatedRecordsController,
            IActionLogController actionLogController) :
            base(
                updatedRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}