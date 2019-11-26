using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.ProductTypes
{
    public class UpdatedRecordsIndexController : IndexRecordsController
    {
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