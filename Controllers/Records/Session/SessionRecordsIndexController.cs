using Interfaces.Controllers.Data;
using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Records.Session
{
    public class SessionRecordsIndexController : IndexRecordsController
    {
        [Dependencies(
            "Controllers.Data.Records.SessionRecordsDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public SessionRecordsIndexController(
            IDataController<ProductRecords> sessionRecordsController,
            IStatusController statusController) :
            base(
                sessionRecordsController,
                statusController)
        {
            // ...
        }
    }
}