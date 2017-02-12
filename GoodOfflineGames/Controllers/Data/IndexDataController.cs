using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Collection;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.TaskStatus;

namespace Controllers.Data
{
    public class IndexDataController : IDataController<long>
    {
        private ICollectionController collectionController;

        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private ISerializedStorageController serializedStorageController;
        private ITaskStatusController taskStatusController;

        private IList<long> indexes;

        public IndexDataController(
            ICollectionController collectionController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController,
            ITaskStatusController taskStatusController)
        {
            this.collectionController = collectionController;

            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.serializedStorageController = serializedStorageController;

            this.taskStatusController = taskStatusController;
        }

        public bool Contains(long data)
        {
            return indexes.Contains(data);
        }

        public bool ContainsId(long id)
        {
            return indexes.Contains(id);
        }

        public int Count()
        {
            return indexes.Count;
        }

        public IEnumerable<long> EnumerateIds()
        {
            return indexes;
        }

        public Task<long> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync()
        {
            var indexUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            indexes = await serializedStorageController.DeserializePullAsync<List<long>>(indexUri);
            if (indexes == null) indexes = new List<long>();
        }

        public async Task SaveAsync()
        {
            var indexUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            await serializedStorageController.SerializePushAsync(indexUri, indexes);
        }

        private async Task Map(ITaskStatus taskStatus, string taskMessage, Func<long, bool> itemAction, params long[] data)
        {
            var task = taskStatusController.Create(taskStatus, taskMessage);
            var counter = 0;
            var dataChanged = false;

            foreach (var item in data)
            {
                taskStatusController.UpdateProgress(
                    task,
                    ++counter,
                    data.Length,
                    item.ToString());

                // do this for every item
                if (itemAction(item)) dataChanged = true;
            }

            if (dataChanged)
            {
                var saveDataTask = taskStatusController.Create(task, "Save modified index");
                await SaveAsync();
                taskStatusController.Complete(saveDataTask);
            }

            taskStatusController.Complete(task);
        }

        public async Task RemoveAsync(ITaskStatus taskStatus, params long[] data)
        {
            await Map(
                taskStatus,
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

        public async Task UpdateAsync(ITaskStatus taskStatus, params long[] data)
        {
            await Map(
                taskStatus,
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
