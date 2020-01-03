using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.Session
{
    public class SessionRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            DependencyContext.Default,
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