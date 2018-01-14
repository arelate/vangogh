using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.Index;
using Interfaces.Controllers.Collection;

using Interfaces.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Index
{
    public class IndexDataController : IIndexController<long>
    {
        private ICollectionController collectionController;

        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private ISerializedStorageController serializedStorageController;
        private IStatusController statusController;

        private IDictionary<long, DateTime> indexesCreated;

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

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexesCreated.ContainsKey(id);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexesCreated.Count;
        }

        public async Task<IEnumerable<long>> ItemizeAllAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexesCreated.Keys;
        }

        public async Task LoadAsync(IStatus status)
        {
            var loadStatus = await statusController.CreateAsync(status, "Load index");

            var indexUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            indexesCreated = await serializedStorageController.DeserializePullAsync<Dictionary<long, DateTime>>(indexUri, loadStatus);
            if (indexesCreated == null) indexesCreated = new Dictionary<long, DateTime>();

            DataAvailable = true;

            await statusController.CompleteAsync(loadStatus);
        }

        public async Task SaveAsync(IStatus status)
        {
            if (!DataAvailable) throw new InvalidOperationException("Cannot save data before it's available");

            var saveStatus = await statusController.CreateAsync(status, "Save index");

            var indexUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            await serializedStorageController.SerializePushAsync(indexUri, indexesCreated, saveStatus);

            await statusController.CompleteAsync(saveStatus);
        }

        private async Task Map(IStatus status, string taskMessage, Func<long, bool> itemAction, params long[] data)
        {
            if (!DataAvailable) await LoadAsync(status);

            var task = await statusController.CreateAsync(status, taskMessage);
            var counter = 0;
            var dataChanged = false;

            foreach (var item in data)
            {
                await statusController.UpdateProgressAsync(
                    task,
                    ++counter,
                    data.Length,
                    item.ToString());

                // do this for every item
                if (itemAction(item)) dataChanged = true;
            }

            if (dataChanged)
            {
                var saveDataTask = await statusController.CreateAsync(task, "Save modified index");
                await SaveAsync(status);
                await statusController.CompleteAsync(saveDataTask);
            }

            await statusController.CompleteAsync(task);
        }

        public async Task RemoveAsync(IStatus status, params long[] data)
        {
            if (!DataAvailable) await LoadAsync(status);

            await Map(
                status,
                "Remove index item(s)",
                (item) =>
                {
                    if (indexesCreated.ContainsKey(item))
                    {
                        indexesCreated.Remove(item);
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
                    if (!indexesCreated.ContainsKey(item))
                    {
                        // TODO: need to understand if we should update last modified even if the item exists
                        indexesCreated.Add(item, DateTime.UtcNow);
                        return true;
                    }
                    return false;
                },
                data);
        }

        public async Task<DateTime> GetCreatedAsync(long id, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexesCreated.ContainsKey(id) ?
                indexesCreated[id] :
                DateTime.MinValue.ToUniversalTime();
        }

        public async Task<IEnumerable<long>> ItemizeAsync(DateTime moment, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return indexesCreated.Where(idLastModified => { return idLastModified.Value >= moment; }).Select(idLastModified => idLastModified.Key);
        }

        public async Task Recreate(IStatus status, params long[] data)
        {
            await RemoveAsync(status, data);
            await UpdateAsync(status, data);
        }
    }
}
