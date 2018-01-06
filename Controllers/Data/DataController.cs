using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Collection;
using Interfaces.Indexing;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.RecycleBin;
using Interfaces.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Data
{
    public class DataController<Type> : IDataController<Type>
    {
        private IDataController<long> indexDataController;

        private ISerializedStorageController serializedStorageController;

        private IIndexingController indexingController;
        private ICollectionController collectionController;

        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private IRecycleBinController recycleBinController;

        private IStatusController statusController;

        public DataController(
            IDataController<long> indexDataController,
            ISerializedStorageController serializedStorageController,
            IIndexingController indexingController,
            ICollectionController collectionController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecycleBinController recycleBinController,
            IStatusController statusController)
        {
            this.indexDataController = indexDataController;

            this.serializedStorageController = serializedStorageController;

            this.indexingController = indexingController;
            this.collectionController = collectionController;

            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.recycleBinController = recycleBinController;

            this.statusController = statusController;
        }

        public bool DataAvailable
        {
            get { return true; } // dataController doesn't hold any state, always reads from disk, so the data is always available
        }

        public async Task<bool> ContainsAsync(Type data, IStatus status)
        {
            if (data == null) return true;

            var index = indexingController.GetIndex(data);
            return await indexDataController.ContainsAsync(index, status);
        }

        private string GetItemUri(long id)
        {
            return Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename(id.ToString()));
        }

        public async Task<Type> GetByIdAsync(long id, IStatus status)
        {
            return await serializedStorageController.DeserializePullAsync<Type>(GetItemUri(id), status);
        }

        public async Task LoadAsync(IStatus status)
        {
            var loadStatus = await statusController.CreateAsync(status, "Load data");

            await indexDataController.LoadAsync(loadStatus);

            await statusController.CompleteAsync(loadStatus);
        }

        public Task SaveAsync(IStatus status = null)
        {
            throw new NotImplementedException();
        }

        private async Task MapItemsAndIndexes(
            IStatus status,
            string taskMessage,
            Func<long, Type, Task> itemAction,
            Func<long[], Task> indexAction,
            params Type[] data)
        {
            var mapTask = await statusController.CreateAsync(status, taskMessage);
            var counter = 0;
            var indexes = new List<long>();

            foreach (var item in data)
            {
                var index = indexingController.GetIndex(item);
                indexes.Add(index);

                await statusController.UpdateProgressAsync(
                    mapTask,
                    ++counter,
                    data.Length,
                    index.ToString());

                await itemAction(index, item);
            }

            var updateIndexTask = await statusController.CreateAsync(mapTask, "Update indexes");
            await indexAction(indexes.ToArray());
            await statusController.CompleteAsync(updateIndexTask);

            await statusController.CompleteAsync(mapTask);
        }

        public async Task UpdateAsync(IStatus status, params Type[] data)
        {
            await MapItemsAndIndexes(
                status,
                "Update data item(s)",
                async (index, item) =>
                {
                    await serializedStorageController.SerializePushAsync(
                        GetItemUri(index),
                        item,
                        status);
                },
                async (indexes) =>
                {
                    await indexDataController.UpdateAsync(status, indexes);
                },
                data);
        }

        public async Task RemoveAsync(IStatus status, params Type[] data)
        {
            await MapItemsAndIndexes(
                status,
                "Remove data item(s)",
                async (index, item) =>
                {
                    if (await indexDataController.ContainsAsync(index, status))
                        recycleBinController.MoveToRecycleBin(GetItemUri(index));
                },
                async (indexes) =>
                {
                    await indexDataController.RemoveAsync(status, indexes);
                },
                data);
        }

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            return await indexDataController.EnumerateIdsAsync(status);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            return await indexDataController.CountAsync(status);
        }

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            return await indexDataController.ContainsAsync(id, status);
        }
    }
}
