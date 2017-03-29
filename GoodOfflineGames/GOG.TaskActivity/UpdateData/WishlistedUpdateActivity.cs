using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Network;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.Uris;

namespace GOG.Activities.UpdateData
{
    public class WishlistedUpdateActivity : Activity
    {
        private IGetDeserializedDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        private IDataController<long> wishlistedDataController;

        public WishlistedUpdateActivity(
            IGetDeserializedDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
        }

        public override async Task ProcessActivityAsync(ITaskStatus taskStatus)
        {
            var updateWishlistTask = taskStatusController.Create(taskStatus, "Update wishlisted products");

            var requestContentTask = taskStatusController.Create(updateWishlistTask, "Request wishlist content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserialized(
                requestContentTask,
                Uris.Paths.Account.Wishlist);

            taskStatusController.Complete(requestContentTask);

            var saveDataTask = taskStatusController.Create(updateWishlistTask, "Add new wishlisted products");

            var wishlistedIds = new List<long>();

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                wishlistedIds.Add(product.Id);
            }

            await wishlistedDataController.UpdateAsync(saveDataTask, wishlistedIds.ToArray());

            taskStatusController.Complete(saveDataTask);

            taskStatusController.Complete(updateWishlistTask);
        }
    }
}
