using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Attributes;

using GOG.Models;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateProductsActivity : UpdatePageResultActivity<ProductsPageResult, Product>
    {
        [Dependencies(
            "GOG.Delegates.GetPageResults.ProductTypes.GetProductsPageResultsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeProductsPageResultProductsDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Records.Session.SessionRecordsController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdateProductsActivity(
            IGetPageResultsAsyncDelegate<ProductsPageResult> getProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<ProductsPageResult>, Product> itemizeProductsPageResultsDelegate,
            IDataController<Product> productsDataController,
            IRecordsController<string> activityRecordsController,
            IActionLogController actionLogController) :
            base(
                getProductsPageResultsAsyncDelegate,
                itemizeProductsPageResultsDelegate,
                productsDataController,
                activityRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}