using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Controllers.Data;

using Interfaces.Status;

using Attributes;

using Models.Uris;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateWishlistedActivity : Activity
    {
        readonly IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        readonly IDataController<long> wishlistedDataController;

        [Dependencies(
            "GOG.Delegates.GetDeserialized.ProductTypes.GetProductsPageResultDeserializedGOGDataAsyncDelegate,GOG.Delegates",
            "Controllers.Data.ProductTypes.WishlistedDataController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public UpdateWishlistedActivity(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
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
                await wishlistedDataController.UpdateAsync(product.Id, saveDataTask);
            }

            await statusController.CompleteAsync(saveDataTask);

            await wishlistedDataController.CommitAsync(updateWishlistTask);

            await statusController.CompleteAsync(updateWishlistTask);
        }
    }
}
