using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.Data
{
    public class AccountProductsRecordsIndexController : IndexRecordsController
    {
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