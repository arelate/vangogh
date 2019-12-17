using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.Templates
{
    public class ReportTemplateStashController: StashController<List<Models.Template.Template>>
    {
        [Dependencies(
            "Delegates.GetPath.Json.GetReportTemplatePathDelegate,Delegates",
            Dependencies.JSONSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
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