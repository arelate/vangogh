using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class AccountProductsRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Data.Records.AccountProductsRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
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