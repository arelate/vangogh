using System.Collections.Generic;
using Attributes;
using Delegates.Activities;
using GOG.Delegates.Data.Models.ProductTypes;
using GOG.Delegates.Itemizations;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Server.Update
{
    // TODO: We should generate those files
    [RespondsToRequests(Method = "update", Collection = "accountproducts")]
    public class UpdateAccountProductsAsyncDelegate :
        UpdatePageResultAsyncDelegate<AccountProductsPageResult, AccountProduct>
    {
        [Dependencies(
            typeof(GetAccountProductsPageResultsAsyncDelegate),
            typeof(ItemizeAccountProductsPageResultProductsDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateAccountProductsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitAccountProductsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateAccountProductsAsyncDelegate(
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