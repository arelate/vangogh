using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ArgsDefinitions;
using Models.Dependencies;

namespace Controllers.Stash.ArgsDefinitions
{
    public class ArgsDefinitionsStashController: StashController<ArgsDefinition>
    {
        [Dependencies(
            DependencyContext.Default,
            "Delegates.GetPath.ArgsDefinitions.GetArgsDefinitionsPathDelegate,Delegates",
            Dependencies.JSONSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
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