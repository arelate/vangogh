using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;

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
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ArgsDefinitionsStashController(
            IGetPathDelegate getPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController):
            base(
                getPathDelegate, 
                serializedStorageController, 
                actionLogController)
        {
            // ...
        }
    }
}