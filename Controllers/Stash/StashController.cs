using System;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Serialization;
using Interfaces.Storage;

using Interfaces.Controllers.Stash;

using Interfaces.Status;

namespace Controllers.Stash
{
    public class StashController<ModelType>: IStashController<ModelType> where ModelType : new()
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private ISerializationController<string> serializationController;
        private IStorageController<string> storageController;

        private IStatusController statusController;

        private ModelType storedData;

        public StashController(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            IStatusController statusController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.serializationController = serializationController;
            this.storageController = storageController;

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

            var storedDataUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(string.Empty),
                getFilenameDelegate.GetFilename());

            var serializedData = await storageController.PullAsync(storedDataUri);
            storedData = serializationController.Deserialize<ModelType>(serializedData);

            if (storedData == null) storedData = new ModelType();

            DataAvailable = true;

            await statusController.CompleteAsync(loadStatus);
        }

        public async Task SaveAsync(IStatus status)
        {
            if (!DataAvailable) throw new InvalidOperationException("Cannot save data before it's available");

            var saveStatus = await statusController.CreateAsync(status, "Save stored data", false);

            var storedDataUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(string.Empty),
                getFilenameDelegate.GetFilename());

            var serializedData = serializationController.Serialize(storedData);
            await storageController.PushAsync(storedDataUri, serializedData);

            await statusController.CompleteAsync(saveStatus);
        }
    }
}
