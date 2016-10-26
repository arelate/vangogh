using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.AdditionalDetails;

using Models.Uris;
using Models.ProductCore;

using GOG.Models;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ProductCoreUpdateController<UpdateType, ListType> : TaskActivityController
        where ListType : ProductCore
        where UpdateType : ProductCore 
    {
        internal IProductTypeStorageController productStorageController;
        private ICollectionController collectionController;
        private INetworkController networkController;
        private IThrottleController throttleController;
        internal ISerializationController<string> serializationController;

        private IUpdateUriController updateUriController;
        private IRequiredUpdatesController requiredUpdatesController;
        private ISkipUpdateController skipUpdateController;
        private IDataDecodingController dataDecodingController;
        private IConnectionController connectionController;
        private IAdditionalDetailsController additionalDetailsController;

        // TODO: Break this pattern and implement controllers
        internal ProductTypes updateProductType;
        internal ProductTypes listProductType;
        internal string displayProductName;

        public ProductCoreUpdateController(
            IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            IRequiredUpdatesController requiredUpdatesController,
            ISkipUpdateController skipUpdateController,
            IDataDecodingController dataDecodingController,
            IConnectionController connectionController,
            IAdditionalDetailsController additionalDetailsController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.productStorageController = productStorageController;
            this.collectionController = collectionController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            this.throttleController = throttleController;

            this.updateUriController = updateUriController;
            this.requiredUpdatesController = requiredUpdatesController;
            this.skipUpdateController = skipUpdateController;
            this.dataDecodingController = dataDecodingController;
            this.connectionController = connectionController;
            this.additionalDetailsController = additionalDetailsController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing products and " + displayProductName);

            var products = await productStorageController.Pull<ListType>(listProductType);
            var existingData = await productStorageController.Pull<UpdateType>(updateProductType);

            var updateCollection = new List<UpdateType>(existingData);

            taskReportingController.CompleteTask();

            var updateProducts = await GetUpdates(products, updateCollection);

            taskReportingController.StartTask("Update required " + displayProductName);

            var currentProduct = 0;
            var storagePushNthProduct = 1; // push after updating every nth product
            var somethingChanged = false;

            foreach (var id in updateProducts)
            {
                var product = collectionController.Find(products, p => p.Id == id);

                taskReportingController.StartTask(
                    string.Format(
                        "Update {0} {1}/{2}: {3}",
                        displayProductName,
                        ++currentProduct,
                        updateProducts.Count,
                        product.Title));

                var content = await GetContent(product);
                if (content == null) continue;

                var data = serializationController.Deserialize<UpdateType>(content);

                if (data != null)
                {
                    connectionController?.Connect(data, product);

                    additionalDetailsController?.AddDetails(data, content);

                    updateCollection.Add(data);
                    somethingChanged = true;
                }

                if (currentProduct % storagePushNthProduct == 0)
                    await PushChanges(updateCollection);

                if (throttleController != null)
                {
                    taskReportingController.StartTask("Throttle network requests");
                    throttleController.Throttle();
                    taskReportingController.CompleteTask();
                }

                taskReportingController.CompleteTask();
            }

            if (somethingChanged) await PushChanges(updateCollection);

            taskReportingController.CompleteTask();
        }

        private async Task<List<long>> GetUpdates(IEnumerable<ListType> products, IEnumerable<UpdateType> updateCollection)
        {
            taskReportingController.StartTask("Get a list of updates for " + displayProductName);

            var updateProducts = new List<long>();

            if (requiredUpdatesController != null)
            {
                foreach (var id in await requiredUpdatesController.GetRequiredUpdates())
                {
                    var product = collectionController.Find(products, p => p.Id == id);
                    if (product != null) updateProducts.Add(product.Id);
                }
            }

            foreach (var product in products)
            {
                if (product == null) continue;

                if (skipUpdateController != null &&
                    skipUpdateController.SkipUpdate(product)) continue;

                var foundGameProductData = collectionController.Find(updateCollection, p => p != null && p.Id == product.Id);
                if (foundGameProductData == null &&
                    !updateProducts.Contains(product.Id))
                    updateProducts.Add(product.Id);
            }

            taskReportingController.CompleteTask();

            return updateProducts;
        }

        private async Task<string> GetContent(ListType product)
        {
            var uri = string.Format(
                Uris.Paths.GetUpdateUri(updateProductType), 
                updateUriController.GetUpdateUri(product));

            var rawResponse = await networkController.Get(uri);

            var content = dataDecodingController != null ?
                dataDecodingController.DecodeData(rawResponse) :
                rawResponse;

            if (content == null)
                taskReportingController.ReportWarning(
                    string.Format(
                        "Product {0} doesn't have valid " + displayProductName,
                        product.Title));

            return content;
        }

        private async Task PushChanges(IList<UpdateType> updateCollection)
        {
            taskReportingController.StartTask("Save " + displayProductName + " to disk");
            await productStorageController.Push(updateProductType, updateCollection);
            taskReportingController.CompleteTask();
        }
    }
}
