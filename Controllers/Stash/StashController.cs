using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.GetPath;


namespace Controllers.Stash
{
    public abstract class StashController<ModelType>: IStashController<ModelType> where ModelType : class, new()
    {
        readonly IGetPathDelegate getPathDelegate;
        readonly ISerializedStorageController serializedStorageController;
        readonly IActionLogController actionLogController;

        ModelType storedData;

        public StashController(
            IGetPathDelegate getPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController)
        {
            this.getPathDelegate = getPathDelegate;
            this.serializedStorageController = serializedStorageController;
            this.actionLogController = actionLogController;
        }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task<ModelType> GetDataAsync()
        {
            if (!DataAvailable) await LoadAsync();

            return storedData;
        }

        public async Task LoadAsync()
        {
            actionLogController.StartAction("Load stored data");

            var storedDataUri = getPathDelegate.GetPath(string.Empty, string.Empty);

            storedData = await serializedStorageController.DeserializePullAsync<ModelType>(storedDataUri);

            if (storedData == null) storedData = new ModelType();

            DataAvailable = true;

            actionLogController.CompleteAction();
        }

        public async Task SaveAsync()
        {
            if (!DataAvailable) {
                // TODO: Replce statusController warning
                // await statusController.WarnAsync(status, 
                //     "Attempted to save stashed data that has not been made available");
                return;
            };

            actionLogController.StartAction("Save stored data");

            var storedDataUri = getPathDelegate.GetPath(string.Empty, string.Empty);

            await serializedStorageController.SerializePushAsync(storedDataUri, storedData);

            actionLogController.CompleteAction();
        }
    }
}
