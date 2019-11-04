using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Stash.Templates
{
    public class AppTemplateStashController: StashController<List<Models.Template.Template>>
    {
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