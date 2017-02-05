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
using Interfaces.TaskStatus;

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

        private ITaskStatusController taskStatusController;

        public DataController(
            IDataController<long> indexDataController,
            ISerializedStorageController serializedStorageController,
            IIndexingController indexingController,
            ICollectionController collectionController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecycleBinController recycleBinController,
            ITaskStatusController taskStatusController)
        {
            this.indexDataController = indexDataController;

            this.serializedStorageController = serializedStorageController;

            this.indexingController = indexingController;
            this.collectionController = collectionController;

            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.recycleBinController = recycleBinController;

            this.taskStatusController = taskStatusController;
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
            throw new System.NotImplementedException();
        }

        private async Task Map(ITaskStatus taskStatus, string taskMessage, Func<long, Type, Task> itemAction, params Type[] data)
        {
            var task = taskStatusController.Create(taskStatus, taskMessage);
            var counter = 0;

            foreach (var item in data)
            {
                var index = indexingController.GetIndex(item);

                taskStatusController.UpdateProgress(
                    task,
                    ++counter,
                    data.Length,
                    index.ToString());

                // do this for every item
                await itemAction(index, item);
            }

            taskStatusController.Complete(task);
        }

        public async Task AddAsync(ITaskStatus taskStatus, params Type[] data)
        {
            await Map(
                taskStatus,
                "Add data items",
                async (index, item) =>
                {
                    if (!indexDataController.Contains(index))
                    {
                        await indexDataController.AddAsync(taskStatus, index);
                        await serializedStorageController.SerializePushAsync(
                            GetItemUri(index),
                            item);
                    }
                },
                data);
        }

        public async Task RemoveAsync(ITaskStatus taskStatus, params Type[] data)
        {
            await Map(
                taskStatus,
                "Remove data items",
                async (index, item) =>
                {
                    if (indexDataController.Contains(index))
                    {
                        await indexDataController.RemoveAsync(taskStatus, index);
                        recycleBinController?.MoveFileToRecycleBin(GetItemUri(index));
                    }
                },
                data);
        }

        public async Task ModifyAsync(ITaskStatus taskStatus, params Type[] data)
        {
            await Map(
                taskStatus,
                "Modify data items",
                async (index, item) =>
                {
                    if (indexDataController.Contains(index))
                        await RemoveAsync(taskStatus, data);
                    await AddAsync(taskStatus, data);
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
