using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductScreenshotsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductScreenshotsRecordsDataController,Controllers")]
        public ProductScreenshotsRecordsIndexController(
            IDataController<ProductRecords> productScreenshotsRecordsController) :
            base(
                productScreenshotsRecordsController)
        {
            // ...
        }
    }
}