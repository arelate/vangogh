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
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Data
{
    public class DataController<Type> : IDataController<Type>
    {
        readonly IDictionary<long, Type> data;

        readonly IIndexController<long> indexController;

        ISerializedStorageController serializedStorageController;

        IConvertDelegate<Type, long> convertProductToIndexDelegate;
        ICollectionController collectionController;

        IGetPathDelegate getPathDelegate;

        IRecycleDelegate recycleDelegate;

        IRecordsController<long> recordsController;

        IStatusController statusController;

        ICommitAsyncDelegate[] additionalCommitDelegates;

        public DataController(
            IIndexController<long> indexController,
            ISerializedStorageController serializedStorageController,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            ICollectionController collectionController,
            IGetPathDelegate getPathDelegate,
            IRecycleDelegate recycleDelegate,
            IRecordsController<long> recordsController,
            IStatusController statusController,
            params ICommitAsyncDelegate[] additionalCommitDelegates)
        {
            this.data = new Dictionary<long, Type>();

            this.indexController = indexController;

            this.serializedStorageController = serializedStorageController;

            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.collectionController = collectionController;

            this.getPathDelegate = getPathDelegate;

            this.recycleDelegate = recycleDelegate;

            this.recordsController = recordsController;

            this.statusController = statusController;
            this.additionalCommitDelegates = additionalCommitDelegates;
        }

        public async Task<bool> ContainsAsync(Type data, IStatus status)
        {
            var index = convertProductToIndexDelegate.Convert(data);
            return await indexController.ContainsIdAsync(index, status);
        }

        public async Task<Type> GetByIdAsync(long id, IStatus status)
        {
            if (data.ContainsKey(id)) return data[id];
            else return await serializedStorageController.DeserializePullAsync<Type>(
                getPathDelegate.GetPath(
                    string.Empty,
                    id.ToString()), 
                status);
        }

        async Task MapItemsAndIndexesAsync(
            IStatus status,
            string taskMessage,
            Func<long, Type, Task> action,
            Type data)
        {
            var mapTask = await statusController.CreateAsync(status, taskMessage, false);

            var index = convertProductToIndexDelegate.Convert(data);
            await action(index, data);

            await statusController.CompleteAsync(mapTask, false);
        }

        public async Task UpdateAsync(Type updatedData, IStatus status)
        {
            await MapItemsAndIndexesAsync(
                status,
                "Update data item",
                async (index, item) =>
                {
                    if (!(await indexController.ContainsIdAsync(index, status)))
                        await indexController.CreateAsync(index, status);

                    if (!data.ContainsKey(index)) data.Add(index, item);
                    else data[index] = item;

                    if (recordsController != null)
                        await recordsController.SetRecordAsync(
                            index, 
                            RecordsTypes.Updated, 
                            status);
                },
                updatedData);
        }

        public async Task DeleteAsync(Type data, IStatus status)
        {
            await MapItemsAndIndexesAsync(
                status,
                "Remove data item(s)",
                async (index, item) =>
                {
                    if (await indexController.ContainsIdAsync(index, status))
                        recycleDelegate.Recycle(
                            getPathDelegate.GetPath(
                                string.Empty,
                                index.ToString()));

                    await indexController.DeleteAsync(index, status);

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

        public async Task CommitAsync(IStatus status)
        {
            var commitTask = await statusController.CreateAsync(status, "Commit updated data");

            var commitIndexTask = await statusController.CreateAsync(commitTask, "Commit index");
            // commit index controller
            await indexController.CommitAsync(status);
            await statusController.CompleteAsync(commitIndexTask);

            // commit records controller
            if (recordsController != null)
            {
                var commitRecordsTask = await statusController.CreateAsync(commitTask, "Commit records");
                await recordsController.CommitAsync(status);
                await statusController.CompleteAsync(commitRecordsTask);
            }

            var commitDataTask = await statusController.CreateAsync(commitTask, "Commit items");
            var current = 0;
            // update all items
            foreach (var indexItem in data)
            {
                var index = indexItem.Key;
                var item = indexItem.Value;

                await statusController.UpdateProgressAsync(
                    commitDataTask,
                    ++current,
                    data.Count,
                    index.ToString());

                await serializedStorageController.SerializePushAsync(
                    getPathDelegate.GetPath(
                        string.Empty,
                        index.ToString()),
                    item,
                    status);
            }
            await statusController.CompleteAsync(commitDataTask);

            var additionalCommitsTask = await statusController.CreateAsync(commitTask, "Additional commit dependencies");
            current = 0;

            foreach (var commitDelegate in additionalCommitDelegates)
            {
                await statusController.UpdateProgressAsync(
                    additionalCommitsTask,
                    ++current,
                    additionalCommitDelegates.Length,
                    "Commit delegate");

                await commitDelegate.CommitAsync(additionalCommitsTask);
            }

            await statusController.CompleteAsync(additionalCommitsTask);

            await statusController.CompleteAsync(commitTask);
        }
    }
}
