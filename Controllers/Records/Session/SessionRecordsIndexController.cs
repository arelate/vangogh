using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.Session
{
    public class SessionRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.SessionRecordsDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public SessionRecordsIndexController(
            IDataController<ProductRecords> sessionRecordsController,
            IActionLogController actionLogController) :
            base(
                sessionRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}