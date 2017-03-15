﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Connection;
using Interfaces.Throttle;
using Interfaces.UpdateUri;
using Interfaces.UpdateIdentity;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductCore;
using Models.Units;

namespace GOG.TaskActivities.UpdateData
{
    public class ProductCoreUpdateController<UpdateType, ListType> :
        TaskActivityController
        where ListType : ProductCore
        where UpdateType : ProductCore
    {
        private IDataController<UpdateType> updateTypeDataController;
        private IDataController<ListType> listTypeDataController;
        private IDataController<long> updatedDataController;

        private IGetDeserializedDelegate<UpdateType> getDeserializedDelegate;
        private IThrottleController throttleController;

        private IGetUpdateIdentityDelegate<ListType> getUpdateIdentityDelegate;
        private IConnectDelegate<UpdateType, ListType> connectDelegate;

        private string updateProductParameter;
        private IGetUpdateUriDelegate<string> getUpdateUriDelegate;

        private string updateTypeDescription;

        public ProductCoreUpdateController(
            string updateProductParameter,
            IGetUpdateUriDelegate<string> getUpdateUriDelegate,
            IDataController<UpdateType> updateTypeDataController,
            IDataController<ListType> listTypeDataController,
            IDataController<long> updatedDataController,
            IGetDeserializedDelegate<UpdateType> getDeserializedDelegate,
            IGetUpdateIdentityDelegate<ListType> getUpdateIdentityDelegate,
            ITaskStatusController taskStatusController,
            IThrottleController throttleController = null,
            IConnectDelegate<UpdateType, ListType> connectDelegate = null) :
            base(taskStatusController)
        {
            this.updateTypeDataController = updateTypeDataController;
            this.listTypeDataController = listTypeDataController;
            this.updatedDataController = updatedDataController;

            this.getDeserializedDelegate = getDeserializedDelegate;
            this.throttleController = throttleController;

            this.getUpdateIdentityDelegate = getUpdateIdentityDelegate;
            this.connectDelegate = connectDelegate;

            this.updateProductParameter = updateProductParameter;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            updateTypeDescription = typeof(UpdateType).Name;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
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
                if (product == null) continue;

                taskStatusController.UpdateProgress(
                    updateProductsTask,
                    ++currentProduct,
                    updatedProducts.Count,
                    product.Title,
                    ProductUnits.Products);

                var updateIdentity = getUpdateIdentityDelegate.GetUpdateIdentity(product);
                if (string.IsNullOrEmpty(updateIdentity)) continue;

                var uri = string.Format(
                    getUpdateUriDelegate.GetUpdateUri(updateProductParameter),
                    updateIdentity);

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