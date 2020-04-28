using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
// using Interfaces.Controllers.Records;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;
using GOG.Interfaces.Delegates.GetPageResults;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    // TODO: We should generate those files
    [RespondsToRequests(Method = "update", Collection = "products")]
    public class RespondToUpdateProductsRequestDelegate :
        RespondToUpdatePageResultRequestDelegate<ProductsPageResult, Product>
    {
        [Dependencies(
            "GOG.Delegates.GetPageResults.ProductTypes.GetProductsPageResultsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ItemizeProductsPageResultProductsDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.UpdateProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.CommitProductsAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
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