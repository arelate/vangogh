using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class UpdatedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.UpdatedRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public UpdatedRecordsIndexController(
            IDataController<ProductRecords> updatedRecordsController,
            IStatusController statusController) :
            base(
                updatedRecordsController,
                statusController)
        {
            // ...
        }
    }
}