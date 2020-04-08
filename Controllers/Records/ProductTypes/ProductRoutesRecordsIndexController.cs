using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductRoutesRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductRoutesRecordsDataController,Controllers")]
        public ProductRoutesRecordsIndexController(
            IDataController<ProductRecords> productRoutesRecordsController) :
            base(
                productRoutesRecordsController)
        {
            // ...
        }
    }
}