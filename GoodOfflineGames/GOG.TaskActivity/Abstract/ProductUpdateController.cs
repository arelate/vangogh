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
    public abstract class ProductUpdateController<Type> : TaskActivityController where Type : ProductCore
    {
        private IProductTypeStorageController productStorageController;
        private ICollectionController collectionController;
        private INetworkController networkController;
        private IPolitenessController politenessController;
        internal ISerializationController<string> serializationController;

        internal ProductTypes productType;
        internal string name;

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
            taskReportingController.AddTask("Load existing products and " + name);
            var products = await productStorageController.Pull<Product>(ProductTypes.Product);
            var existingData = await productStorageController.Pull<Type>(productType);
            if (existingData == null) existingData = new List<Type>();
            var dataCollection = new List<Type>(existingData);
            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Get products without " + name);
            var updateProducts = new List<Product>();
            foreach (var product in products)
            {
                if (product == null) continue;
                var foundGameProductData = collectionController.Find(dataCollection, p => p.Id == product.Id);
                if (foundGameProductData == null) updateProducts.Add(product);
            }
            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Update required " + name);

            var currentProduct = 0;
            var storagePushNthProduct = 100; // push after updating every nth product

            foreach (var product in updateProducts)
            {
                taskReportingController.AddTask(
                    string.Format(
                        "Update {0} {1}/{2}: {3}",
                        name,
                        ++currentProduct,
                        updateProducts.Count,
                        product.Title));

                var uri = string.Format(Uris.Paths.GetUpdateUri(productType), GetProductUri(product));
                var rawResponse = await networkController.Get(uri);

                var dataContent = ProcessRawData(rawResponse);

                if (dataContent == null)
                {
                    taskReportingController.ReportWarning(
                        string.Format(
                            "Product {0} doesn't have valid " + name,
                            product.Title));
                    continue;
                }

                var data = Deserialize(dataContent);

                dataCollection.Add(data);

                if (currentProduct % storagePushNthProduct == 0)
                {
                    taskReportingController.AddTask("Save " + name + " to disk");
                    await productStorageController.Push(productType, dataCollection);
                    taskReportingController.CompleteTask();
                }

                if (politenessController != null)
                {
                    taskReportingController.AddTask("Throttle network requests");
                    politenessController.Throttle();
                    taskReportingController.CompleteTask();
                }

                taskReportingController.CompleteTask();
            }

            taskReportingController.AddTask("Save " + name + " to disk");
            await productStorageController.Push(productType, dataCollection);
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();
        }

        internal virtual string GetProductUri(Product product)
        {
            return product.Id.ToString(); // most product updates go by id
        }

        internal virtual string ProcessRawData(string rawData)
        {
            return rawData; // passthrough by default
        }

        internal virtual Type Deserialize(string content)
        {
            return serializationController.Deserialize<Type>(content);
        }
    }
}
