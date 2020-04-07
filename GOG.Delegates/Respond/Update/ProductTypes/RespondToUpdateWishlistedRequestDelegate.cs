using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;


using Attributes;

using Models.Uris;

using GOG.Interfaces.Delegates.GetDeserialized;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    public class RespondToUpdateWishlistedRequestDelegate : IRespondAsyncDelegate
    {
        readonly IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        readonly IDataController<long> wishlistedDataController;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;
        
        [Dependencies(
            "GOG.Delegates.GetDeserialized.ProductTypes.GetProductsPageResultDeserializedGOGDataAsyncDelegate,GOG.Delegates",
            "Controllers.Data.ProductTypes.WishlistedDataController,Controllers",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateWishlistedRequestDelegate(
            IGetDeserializedAsyncDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
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
                await wishlistedDataController.UpdateAsync(product.Id);
            }

            completeDelegate.Complete();

            await wishlistedDataController.CommitAsync();

            completeDelegate.Complete();
        }
    }
}
