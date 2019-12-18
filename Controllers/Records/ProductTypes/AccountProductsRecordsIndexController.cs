using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class AccountProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.AccountProductsRecordsDataController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public AccountProductsRecordsIndexController(
            IDataController<ProductRecords> accountProductsRecordsController,
            IActionLogController actionLogController) :
            base(
                accountProductsRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}