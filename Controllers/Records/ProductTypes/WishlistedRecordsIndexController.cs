using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.ProductTypes
{
    public class WishlistedRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
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