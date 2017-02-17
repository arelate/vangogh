using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.Uris;
using Models.ProductCore;
using Models.Units;

namespace GOG.TaskActivities.Update
{
    public abstract class ProductCoreUpdateController<UpdateType, ListType> :
        TaskActivityController
        where ListType : ProductCore
        where UpdateType : ProductCore
    {
        private IDataController<UpdateType> updateTypeDataController;
        private IDataController<ListType> listTypeDataController;
        private IDataController<long> updatedDataController;

        //private INetworkController networkController;
        private IGetDelegate getDelegate;
        private IThrottleController throttleController;
        private ISerializationController<string> serializationController;

        private IUpdateUriController updateUriController;
        private IConnectionController connectionController;
        private IAdditionalDetailsController additionalDetailsController;

        private ProductTypes updateProductType;

        private string updateTypeDescription;

        public ProductCoreUpdateController(
            ProductTypes updateProductType,
            IDataController<UpdateType> updateTypeDataController,
            IDataController<ListType> listTypeDataController,
            IDataController<long> updatedDataController,
            IGetDelegate getDelegate,
            ISerializationController<string> serializationController,
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            IConnectionController connectionController,
            IAdditionalDetailsController additionalDetailsController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.updateTypeDataController = updateTypeDataController;
            this.listTypeDataController = listTypeDataController;
            this.updatedDataController = updatedDataController;

            this.getDelegate = getDelegate;
            this.serializationController = serializationController;
            this.throttleController = throttleController;

            this.updateUriController = updateUriController;
            this.connectionController = connectionController;
            this.additionalDetailsController = additionalDetailsController;

            this.updateProductType = updateProductType;
            updateTypeDescription = typeof(UpdateType).Name;
        }

        public override async Task ProcessTaskAsync()
        {
            var updateProductsTask = taskStatusController.Create(taskStatus, "Update products type: " + updateTypeDescription);

            var updatedProducts = new List<long>();

            var missingDataEnumerationTask = taskStatusController.Create(updateProductsTask, "Enumerate missing data");

            foreach (var id in listTypeDataController.EnumerateIds())
            {
                if (!updateTypeDataController.ContainsId(id))
                    updatedProducts.Add(id);
            }

            taskStatusController.Complete(missingDataEnumerationTask);

            var addUpdatedTask = taskStatusController.Create(updateProductsTask, "Enumerate required data updates");

            updatedProducts.AddRange(updatedDataController.EnumerateIds());

            taskStatusController.Complete(addUpdatedTask);

            var currentProduct = 0;

            foreach (var id in updatedProducts)
            {
                var product = await listTypeDataController.GetByIdAsync(id);

                taskStatusController.UpdateProgress(
                    updateProductsTask,
                    ++currentProduct,
                    updatedProducts.Count,
                    product.Title,
                    ProductUnits.Products);

                var uri = string.Format(
                    Uris.Paths.GetUpdateUri(updateProductType),
                    updateUriController.GetUpdateUri(product));

                var content = await getDelegate.Get(uri);

                if (content == null)
                {
                    taskStatusController.Warn(updateProductsTask,
                            "Product {0} doesn't have valid associated data of type: " + updateTypeDescription,
                            product.Title);
                    continue;
                }

                var data = serializationController.Deserialize<UpdateType>(content);

                if (data != null)
                {
                    connectionController?.Connect(data, product);
                    additionalDetailsController?.AddDetails(data, content);

                    await updateTypeDataController.UpdateAsync(updateProductsTask, data);
                }

                // don't throttle if there are less elements than we've set throttling threshold to
                // if throttle - do it for all iterations, but the very last one
                if (updatedProducts.Count > throttleController?.Threshold &&
                    id != updatedProducts[updatedProducts.Count - 1])
                    throttleController?.Throttle(updateProductsTask);
            }

            taskStatusController.Complete(updateProductsTask);
        }
    }
}
