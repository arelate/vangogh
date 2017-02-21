using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Connection;
using Interfaces.Throttle;
using Interfaces.UpdateUri;
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

        private IGetDeserializedDelegate<UpdateType> getDeserializedDelegate;
        private IThrottleController throttleController;

        private IGetUpdateUriDelegate<ListType> getUpdateUriDelegate;
        private IConnectDelegate<UpdateType, ListType> connectDelegate;

        private ProductTypes updateProductType;

        private string updateTypeDescription;

        public ProductCoreUpdateController(
            ProductTypes updateProductType,
            IDataController<UpdateType> updateTypeDataController,
            IDataController<ListType> listTypeDataController,
            IDataController<long> updatedDataController,
            IGetDeserializedDelegate<UpdateType> getDeserializedDelegate,
            IGetUpdateUriDelegate<ListType> getUpdateUriDelegate,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController,
            IThrottleController throttleController = null,
            IConnectDelegate<UpdateType, ListType> connectDelegate = null) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.updateTypeDataController = updateTypeDataController;
            this.listTypeDataController = listTypeDataController;
            this.updatedDataController = updatedDataController;

            this.getDeserializedDelegate = getDeserializedDelegate;
            this.throttleController = throttleController;

            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.connectDelegate = connectDelegate;

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

            var addUpdatedTask = taskStatusController.Create(updateProductsTask, "Add updated or new products");

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
                    getUpdateUriDelegate.GetUpdateUri(product));

                var data = await getDeserializedDelegate.GetDeserialized(uri);

                if (data != null)
                {
                    connectDelegate?.Connect(data, product);
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
