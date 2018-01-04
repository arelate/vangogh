using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Collection;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Data
{
    public class IndexDataController : IDataController<long>
    {
        private ICollectionController collectionController;

        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private ISerializedStorageController serializedStorageController;
        private IStatusController statusController;

        private IList<long> indexes;

        public IndexDataController(
            ICollectionController collectionController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController)
        {
            this.collectionController = collectionController;

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

        public async Task<bool> ContainsAsync(long data, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexes.Contains(data);
        }

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexes.Contains(id);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexes.Count;
        }

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexes;
        }

        public Task<long> GetByIdAsync(long id, IStatus status)
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync(IStatus status)
        {
            var loadStatus = statusController.Create(status, "Load index");

            var indexUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            indexes = await serializedStorageController.DeserializePullAsync<List<long>>(indexUri, loadStatus);
            if (indexes == null) indexes = new List<long>();

            DataAvailable = true;

            statusController.Complete(loadStatus);
        }

        public async Task SaveAsync(IStatus status)
        {
            if (!DataAvailable) throw new InvalidOperationException("Cannot save data before it's available");

            var saveStatus = statusController.Create(status, "Save index");

            var indexUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            await serializedStorageController.SerializePushAsync(indexUri, indexes, saveStatus);

            statusController.Complete(saveStatus);
        }

        private async Task Map(IStatus status, string taskMessage, Func<long, bool> itemAction, params long[] data)
        {
            if (!DataAvailable) await LoadAsync(status);

            var task = statusController.Create(status, taskMessage);
            var counter = 0;
            var dataChanged = false;

            foreach (var item in data)
            {
                statusController.UpdateProgress(
                    task,
                    ++counter,
                    data.Length,
                    item.ToString());

                // do this for every item
                if (itemAction(item)) dataChanged = true;
            }

            if (dataChanged)
            {
                var saveDataTask = statusController.Create(task, "Save modified index");
                await SaveAsync(status);
                statusController.Complete(saveDataTask);
            }

            statusController.Complete(task);
        }

        public async Task RemoveAsync(IStatus status, params long[] data)
        {
            if (!DataAvailable) await LoadAsync(status);

            await Map(
                status,
                "Remove index item(s)",
                (item) =>
                {
                    if (indexes.Contains(item))
                    {
                        indexes.Remove(item);
                        return true;
                    }
                    return false;
                },
                data);
        }

        public async Task UpdateAsync(IStatus status, params long[] data)
        {
            if (!DataAvailable) await LoadAsync(status);

            await Map(
                status,
                "Update index item(s)",
                (item) => {
                    if (!indexes.Contains(item))
                    {
                        indexes.Add(item);
                        return true;
                    }
                    return false;
                },
                data);
        }
    }
}
