using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
// using Interfaces.Controllers.Records;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;
using GOG.Interfaces.Delegates.GetPageResults;
using Delegates.Activities;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    // TODO: We should generate those files
    [RespondsToRequests(Method = "update", Collection = "products")]
    public class RespondToUpdateProductsRequestDelegate :
        RespondToUpdatePageResultRequestDelegate<ProductsPageResult, Product>
    {
        [Dependencies(
            typeof(GOG.Delegates.GetPageResults.ProductTypes.GetProductsPageResultsAsyncDelegate),
            typeof(Itemize.ItemizeProductsPageResultProductsDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateProductsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitProductsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateProductsRequestDelegate(
            IGetPageResultsAsyncDelegate<ProductsPageResult> getProductsPageResultsAsyncDelegate,
            IItemizeDelegate<IList<ProductsPageResult>, Product> itemizeProductsPageResultsDelegate,
            IUpdateAsyncDelegate<Product> updateProductsAsyncDelegate,
            ICommitAsyncDelegate commitProductsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductsPageResultsAsyncDelegate,
                itemizeProductsPageResultsDelegate,
                updateProductsAsyncDelegate,
                commitProductsAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}