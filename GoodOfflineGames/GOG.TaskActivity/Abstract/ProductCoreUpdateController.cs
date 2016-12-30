using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.AdditionalDetails;
using Interfaces.Data;

using Models.Uris;
using Models.ProductCore;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ProductCoreUpdateController<UpdateType, ListType> :
        TaskActivityController
        where ListType : ProductCore
        where UpdateType : ProductCore
    {
        private IDataController<UpdateType> updateTypeDataController;
        private IDataController<ListType> listTypeDataController;

        private INetworkController networkController;
        private IThrottleController throttleController;
        private ISerializationController<string> serializationController;

        private IUpdateUriController updateUriController;
        private IRequiredUpdatesController requiredUpdatesController;
        private ISkipUpdateController skipUpdateController;
        private IDataDecodingController dataDecodingController;
        private IConnectionController connectionController;
        private IAdditionalDetailsController additionalDetailsController;

        private ProductTypes updateProductType;

        private string updateTypeDescription;

        public ProductCoreUpdateController(
            ProductTypes updateProductType,
            IDataController<UpdateType> updateTypeDataController,
            IDataController<ListType> listTypeDataController,
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
            this.updateTypeDataController = updateTypeDataController;
            this.listTypeDataController = listTypeDataController;

            this.networkController = networkController;
            this.serializationController = serializationController;
            this.throttleController = throttleController;

            this.updateUriController = updateUriController;
            this.requiredUpdatesController = requiredUpdatesController;
            this.skipUpdateController = skipUpdateController;
            this.dataDecodingController = dataDecodingController;
            this.connectionController = connectionController;
            this.additionalDetailsController = additionalDetailsController;

            this.updateProductType = updateProductType;
            updateTypeDescription = typeof(UpdateType).Name;
        }

        public override async Task ProcessTaskAsync()
        {
            var updatedProducts = new List<long>();

            taskReportingController.StartTask("Enumerate missing data");

            foreach (var id in listTypeDataController.EnumerateIds())
            {
                if (skipUpdateController != null &&
                    await skipUpdateController.SkipUpdate(id)) continue;

                if (!updateTypeDataController.ContainsId(id))
                    updatedProducts.Add(id);
            }

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Enumerate required data updates");

            if (requiredUpdatesController != null)
                updatedProducts.AddRange(requiredUpdatesController.GetRequiredUpdates());

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Getting updates for data type: " + updateTypeDescription);

            var currentProduct = 0;

            foreach (var id in updatedProducts)
            {
                var product = await listTypeDataController.GetByIdAsync(id);

                taskReportingController.StartTask(
                        "Update {0} {1}/{2}: {3}",
                        updateTypeDescription,
                        ++currentProduct,
                        updatedProducts.Count,
                        product.Title);

                var uri = string.Format(
                    Uris.Paths.GetUpdateUri(updateProductType),
                    updateUriController.GetUpdateUri(product));

                var rawResponse = await networkController.Get(uri);

                var content = dataDecodingController != null ?
                    dataDecodingController.DecodeData(rawResponse) :
                    rawResponse;

                if (content == null)
                {
                    taskReportingController.ReportWarning(
                        string.Format(
                            "Product {0} doesn't have valid associated data of type: " + updateTypeDescription,
                            product.Title));
                    continue;
                }

                var data = serializationController.Deserialize<UpdateType>(content);

                if (data != null)
                {
                    connectionController?.Connect(data, product);
                    additionalDetailsController?.AddDetails(data, content);

                    await updateTypeDataController.UpdateAsync(data);
                }

                // don't throttle if there are less elements than we've set throttling threshold to
                // if throttle - do it for all iterations, but the very last one
                if (updatedProducts.Count > throttleController?.Threshold &&
                    id != updatedProducts[updatedProducts.Count - 1])
                    throttleController?.Throttle();

                taskReportingController.CompleteTask();
            }

            taskReportingController.CompleteTask();
        }
    }
}
