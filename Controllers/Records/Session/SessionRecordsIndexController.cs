using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.Session
{
    public class SessionRecordsIndexController : IndexRecordsController
    {
        public SessionRecordsIndexController(
            IDataController<ProductRecords> productRecordsController,
            IStatusController statusController) :
            base(
                productRecordsController,
                statusController)
        {
            // ...
        }
    }
}