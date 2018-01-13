using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Collection;

using Interfaces.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Data
{
    public class DataController<Type> : IDataController<Type>
    {
        private IIndexController<long> indexController;

        private ISerializedStorageController serializedStorageController;

        private IConvertDelegate<Type, long> convertProductToIndexDelegate;
        private ICollectionController collectionController;

        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private IRecycleDelegate moveToRecycleBinDelegate;

        private IStatusController statusController;

        public DataController(
            IIndexController<long> indexController,
            ISerializedStorageController serializedStorageController,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            ICollectionController collectionController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecycleDelegate moveToRecycleBinDelegate,
            IStatusController statusController)
        {
            this.indexController = indexController;

            this.serializedStorageController = serializedStorageController;

            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.collectionController = collectionController;

            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.moveToRecycleBinDelegate = moveToRecycleBinDelegate;

            this.statusController = statusController;
        }

        public bool DataAvailable
        {
            get { return true; } // dataController doesn't hold any state, always reads from disk, so the data is always available
        }

        public async Task<bool> ContainsAsync(Type data, IStatus status)
        {
            if (data == null) return true;

            var index = convertProductToIndexDelegate.Convert(data);
            return await indexController.ContainsIdAsync(index, status);
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

            await indexController.LoadAsync(loadStatus);

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
                var index = convertProductToIndexDelegate.Convert(item);
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
                    await indexController.UpdateAsync(status, indexes);
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
                    if (await indexController.ContainsIdAsync(index, status))
                        moveToRecycleBinDelegate.Recycle(GetItemUri(index));
                },
                async (indexes) =>
                {
                    await indexController.RemoveAsync(status, indexes);
                },
                data);
        }

        public async Task<IEnumerable<long>> ItemizeAllAsync(IStatus status)
        {
            return await indexController.ItemizeAllAsync(status);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            return await indexController.CountAsync(status);
        }

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            return await indexController.ContainsIdAsync(id, status);
        }

        public async Task<DateTime> GetLastModifiedAsync(long id, IStatus status)
        {
            return await indexController.GetLastModifiedAsync(id, status);
        }

        public async Task<IEnumerable<long>> ItemizeAsync(DateTime item, IStatus status)
        {
            return await indexController.ItemizeAsync(item, status);
        }
    }
}
