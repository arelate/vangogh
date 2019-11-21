using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ProductRoutes;

namespace Controllers.Data.ProductTypes
{
    public class ProductRoutesDataController : DataController<ProductRoutes>
    {
        public ProductRoutesDataController(
            IStashController<Dictionary<long, ProductRoutes>> productRoutesStashController,
            IConvertDelegate<ProductRoutes, long> convertProductRoutesToIndexDelegate,
            IRecordsController<long> productRoutesRecordsIndexController,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                productRoutesStashController,
                convertProductRoutesToIndexDelegate,
                productRoutesRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}