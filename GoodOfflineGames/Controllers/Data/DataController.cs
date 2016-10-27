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

        private IList<Type> products;
        private IList<long> productsIndexes;

        private string destinationDirectory;

        private const string indexesFilename = "indexes.json";
        private string indexesUri;

        private string productsUri;

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
            indexesUri = Path.Combine(destinationDirectory, indexesFilename);

            this.recycleBinController = recycleBinController;

            if (this.dataStoragePolicy == DataStoragePolicy.SerializeItems)
                productsUri = Path.Combine(destinationDirectory,
                    destinationController.GetFilename(string.Empty));

            products = null;
            productsIndexes = null;
        }

        public bool Contains(Type product)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndSerializeItems:
                    return productsIndexes.Contains(indexingController.GetIndex(product));
                case DataStoragePolicy.SerializeItems:
                    var productIndex = indexingController.GetIndex(product);
                    var existingProduct = collectionController.Find(
                        products,
                        p =>
                            indexingController.GetIndex(p) == productIndex);
                    return existingProduct != null;
            }
            return false;
        }

        private string GetProductUri(long index)
        {
            return Path.Combine(
                destinationDirectory,
                destinationController.GetFilename(index.ToString()));
        }

        public async Task<Type> GetById(long id)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndSerializeItems:
                    var productUri = GetProductUri(id);
                    return await serializedStorageController.DeserializePull<Type>(productUri);
                case DataStoragePolicy.SerializeItems:
                    return collectionController.Find(
                        products,
                        p =>
                            indexingController.GetIndex(p) == id);
            }

            return default(Type);
        }

        public async Task Initialize()
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndSerializeItems:
                    productsIndexes = await serializedStorageController.DeserializePull<List<long>>(indexesUri);
                    break;
                case DataStoragePolicy.SerializeItems:
                    products = await serializedStorageController.DeserializePull<List<Type>>(productsUri);
                    break;
            }
        }

        public async Task Remove(Type product)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndSerializeItems:
                    var index = indexingController.GetIndex(product);
                    productsIndexes.Remove(index);
                    var productUri = GetProductUri(index);
                    recycleBinController.MoveToRecycleBin(productUri);
                    await serializedStorageController.SerializePush(indexesUri, productsIndexes);
                    break;
                case DataStoragePolicy.SerializeItems:
                    products.Remove(product);
                    await serializedStorageController.SerializePush(productsUri, products);
                    break;
            }
        }

        public async Task Update(Type product)
        {
            switch (dataStoragePolicy)
            {
                case DataStoragePolicy.IndexAndSerializeItems:
                    var index = indexingController.GetIndex(product);
                    var productUri = GetProductUri(index);
                    await serializedStorageController.SerializePush(productUri, product);
                    break;
                case DataStoragePolicy.SerializeItems:
                    var productIndex = indexingController.GetIndex(product);
                    var updated = false;
                    for (var ii = 0; ii < products.Count; ii++)
                        if (indexingController.GetIndex(products[ii]) == productIndex)
                        {
                            products[ii] = product;
                            updated = true;
                        }
                    if (!updated) products.Add(product);
                    await serializedStorageController.SerializePush(productsUri, products);
                    break;
            }
        }
    }
}
