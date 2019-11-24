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
    public class UpdateAccountProductsActivity : UpdatePageResultActivity<AccountProductsPageResult, AccountProduct>
    {
        public UpdateAccountProductsActivity(
            IGetPageResultsAsyncDelegate<AccountProductsPageResult> getAccountProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<AccountProductsPageResult>, AccountProduct> itemizeAccountProductsPageResultsDelegate,
            IDataController<AccountProduct> accountProductsDataController,
            IRecordsController<string> activityRecordsController,
            IStatusController statusController) :
            base(
                getAccountProductsPageResultsAsyncDelegate,
                itemizeAccountProductsPageResultsDelegate,
                accountProductsDataController,
                activityRecordsController,
                statusController)
        {
            // ...
        }
    }
}