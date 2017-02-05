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

        public async Task RemoveAsync(ITaskStatus taskStatus, params long[] data)
        {
            foreach (var item in data)
                if (indexes.Contains(item))
                    indexes.Remove(item);

            await SaveAsync();
        }

        public async Task AddAsync(ITaskStatus taskStatus, params long[] data)
        {
            foreach (var index in data)
                if (!indexes.Contains(index))
                    indexes.Add(index);

            await SaveAsync();
        }

        public Task ModifyAsync(ITaskStatus taskStatus, params long[] data)
        {
            throw new NotImplementedException();
        }
    }
}
