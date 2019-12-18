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
    public class UpdateAccountProductsActivity : UpdatePageResultActivity<AccountProductsPageResult, AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.GetPageResults.ProductTypes.GetAccountProductsPageResultsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeAccountProductsPageResultProductsDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "Controllers.Records.Session.SessionRecordsController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public UpdateAccountProductsActivity(
            IGetPageResultsAsyncDelegate<AccountProductsPageResult> getAccountProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<AccountProductsPageResult>, AccountProduct> itemizeAccountProductsPageResultsDelegate,
            IDataController<AccountProduct> accountProductsDataController,
            IRecordsController<string> activityRecordsController,
            IResponseLogController responseLogController) :
            base(
                getAccountProductsPageResultsAsyncDelegate,
                itemizeAccountProductsPageResultsDelegate,
                accountProductsDataController,
                activityRecordsController,
                responseLogController)
        {
            // ...
        }
    }
}