using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Network;
using Interfaces.Data;
using Interfaces.Status;

using Models.Uris;

namespace GOG.Activities.UpdateData
{
    public class WishlistedUpdateActivity : Activity
    {
        private IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        private IDataController<long> wishlistedDataController;

        public WishlistedUpdateActivity(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            var updateWishlistTask = statusController.Create(status, "Update wishlisted products");

            var requestContentTask = statusController.Create(updateWishlistTask, "Request wishlist content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserializedAsync(
                requestContentTask,
                Uris.Paths.Account.Wishlist);

            statusController.Complete(requestContentTask);

            var saveDataTask = statusController.Create(updateWishlistTask, "Add new wishlisted products");

            var wishlistedIds = new List<long>();

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                wishlistedIds.Add(product.Id);
            }

            await wishlistedDataController.UpdateAsync(saveDataTask, wishlistedIds.ToArray());

            statusController.Complete(saveDataTask);

            statusController.Complete(updateWishlistTask);
        }
    }
}
