using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Uris;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    public class RespondToUpdateWishlistedRequestDelegate : IRespondAsyncDelegate
    {
        readonly IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        readonly IDataController<long> wishlistedDataController;
        readonly IActionLogController actionLogController;

        [Dependencies(
            DependencyContext.Default,
            "GOG.Delegates.GetDeserialized.ProductTypes.GetProductsPageResultDeserializedGOGDataAsyncDelegate,GOG.Delegates",
            "Controllers.Data.ProductTypes.WishlistedDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateWishlistedRequestDelegate(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            IActionLogController actionLogController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
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
