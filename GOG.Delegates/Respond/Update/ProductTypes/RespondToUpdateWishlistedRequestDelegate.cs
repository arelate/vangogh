using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using Models.Uris;
using GOG.Interfaces.Delegates.GetDeserialized;
using Delegates.Data.Models.ProductTypes;
using Delegates.Activities; 

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    public class RespondToUpdateWishlistedRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        private readonly IUpdateAsyncDelegate<long> updateWishlistedAsyncDelegate;
        private readonly ICommitAsyncDelegate commitWishlistedAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GOG.Delegates.GetDeserialized.ProductTypes.GetProductsPageResultDeserializedGOGDataAsyncDelegate),
            typeof(UpdateWishlistedAsyncDelegate),
            typeof(CommitWishlistedAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateWishlistedRequestDelegate(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IUpdateAsyncDelegate<long> updateWishlistedAsyncDelegate,
            ICommitAsyncDelegate commitWishlistedAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.updateWishlistedAsyncDelegate = updateWishlistedAsyncDelegate;
            this.commitWishlistedAsyncDelegate = commitWishlistedAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Update Wishlisted");

            startDelegate.Start("Request content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserializedAsync(
                Uris.Endpoints.Account.Wishlist);

            completeDelegate.Complete();

            startDelegate.Start("Save");

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                await updateWishlistedAsyncDelegate.UpdateAsync(product.Id);
            }

            completeDelegate.Complete();

            await commitWishlistedAsyncDelegate.CommitAsync();

            completeDelegate.Complete();
        }
    }
}