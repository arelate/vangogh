using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;


using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class ApiProductsDataController : DataController<ApiProduct>
    {
        [Dependencies(
            "GOG.Controllers.Stash.ProductTypes.ApiProductsStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertApiProductToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.ApiProductsRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindApiProductDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ApiProductsDataController(
            IStashController<List<ApiProduct>> apiProductsStashController,
            IConvertDelegate<ApiProduct, long> convertApiProductsToIndexDelegate,
            IRecordsController<long> apiProductsRecordsIndexController,
            IFindDelegate<ApiProduct> findApiProductDelegate,
            IActionLogController actionLogController) :
            base(
                apiProductsStashController,
                convertApiProductsToIndexDelegate,
                apiProductsRecordsIndexController,
                findApiProductDelegate,
                actionLogController)
        {
            // ...
        }
    }
}