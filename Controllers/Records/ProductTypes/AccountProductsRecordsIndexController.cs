using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class AccountProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.AccountProductsRecordsDataController,Controllers")]
        public AccountProductsRecordsIndexController(
            IDataController<ProductRecords> accountProductsRecordsController) :
            base(
                accountProductsRecordsController)
        {
            // ...
        }
    }
}