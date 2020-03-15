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
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.ProductScreenshotsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductScreenshotsRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindProductScreenshotsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductScreenshotsDataController(
            IStashController<List<ProductScreenshots>> productScreenshotsStashController,
            IConvertDelegate<ProductScreenshots, long> convertProductScreenshotsToIndexDelegate,
            IRecordsController<long> productScreenshotsRecordsIndexController,
            IFindDelegate<ProductScreenshots> findProductScreenshotsDelegate,
            IActionLogController actionLogController) :
            base(
                productScreenshotsStashController,
                convertProductScreenshotsToIndexDelegate,
                productScreenshotsRecordsIndexController,
                findProductScreenshotsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}