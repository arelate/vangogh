using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class UpdatedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.UpdatedRecordsDataController,Controllers")]
        public UpdatedRecordsIndexController(
            IDataController<ProductRecords> updatedRecordsController) :
            base(
                updatedRecordsController)
        {
            // ...
        }
    }
}