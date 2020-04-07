using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductDownloadsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductDownloadsRecordsDataController,Controllers")]
        public ProductDownloadsRecordsIndexController(
            IDataController<ProductRecords> productDownloadsRecordsController) :
            base(
                productDownloadsRecordsController)
        {
            // ...
        }
    }
}