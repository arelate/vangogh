using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;
using Delegates.Activities;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    // TODO: We should generate those files
    [RespondsToRequests(Method = "update", Collection = "accountproducts")]
    public class RespondToUpdateAccountProductsRequestDelegate :
        RespondToUpdatePageResultRequestDelegate<AccountProductsPageResult, AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.GetPageResults.ProductTypes.GetAccountProductsPageResultsAsyncDelegate),
            typeof(Itemize.ItemizeAccountProductsPageResultProductsDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateAccountProductsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitAccountProductsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateAccountProductsRequestDelegate(
            IGetDataAsyncDelegate<IList<AccountProductsPageResult>, string> 
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