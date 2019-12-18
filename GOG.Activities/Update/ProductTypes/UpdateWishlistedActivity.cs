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
        readonly IResponseLogController responseLogController;

        [Dependencies(
            "GOG.Delegates.GetDeserialized.ProductTypes.GetProductsPageResultDeserializedGOGDataAsyncDelegate,GOG.Delegates",
            "Controllers.Data.ProductTypes.WishlistedDataController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public UpdateWishlistedActivity(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            IResponseLogController responseLogController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
            this.responseLogController = responseLogController;
        }

        public async Task ProcessActivityAsync()
        {
            responseLogController.OpenResponseLog("Update Wishlisted");

            responseLogController.StartAction("Request content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserializedAsync(
                Uris.Endpoints.Account.Wishlist);

            responseLogController.CompleteAction();

            responseLogController.StartAction("Save");

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                await wishlistedDataController.UpdateAsync(product.Id);
            }

            responseLogController.CompleteAction();

            await wishlistedDataController.CommitAsync();

            responseLogController.CloseResponseLog();
        }
    }
}
