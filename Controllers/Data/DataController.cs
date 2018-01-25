using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Records;

using Interfaces.SerializedStorage;
using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

using Models.ProductCore;

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

        private IRecordsController<long> recordsController;

        private IStatusController statusController;

        public DataController(
            IIndexController<long> indexController,
            ISerializedStorageController serializedStorageController,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            ICollectionController collectionController,
            IGetPathDelegate getPathDelegate,
            IRecycleDelegate recycleDelegate,
            IRecordsController<long> recordsController,
            IStatusController statusController)
        {
            this.indexController = indexController;

            this.serializedStorageController = serializedStorageController;

            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.collectionController = collectionController;

            this.getPathDelegate = getPathDelegate;

            this.recycleDelegate = recycleDelegate;

            this.recordsController = recordsController;

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
            Func<long, Type, Task> action,
            Type data)
        {
            var mapTask = await statusController.CreateAsync(status, taskMessage);

            var index = convertProductToIndexDelegate.Convert(data);
            await action(index, data);

            await statusController.CompleteAsync(mapTask);
        }

        public async Task UpdateAsync(Type data, IStatus status)
        {
            await MapItemsAndIndexes(
                status,
                "Update data item",
                async (index, item) =>
                {
                    if (!(await indexController.ContainsIdAsync(index, status)))
                        await indexController.CreateAsync(index, status);

                    await serializedStorageController.SerializePushAsync(
                        getPathDelegate.GetPath(
                            string.Empty,
                            index.ToString()),
                        item,
                        status);

                    if (recordsController != null)
                        await recordsController.SetRecordAsync(
                            index, 
                            RecordsTypes.Updated, 
                            status);
                },
                data);
        }

        public async Task DeleteAsync(Type data, IStatus status)
        {
            await MapItemsAndIndexes(
                status,
                "Remove data item(s)",
                async (index, item) =>
                {
                    await indexController.DeleteAsync(index, status);

                    if (await indexController.ContainsIdAsync(index, status))
                    recycleDelegate.Recycle(
                        getPathDelegate.GetPath(
                            string.Empty,
                            index.ToString())); 

                    if (recordsController != null)
                        await recordsController.SetRecordAsync(
                            index, 
                            RecordsTypes.Deleted, 
                            status);
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
