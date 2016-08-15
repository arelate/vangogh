using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Politeness;

using Models.Uris;

using GOG.Models;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ProductUpdateController<UpdateType, ListType> : TaskActivityController
        where ListType : ProductCore
        where UpdateType : ProductCore 
    {
        internal IProductTypeStorageController productStorageController;
        private ICollectionController collectionController;
        private INetworkController networkController;
        private IPolitenessController politenessController;
        internal ISerializationController<string> serializationController;

        internal ProductTypes updateProductType;
        internal ProductTypes listProductType;
        internal string displayProductName;

        public ProductUpdateController(
            IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IPolitenessController politenessController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.productStorageController = productStorageController;
            this.collectionController = collectionController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            this.politenessController = politenessController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.AddTask("Load existing products and " + displayProductName);

            var products = await productStorageController.Pull<ListType>(listProductType);
            var existingData = await productStorageController.Pull<UpdateType>(updateProductType);
            if (existingData == null) existingData = new List<UpdateType>();
            var updateCollection = new List<UpdateType>(existingData);

            taskReportingController.CompleteTask();

            var updateProducts = await GetUpdates(products, updateCollection);

            taskReportingController.AddTask("Update required " + displayProductName);

            var currentProduct = 0;
            var storagePushNthProduct = 100; // push after updating every nth product

            foreach (var id in updateProducts)
            {
                var product = collectionController.Find(products, p => p.Id == id);

                taskReportingController.AddTask(
                    string.Format(
                        "Update {0} {1}/{2}: {3}",
                        displayProductName,
                        ++currentProduct,
                        updateProducts.Count,
                        product.Title));

                var content = await GetContent(product);
                if (content == null) continue;

                var data = Deserialize(content, product);

                if (data != null) updateCollection.Add(data);

                if (currentProduct % storagePushNthProduct == 0)
                    await PushChanges(updateCollection);

                if (politenessController != null)
                {
                    taskReportingController.AddTask("Throttle network requests");
                    politenessController.Throttle();
                    taskReportingController.CompleteTask();
                }

                taskReportingController.CompleteTask();
            }

            await PushChanges(updateCollection);

            taskReportingController.CompleteTask();
        }

        internal virtual async Task<List<long>> GetUpdates(IEnumerable<ListType> products, IEnumerable<UpdateType> updateCollection)
        {
            taskReportingController.AddTask("Get a list of updates for " + displayProductName);

            var updateProducts = new List<long>();

            foreach (var id in await GetRequiredUpdates())
            {
                var product = collectionController.Find(products, p => p.Id == id);
                if (product != null) updateProducts.Add(product.Id);
            }

            foreach (var product in products)
            {
                if (product == null) continue;
                if (ShouldSkipProduct(product)) continue;
                var foundGameProductData = collectionController.Find(updateCollection, p => p != null && p.Id == product.Id);
                if (foundGameProductData == null &&
                    !updateProducts.Contains(product.Id))
                    updateProducts.Add(product.Id);
            }

            taskReportingController.CompleteTask();

            return updateProducts;
        }

        internal virtual async Task<string> GetContent(ListType product)
        {
            var uri = string.Format(Uris.Paths.GetUpdateUri(updateProductType), GetProductUri(product));
            var rawResponse = await networkController.Get(uri);

            var content = ProcessRawData(rawResponse);

            if (content == null)
                taskReportingController.ReportWarning(
                    string.Format(
                        "Product {0} doesn't have valid " + displayProductName,
                        product.Title));

            return content;
        }

        internal virtual async Task PushChanges(IList<UpdateType> updateCollection)
        {
            taskReportingController.AddTask("Save " + displayProductName + " to disk");
            await productStorageController.Push(updateProductType, updateCollection);
            taskReportingController.CompleteTask();
        }

        internal virtual async Task<long[]> GetRequiredUpdates()
        {
            return new long[0];
        }

        internal virtual bool ShouldSkipProduct(ListType product)
        {
            return false;
        }

        internal virtual string GetProductUri(ListType product)
        {
            return product.Id.ToString(); // most product updates go by id
        }

        internal virtual string ProcessRawData(string rawData)
        {
            return rawData; // passthrough by default
        }

        internal virtual UpdateType Deserialize(string content, ListType product)
        {
            return serializationController.Deserialize<UpdateType>(content);
        }
    }
}
