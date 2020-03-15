using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductDownloadsDataController : DataController<ProductDownloads>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.ProductDownloadsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductDownloadsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductDownloadsRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindProductDownloadsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductDownloadsDataController(
            IStashController<List<ProductDownloads>> productDownloadsStashController,
            IConvertDelegate<ProductDownloads, long> convertProductDownloadsToIndexDelegate,
            IRecordsController<long> productDownloadsRecordsIndexController,
            IFindDelegate<ProductDownloads> productDownloadsFindDelegate,
            IActionLogController actionLogController) :
            base(
                productDownloadsStashController,
                convertProductDownloadsToIndexDelegate,
                productDownloadsRecordsIndexController,
                productDownloadsFindDelegate,
                actionLogController)
        {
            // ...
        }
    }
}