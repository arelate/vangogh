using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

using Models.ArgsDefinitions;

namespace Controllers.Stash.ArgsDefinitions
{
    public class ArgsDefinitionsStashController: StashController<ArgsDefinition>
    {
        public ArgsDefinitionsStashController(
            IGetPathDelegate getPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController):
            base(getPathDelegate, serializedStorageController, statusController)
        {
            // ...
        }
    }
}