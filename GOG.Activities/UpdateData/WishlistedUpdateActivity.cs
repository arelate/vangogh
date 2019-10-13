using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Index;

using Interfaces.Status;

using Models.Uris;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.UpdateData
{
    public class WishlistedUpdateActivity : Activity
    {
        readonly IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        readonly IIndexController<long> wishlistedDataController;

        public WishlistedUpdateActivity(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IIndexController<long> wishlistedDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateWishlistTask = await statusController.CreateAsync(status, "Update Wishlisted");

            var requestContentTask = await statusController.CreateAsync(updateWishlistTask, "Request content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserializedAsync(
                requestContentTask,
                Uris.Endpoints.Account.Wishlist);

            await statusController.CompleteAsync(requestContentTask);

            var saveDataTask = await statusController.CreateAsync(updateWishlistTask, "Save");

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                await wishlistedDataController.CreateAsync(product.Id, saveDataTask);
            }

            await statusController.CompleteAsync(saveDataTask);

            await wishlistedDataController.CommitAsync(updateWishlistTask);

            await statusController.CompleteAsync(updateWishlistTask);
        }
    }
}
