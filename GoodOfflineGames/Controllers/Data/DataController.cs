using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.DataStoragePolicy;
using Interfaces.Collection;
using Interfaces.Indexing;
using Interfaces.Destination;
using Interfaces.RecycleBin;

using Interfaces.SerializedStorage;
using System;

namespace Controllers.Data
{
    public class DataController<Type> : IDataController<Type>
    {
        private DataStoragePolicy dataStoragePolicy;
        private IIndexingController indexingController;
        private ICollectionController collectionController;

        private IDestinationController destinationController;
        private IRecycleBinController recycleBinController;

        private ISerializedStorageController serializedStorageController;

        private IList<Type> dataItems;
        private IList<long> dataIndexes;

        private string destinationDirectory;

        private const string dataIndexesFilename = "indexes.json";
        private string dataIndexesUri;

        private string dataItemsUri;

        public DataController(
            ISerializedStorageController serializedStorageController,

            DataStoragePolicy dataStoragePolicy,
            IIndexingController indexingController,
            ICollectionController collectionController,
            IDestinationController destinationController,
            IRecycleBinController recycleBinController = null)
        {
            this.dataStoragePolicy = dataStoragePolicy;
            this.indexingController = indexingController;
            this.collectionController = collectionController;

            this.serializedStorageController = serializedStorageController;

            this.destinationController = destinationController;
            destinationDirectory = destinationController.GetDirectory(string.Empty);
            dataIndexesUri = Path.Combine(destinationDirectory, dataIndexesFilename);

            this.recycleBinController = recycleBinController;

            if (this.dataStoragePolicy == DataStoragePolicy.ItemsList)
                dataItemsUri = Path.Combine(destinationDirectory,
                    destinationController.GetFilename(string.Empty));

            dataItems = null;
            dataIndexes = null;
        }

        public bool Contains(Type data)
        {
            if (data == null) return true;
            var index = indexingController.GetIndex(data);
            return dataIndexes.Contains(index);
        }

        private string GetDataUri(long index)
        {
            return Path.Combine(
                destinationDirectory,
                destinationController.GetFilename(index.ToString()));
        }

        public async Task<Type> GetByIdAsync(long id)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    var dataUri = GetDataUri(id);
                    return await serializedStorageController.DeserializePullAsync<Type>(dataUri);
                case DataStoragePolicy.ItemsList:
                    return collectionController.Find(
                        dataItems,
                        p =>
                            indexingController.GetIndex(p) == id);
            }

            return default(Type);
        }

        public async Task LoadAsync()
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    dataIndexes = await serializedStorageController.DeserializePullAsync<List<long>>(dataIndexesUri);
                    break;
                case DataStoragePolicy.ItemsList:
                    dataItems = await serializedStorageController.DeserializePullAsync<List<Type>>(dataItemsUri);
                    break;
            }

            if (dataIndexes == null) dataIndexes = new List<long>();
            if (dataItems == null) dataItems = new List<Type>();

            foreach (var item in dataItems)
                dataIndexes.Add(indexingController.GetIndex(item));
        }

        public async Task SaveAsync()
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    await serializedStorageController.SerializePushAsync(dataIndexesUri, dataIndexes);
                    break;
                case DataStoragePolicy.ItemsList:
                    await serializedStorageController.SerializePushAsync(dataItemsUri, dataItems);
                    break;
            }
        }

        public async Task RemoveAsync(params Type[] data)
        {
            foreach (var item in data)
            {
                if (dataItems.Contains(item)) dataItems.Remove(item);

                var index = indexingController.GetIndex(item);
                if (dataIndexes.Contains(index))
                {
                    dataIndexes.Remove(index);
                    var dataUri = GetDataUri(index);
                    recycleBinController?.MoveToRecycleBin(dataUri);
                }
            }

            await SaveAsync();
        }

        public async Task UpdateAsync(params Type[] data)
        {
            foreach (var item in data)
            {
                var index = indexingController.GetIndex(item);
                if (!dataIndexes.Contains(index))
                    dataIndexes.Add(index);

                switch (dataStoragePolicy)
                {
                    case DataStoragePolicy.IndexAndItems:
                        var dataUri = GetDataUri(index);
                        await serializedStorageController.SerializePushAsync(dataUri, item);
                        break;
                    case DataStoragePolicy.ItemsList:
                        var updated = false;
                        for (var ii = 0; ii < dataItems.Count; ii++)
                            if (indexingController.GetIndex(dataItems[ii]) == index)
                            {
                                dataItems[ii] = item;
                                updated = true;
                            }
                        if (!updated) dataItems.Add(item);
                        break;
                }
            }

            await SaveAsync();
        }

        public IEnumerable<long> EnumerateIds()
        {
            return dataIndexes;
        }

        public int Count()
        {
            return dataIndexes.Count;
        }

        public bool ContainsId(long id)
        {
            return dataIndexes.Contains(id);
        }
    }
}
