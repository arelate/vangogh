using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ApiProductsDataController : DataController<ApiProduct>
    {
        public ApiProductsDataController(
            IStashController<Dictionary<long, ApiProduct>> apiProductsStashController,
            IConvertDelegate<ApiProduct, long> convertApiProductsToIndexDelegate,
            IRecordsController<long> apiProductsRecordsIndexController,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                apiProductsStashController,
                convertApiProductsToIndexDelegate,
                apiProductsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}