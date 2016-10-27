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
            IRecycleBinController recycleBinController)
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

            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    return dataIndexes.Contains(indexingController.GetIndex(data));
                case DataStoragePolicy.ItemsList:
                    var index = indexingController.GetIndex(data);
                    var existingData = collectionController.Find(
                        dataItems,
                        d =>
                            indexingController.GetIndex(d) == index);
                    return existingData != null;
            }
            return false;
        }

        private string GetDataUri(long index)
        {
            return Path.Combine(
                destinationDirectory,
                destinationController.GetFilename(index.ToString()));
        }

        public async Task<Type> GetById(long id)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    var dataUri = GetDataUri(id);
                    return await serializedStorageController.DeserializePull<Type>(dataUri);
                case DataStoragePolicy.ItemsList:
                    return collectionController.Find(
                        dataItems,
                        p =>
                            indexingController.GetIndex(p) == id);
            }

            return default(Type);
        }

        public async Task Initialize()
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    dataIndexes = await serializedStorageController.DeserializePull<List<long>>(dataIndexesUri);
                    if (dataIndexes == null) dataIndexes = new List<long>();
                    break;
                case DataStoragePolicy.ItemsList:
                    dataItems = await serializedStorageController.DeserializePull<List<Type>>(dataItemsUri);
                    if (dataItems == null) dataItems = new List<Type>();
                    break;
            }
        }

        public async Task Remove(Type data)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    var index = indexingController.GetIndex(data);
                    dataIndexes.Remove(index);
                    var dataUri = GetDataUri(index);
                    recycleBinController.MoveToRecycleBin(dataUri);
                    await serializedStorageController.SerializePush(dataIndexesUri, dataIndexes);
                    break;
                case DataStoragePolicy.ItemsList:
                    dataItems.Remove(data);
                    await serializedStorageController.SerializePush(dataItemsUri, dataItems);
                    break;
            }
        }

        public async Task Update(Type data)
        {

            var index = indexingController.GetIndex(data);

            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndItems:
                    if (!dataIndexes.Contains(index))
                    {
                        dataIndexes.Add(index);
                        await serializedStorageController.SerializePush(dataIndexesUri, dataIndexes);
                    }
                    var dataUri = GetDataUri(index);
                    await serializedStorageController.SerializePush(dataUri, data);
                    break;
                case DataStoragePolicy.ItemsList:
                    var updated = false;
                    for (var ii = 0; ii < dataItems.Count; ii++)
                        if (indexingController.GetIndex(dataItems[ii]) == index)
                        {
                            dataItems[ii] = data;
                            updated = true;
                        }
                    if (!updated) dataItems.Add(data);
                    await serializedStorageController.SerializePush(dataItemsUri, dataItems);
                    break;
            }
        }
    }
}
