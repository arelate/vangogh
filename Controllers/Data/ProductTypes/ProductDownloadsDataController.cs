using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ProductDownloads;

namespace Controllers.Data.ProductTypes
{
    public class ProductDownloadsDataController : DataController<ProductDownloads>
    {
        public ProductDownloadsDataController(
            IStashController<Dictionary<long, ProductDownloads>> productDownloadsStashController,
            IConvertDelegate<ProductDownloads, long> convertProductDownloadsToIndexDelegate,
            IRecordsController<long> productDownloadsRecordsIndexController,
            IStatusController statusController,
            params ICommitAsyncDelegate[] hashesController) :
            base(
                productDownloadsStashController,
                convertProductDownloadsToIndexDelegate,
                productDownloadsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}