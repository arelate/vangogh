using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Stash.Templates
{
    public class ReportTemplateStashController: StashController<List<Models.Template.Template>>
    {
        public ReportTemplateStashController(
            IGetPathDelegate getReportTemplatePathDelegate,
            ISerializedStorageController jsonSerializedStorageController,
            IStatusController statusController):
            base(
                getReportTemplatePathDelegate,
                jsonSerializedStorageController,
                statusController)
        {
            // ...
        }
    }
}