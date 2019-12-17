using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.Templates
{
    public class AppTemplateStashController: StashController<List<Models.Template.Template>>
    {
        [Dependencies(
            "Delegates.GetPath.Json.GetAppTemplatePathDelegate,Delegates",
            Dependencies.JSONSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
        public AppTemplateStashController(
            IGetPathDelegate getAppTemplatePathDelegate,
            ISerializedStorageController jsonSerializedStorageController,
            IStatusController statusController):
            base(
                getAppTemplatePathDelegate,
                jsonSerializedStorageController,
                statusController)
        {
            // ...
        }
    }
}