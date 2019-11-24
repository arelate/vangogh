using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

using Models.ProductCore;

using GOG.Models;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateProductsActivity : UpdatePageResultActivity<ProductsPageResult, Product>
    {
        public UpdateProductsActivity(IGetPageResultsAsyncDelegate<ProductsPageResult> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<ProductsPageResult>, Product> itemizePageResultsDelegate,
            IDataController<Product> productsDataController,
            IRecordsController<string> activityRecordsController,
            IStatusController statusController) :
            base(
                getPageResultsAsyncDelegate,
                itemizePageResultsDelegate,
                productsDataController,
                activityRecordsController,
                statusController)
        {
            // ...
        }
    }
}