using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Status;

using Attributes;

using Models.ArgsDefinitions;
using Models.Dependencies;

namespace Controllers.Stash.ArgsDefinitions
{
    public class ArgsDefinitionsStashController: StashController<ArgsDefinition>
    {
        [Dependencies(
            "Delegates.GetPath.ArgsDefinitions.GetArgsDefinitionsPathDelegate,Delegates",
            Dependencies.JSONSerializedStorageController,
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