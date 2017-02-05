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

namespace Controllers.Data
{
    public class DataController<Type> : IDataController<Type>
    {
        private IDataController<long> indexDataController;

        private IIndexingController indexingController;
        private ICollectionController collectionController;

        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;

        private IRecycleBinController recycleBinController;

        private ISerializedStorageController serializedStorageController;

        public DataController(
            IDataController<long> indexDataController,
            ISerializedStorageController serializedStorageController,
            IIndexingController indexingController,
            ICollectionController collectionController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IRecycleBinController recycleBinController = null)
        {
            this.indexDataController = indexDataController;

            this.serializedStorageController = serializedStorageController;

            this.indexingController = indexingController;
            this.collectionController = collectionController;

            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;

            this.recycleBinController = recycleBinController;
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

        public async Task SaveAsync()
        {
            await indexDataController.SaveAsync();
        }

        public async Task RemoveAsync(params Type[] data)
        {
            foreach (var item in data)
            {
                var index = indexingController.GetIndex(item);
                if (indexDataController.Contains(index))
                {
                    await indexDataController.RemoveAsync(index);
                    recycleBinController?.MoveFileToRecycleBin(GetItemUri(index));
                }
            }

            await SaveAsync();
        }

        public async Task UpdateAsync(params Type[] data)
        {
            foreach (var item in data)
            {
                var index = indexingController.GetIndex(item);

                if (!indexDataController.Contains(index))
                    await indexDataController.UpdateAsync(index);

                await serializedStorageController.SerializePushAsync(
                    GetItemUri(index), 
                    item);
            }

            await SaveAsync();
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
