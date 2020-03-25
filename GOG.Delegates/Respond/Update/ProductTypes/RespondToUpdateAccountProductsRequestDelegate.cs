using System.Collections.Generic;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;


using Attributes;

using GOG.Models;

using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    // TODO: We should generate those files
    [RespondsToRequests(Method = "update", Collection = "accountproducts")]
    public class RespondToUpdateAccountProductsRequestDelegate : 
        RespondToUpdatePageResultRequestDelegate<AccountProductsPageResult, AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.GetPageResults.ProductTypes.GetAccountProductsPageResultsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeAccountProductsPageResultProductsDelegate,GOG.Delegates",
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "Controllers.Records.Session.SessionRecordsController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateAccountProductsRequestDelegate(
            IGetPageResultsAsyncDelegate<AccountProductsPageResult> getAccountProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<AccountProductsPageResult>, AccountProduct> itemizeAccountProductsPageResultsDelegate,
            IDataController<AccountProduct> accountProductsDataController,
            IRecordsController<string> activityRecordsController,
            IActionLogController actionLogController) :
            base(
                getAccountProductsPageResultsAsyncDelegate,
                itemizeAccountProductsPageResultsDelegate,
                accountProductsDataController,
                activityRecordsController,
                actionLogController)
        {
            // ...
        }
    }
}