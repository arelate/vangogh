using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class UpdatedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
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