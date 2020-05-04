using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using GOG.Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Server;
using Models.Uris;

namespace GOG.Delegates.Server.Update
{
    public class UpdateWishlistedAsyncDelegate : IProcessAsyncDelegate
    {
        private readonly IGetDataAsyncDelegate<Models.ProductsPageResult, string> getProductsPageResultDelegate;
        private readonly IUpdateAsyncDelegate<long> updateWishlistedAsyncDelegate;
        private readonly ICommitAsyncDelegate commitWishlistedAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetProductsPageResultDeserializedGOGDataAsyncDelegate),
            typeof(UpdateWishlistedAsyncDelegate),
            typeof(CommitWishlistedAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public UpdateWishlistedAsyncDelegate(
            IGetDataAsyncDelegate<Models.ProductsPageResult, string> getProductsPageResultDelegate,
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

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Update Wishlisted");

            startDelegate.Start("Request content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDataAsync(
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