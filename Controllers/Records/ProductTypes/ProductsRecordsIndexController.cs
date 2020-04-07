using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ProductsRecordsDataController,Controllers")]
        public ProductsRecordsIndexController(
            IDataController<ProductRecords> productsRecordsController) :
            base(
                productsRecordsController)
        {
            // ...
        }
    }
}