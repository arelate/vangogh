using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class WishlistedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.WishlistedRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedRecordsIndexController(
            IDataController<ProductRecords> wishlistedRecordsController,
            IActionLogController actionLogController) :
            base(
                wishlistedRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}