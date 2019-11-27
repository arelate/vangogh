using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

using Attributes;

using Models.ArgsDefinitions;

namespace Controllers.Stash.ArgsDefinitions
{
    public class ArgsDefinitionsStashController: StashController<ArgsDefinition>
    {
        [Dependencies(
            "Delegates.GetPath.ArgsDefinitions.GetArgsDefinitionsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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