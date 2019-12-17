using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Status;

using Attributes;

using GOG.Models;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "products")]
    public class RespondToUpdateProductsRequestAsyncDelegate : RespondToUpdatePageResultRequestDelegate<ProductsPageResult, Product>
    {
        [Dependencies(
            "GOG.Delegates.GetPageResults.ProductTypes.GetProductsPageResultsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeProductsPageResultProductsDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Records.Session.SessionRecordsController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public RespondToUpdateProductsRequestAsyncDelegate(
            IGetPageResultsAsyncDelegate<ProductsPageResult> getProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<ProductsPageResult>, Product> itemizeProductsPageResultsDelegate,
            IDataController<Product> productsDataController,
            IRecordsController<string> activityRecordsController,
            IStatusController statusController) :
            base(
                getProductsPageResultsAsyncDelegate,
                itemizeProductsPageResultsDelegate,
                productsDataController,
                activityRecordsController,
                statusController)
        {
            // ...
        }
    }
}