using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
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

        public bool Contains(Type data)
        {
            if (data == null) return true;
            var index = indexingController.GetIndex(data);
            return indexDataController.Contains(index);
        }

        private string GetItemUri(long id)
        {
            return Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename(id.ToString()));
        }

        public async Task<Type> GetByIdAsync(long id)
        {
            return await serializedStorageController.DeserializePullAsync<Type>(GetItemUri(id));
        }

        public async Task LoadAsync()
        {
            await indexDataController.LoadAsync();
        }

        public Task SaveAsync()
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
            var mapTask = statusController.Create(status, taskMessage);
            var counter = 0;
            var indexes = new List<long>();

            foreach (var item in data)
            {
                var index = indexingController.GetIndex(item);
                indexes.Add(index);

                statusController.UpdateProgress(
                    mapTask,
                    ++counter,
                    data.Length,
                    index.ToString());

                await itemAction(index, item);
            }

            var updateIndexTask = statusController.Create(mapTask, "Update indexes");
            await indexAction(indexes.ToArray());
            statusController.Complete(updateIndexTask);

            statusController.Complete(mapTask);
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
                        item);
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
                    await Task.Run(() =>
                    {
                        if (indexDataController.Contains(index))
                            recycleBinController.MoveToRecycleBin(GetItemUri(index));
                    });
                },
                async (indexes) =>
                {
                    await indexDataController.RemoveAsync(status, indexes);
                },
                data);
        }

        public IEnumerable<long> EnumerateIds()
        {
            return indexDataController.EnumerateIds();
        }

        public int Count()
        {
            return indexDataController.Count();
        }

        public bool ContainsId(long id)
        {
            return indexDataController.Contains(id);
        }
    }
}
