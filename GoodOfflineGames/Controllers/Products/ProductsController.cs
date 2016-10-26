using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Products;
using Interfaces.Collection;
using Interfaces.Indexing;
using Interfaces.ProductStoragePolicy;
using Interfaces.Destination;
using Interfaces.File;

using Interfaces.SerializedStorage;

namespace Controllers.Products
{
    public class ProductsController<Type> : IProductsController<Type>
    {
        private ProductStoragePolicy productStoragePolicy;
        private IIndexingController indexingController;
        private ICollectionController collectionController;

        private IDestinationController destinationController;
        private IMoveToRecycleBinDelegate moveToRecycleBinDelegate;

        private ISerializedStorageController serializedStorageController;

        private IList<Type> products;
        private IList<long> productsIndexes;

        private string destinationDirectory;

        private const string indexesFilename = "indexes.json";
        private string indexesUri;

        private string productsUri;

        public ProductsController(
            ISerializedStorageController serializedStorageController,

            ProductStoragePolicy productStoragePolicy,
            IIndexingController indexingController,
            ICollectionController collectionController,
            IDestinationController destinationController,
            IMoveToRecycleBinDelegate moveToRecycleBinDelegate)
        {
            this.productStoragePolicy = productStoragePolicy;
            this.indexingController = indexingController;
            this.collectionController = collectionController;

            this.serializedStorageController = serializedStorageController;

            if (this.productStoragePolicy == ProductStoragePolicy.SerializeItems)
                productsUri = Path.Combine(destinationDirectory,
                    destinationController.GetFilename(string.Empty));

            this.destinationController = destinationController;
            destinationDirectory = destinationController.GetDirectory(string.Empty);
            indexesUri = Path.Combine(destinationDirectory, indexesFilename);

            this.moveToRecycleBinDelegate = moveToRecycleBinDelegate;

            products = null;
            productsIndexes = null;
        }

        public bool Contains(Type product)
        {
            switch (productStoragePolicy)
            {
                case ProductStoragePolicy.IndexAndSerializeItems:
                    return productsIndexes.Contains(indexingController.GetIndex(product));
                case ProductStoragePolicy.SerializeItems:
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

        public async Task<Type> GetProductById(long id)
        {
            switch (productStoragePolicy)
            {
                case ProductStoragePolicy.IndexAndSerializeItems:
                    var productUri = GetProductUri(id);
                    return await serializedStorageController.DeserializePull<Type>(productUri);
                case ProductStoragePolicy.SerializeItems:
                    return collectionController.Find(
                        products,
                        p =>
                            indexingController.GetIndex(p) == id);
            }

            return default(Type);
        }

        public async Task LoadProducts()
        {
            switch (productStoragePolicy)
            {
                case ProductStoragePolicy.IndexAndSerializeItems:
                    productsIndexes = await serializedStorageController.DeserializePull<List<long>>(indexesUri);
                    break;
                case ProductStoragePolicy.SerializeItems:
                    products = await serializedStorageController.DeserializePull<List<Type>>(productsUri);
                    break;
            }
        }

        public async Task RemoveProduct(Type product)
        {
            switch (productStoragePolicy)
            {
                case ProductStoragePolicy.IndexAndSerializeItems:
                    var index = indexingController.GetIndex(product);
                    productsIndexes.Remove(index);
                    var productUri = GetProductUri(index);
                    moveToRecycleBinDelegate.MoveToRecycleBin(productUri);
                    await serializedStorageController.SerializePush(indexesUri, productsIndexes);
                    break;
                case ProductStoragePolicy.SerializeItems:
                    products.Remove(product);
                    await serializedStorageController.SerializePush(productsUri, products);
                    break;
            }
        }

        public async Task UpdateProduct(Type product)
        {
            switch (productStoragePolicy)
            {
                case ProductStoragePolicy.IndexAndSerializeItems:
                    var index = indexingController.GetIndex(product);
                    var productUri = GetProductUri(index);
                    await serializedStorageController.SerializePush(productUri, product);
                    break;
                case ProductStoragePolicy.SerializeItems:
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
