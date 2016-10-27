using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Products
{
    public class ApiProductUpdateController : ProductCoreUpdateController<ApiProduct, Product>
    {
        public ApiProductUpdateController(
            //IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            ITaskReportingController taskReportingController) :
            base(
                //productStorageController,
                collectionController,
                networkController,
                serializationController,
                throttleController,
                updateUriController,
                null, // requiredUpdatesController
                null, // skipUpdateController
                null, // dataDecodingController
                null, // connectionController
                null, // additionalDetailsController
                taskReportingController)
        {
            updateProductType = ProductTypes.ApiProduct;
            listProductType = ProductTypes.Product;

            displayProductName = "API product";
        }
    }
}
