using System;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.SerializedStorage;

using Interfaces.Controllers.Stash;

using Interfaces.Status;
using System.Threading.Tasks;

namespace Controllers.Stash
{
    public class StashController<InterfaceType, InstanceType> :
        IStashController<InterfaceType, InstanceType>
        where InstanceType : InterfaceType, new()
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private ISerializedStorageController serializedStorageController;

        private IStatusController statusController;

        private InterfaceType storedData;

        public StashController(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.serializedStorageController = serializedStorageController;

            this.statusController = statusController;
        }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task<InterfaceType> GetDataAsync(IStatus status)
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

            storedData = await serializedStorageController.DeserializePullAsync<InstanceType>(storedDataUri, loadStatus);
            if (storedData == null) storedData = new InstanceType();

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

            await serializedStorageController.SerializePushAsync(storedDataUri, storedData, saveStatus);

            await statusController.CompleteAsync(saveStatus);
        }
    }
}
