using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class AccountProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.AccountProductsRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public AccountProductsRecordsIndexController(
            IDataController<ProductRecords> accountProductsRecordsController,
            IStatusController statusController) :
            base(
                accountProductsRecordsController,
                statusController)
        {
            // ...
        }
    }
}