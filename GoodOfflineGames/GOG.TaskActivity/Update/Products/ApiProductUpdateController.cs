using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.Data;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Products
{
    public class ApiProductUpdateController : ProductCoreUpdateController<ApiProduct, Product>
    {
        public ApiProductUpdateController(
            IDataController<ApiProduct> apiProductDataController,
            IDataController<Product> productsDataController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IUpdateUriController updateUriController,
            ITaskReportingController taskReportingController) :
            base(
                ProductTypes.ApiProduct,
                apiProductDataController,
                productsDataController,
                networkController,
                serializationController,
                null, // throttleController
                updateUriController,
                null, // requiredUpdatesController
                null, // skipUpdateController
                null, // dataDecodingController
                null, // connectionController
                null, // additionalDetailsController
                taskReportingController)
        {
            // ...
        }
    }
}
