using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
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
            "GOG.Delegates.Data.Models.ProductTypes.UpdateAccountProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.CommitAccountProductsAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateAccountProductsRequestDelegate(
            IGetPageResultsAsyncDelegate<AccountProductsPageResult> 
                getAccountProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<AccountProductsPageResult>, AccountProduct>
                itemizeAccountProductsPageResultsDelegate,
            IUpdateAsyncDelegate<AccountProduct> updateAccountProductsAsyncDelegate,
            ICommitAsyncDelegate commitAccountProductsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getAccountProductsPageResultsAsyncDelegate,
                itemizeAccountProductsPageResultsDelegate,
                updateAccountProductsAsyncDelegate,
                commitAccountProductsAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}