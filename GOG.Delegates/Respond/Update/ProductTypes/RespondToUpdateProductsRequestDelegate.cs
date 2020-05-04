using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;
using Delegates.Activities;
using GOG.Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    // TODO: We should generate those files
    [RespondsToRequests(Method = "update", Collection = "products")]
    public class RespondToUpdateProductsRequestDelegate :
        RespondToUpdatePageResultRequestDelegate<ProductsPageResult, Product>
    {
        [Dependencies(
            typeof(GetProductsPageResultsAsyncDelegate),
            typeof(Itemize.ItemizeProductsPageResultProductsDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateProductsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitProductsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateProductsRequestDelegate(
            IGetDataAsyncDelegate<IList<ProductsPageResult>, string> 
                getProductsPageResultsAsyncDelegate,
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