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
    public class ProductRoutesDataController : DataController<ProductRoutes>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.ProductRoutesStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertProductRoutesToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductRoutesRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindProductRoutesDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductRoutesDataController(
            IStashController<List<ProductRoutes>> productRoutesStashController,
            IConvertDelegate<ProductRoutes, long> convertProductRoutesToIndexDelegate,
            IRecordsController<long> productRoutesRecordsIndexController,
            IFindDelegate<ProductRoutes> findProductRoutesDelegate,
            IActionLogController actionLogController) :
            base(
                productRoutesStashController,
                convertProductRoutesToIndexDelegate,
                productRoutesRecordsIndexController,
                findProductRoutesDelegate,
                actionLogController)
        {
            // ...
        }
    }
}