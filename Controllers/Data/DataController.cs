using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.GetPath;

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

        private IGetPathDelegate getPathDelegate;

        private IRecycleDelegate recycleDelegate;

        private IStatusController statusController;

        public DataController(
            IIndexController<long> indexController,
            ISerializedStorageController serializedStorageController,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            ICollectionController collectionController,
            IGetPathDelegate getPathDelegate,
            IRecycleDelegate recycleDelegate,
            IStatusController statusController)
        {
            this.indexController = indexController;

            this.serializedStorageController = serializedStorageController;

            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.collectionController = collectionController;

            this.getPathDelegate = getPathDelegate;

            this.recycleDelegate = recycleDelegate;

            this.statusController = statusController;
        }

        public async Task<bool> ContainsAsync(Type data, IStatus status)
        {
            if (data == null) return true;

            var index = convertProductToIndexDelegate.Convert(data);
            return await indexController.ContainsIdAsync(index, status);
        }

        public async Task<Type> GetByIdAsync(long id, IStatus status)
        {
            return await serializedStorageController.DeserializePullAsync<Type>(
                getPathDelegate.GetPath(
                    string.Empty,
                    id.ToString()), 
                status);
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
                        getPathDelegate.GetPath(
                            string.Empty,
                            index.ToString()),
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
                    recycleDelegate.Recycle(
                        getPathDelegate.GetPath(
                            string.Empty,
                            index.ToString()));
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
    }
}
