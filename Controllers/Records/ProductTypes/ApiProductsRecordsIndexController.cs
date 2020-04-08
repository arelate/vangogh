using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class ApiProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.ApiProductsRecordsDataController,Controllers")]
        public ApiProductsRecordsIndexController(
            IDataController<ProductRecords> apiProductsRecordsController) :
            base(
                apiProductsRecordsController)
        {
            // ...
        }
    }
}