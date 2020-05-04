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
    [RespondsToRequests(Method = "update", Collection = "products")]
    public class UpdateProductsAsyncDelegate :
        UpdatePageResultAsyncDelegate<ProductsPageResult, Product>
    {
        [Dependencies(
            typeof(GetProductsPageResultsAsyncDelegate),
            typeof(ItemizeProductsPageResultProductsDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.UpdateProductsAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.CommitProductsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateProductsAsyncDelegate(
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