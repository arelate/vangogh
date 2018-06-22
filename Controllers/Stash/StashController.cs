using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;
// using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

namespace Controllers.Stash
{
    public class StashController<ModelType>: IStashController<ModelType> where ModelType : class, new()
    {
        readonly IGetPathDelegate getPathDelegate;

        readonly ISerializedStorageController serializedStorageController;
        // readonly ISerializationController<string> serializationController;
        // readonly IStorageController<string> storageController;

        readonly IStatusController statusController;

        ModelType storedData;

        public StashController(
            IGetPathDelegate getPathDelegate,
            // ISerializationController<string> serializationController,
            // IStorageController<string> storageController,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController)
        {
            this.getPathDelegate = getPathDelegate;

            this.serializedStorageController = serializedStorageController;
            // this.serializationController = serializationController;
            // this.storageController = storageController;

            this.statusController = statusController;
        }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task<ModelType> GetDataAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return storedData;
        }

        public async Task LoadAsync(IStatus status)
        {
            var loadStatus = await statusController.CreateAsync(status, "Load stored data", false);

            var storedDataUri = getPathDelegate.GetPath(string.Empty, string.Empty);

            // var serializedData = await storageController.PullAsync(storedDataUri);
            // if (!string.IsNullOrEmpty(serializedData)) 
            //     storedData = serializationController.Deserialize<ModelType>(serializedData);
            storedData = await serializedStorageController.DeserializePullAsync<ModelType>(storedDataUri, loadStatus);

            if (storedData == null) storedData = new ModelType();

            DataAvailable = true;

            await statusController.CompleteAsync(loadStatus, false);
        }

        public async Task SaveAsync(IStatus status)
        {
            if (!DataAvailable) {
                await statusController.WarnAsync(status, 
                    "Attempted to save stashed data that has not been made available");
                return;
            };

            var saveStatus = await statusController.CreateAsync(status, "Save stored data", false);

            var storedDataUri = getPathDelegate.GetPath(string.Empty, string.Empty);

            await serializedStorageController.SerializePushAsync(storedDataUri, storedData, saveStatus);
            // var serializedData = serializationController.Serialize(storedData);
            // await storageController.PushAsync(storedDataUri, serializedData);

            await statusController.CompleteAsync(saveStatus, false);
        }
    }
}
