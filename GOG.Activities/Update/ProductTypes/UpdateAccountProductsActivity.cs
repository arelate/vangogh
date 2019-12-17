using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

using GOG.Models;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateAccountProductsActivity : UpdatePageResultActivity<AccountProductsPageResult, AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.GetPageResults.ProductTypes.GetAccountProductsPageResultsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeAccountProductsPageResultProductsDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "Controllers.Records.Session.SessionRecordsController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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