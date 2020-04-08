using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Delegates.Activities;
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
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateAccountProductsRequestDelegate(
            IGetPageResultsAsyncDelegate<AccountProductsPageResult> getAccountProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<AccountProductsPageResult>, AccountProduct>
                itemizeAccountProductsPageResultsDelegate,
            IDataController<AccountProduct> accountProductsDataController,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getAccountProductsPageResultsAsyncDelegate,
                itemizeAccountProductsPageResultsDelegate,
                accountProductsDataController,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}