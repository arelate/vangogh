using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Interfaces.Activity;

using Attributes;

using Models.Uris;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateWishlistedActivity : IActivity
    {
        readonly IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        readonly IDataController<long> wishlistedDataController;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "GOG.Delegates.GetDeserialized.ProductTypes.GetProductsPageResultDeserializedGOGDataAsyncDelegate,GOG.Delegates",
            "Controllers.Data.ProductTypes.WishlistedDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdateWishlistedActivity(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            IActionLogController actionLogController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
            this.actionLogController = actionLogController;
        }

        public async Task ProcessActivityAsync()
        {
            actionLogController.StartAction("Update Wishlisted");

            actionLogController.StartAction("Request content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserializedAsync(
                Uris.Endpoints.Account.Wishlist);

            actionLogController.CompleteAction();

            actionLogController.StartAction("Save");

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                await wishlistedDataController.UpdateAsync(product.Id);
            }

            actionLogController.CompleteAction();

            await wishlistedDataController.CommitAsync();

            actionLogController.CompleteAction();
        }
    }
}
