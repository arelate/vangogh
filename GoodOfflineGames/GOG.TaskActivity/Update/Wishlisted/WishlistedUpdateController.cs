using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.Uris;

namespace GOG.TaskActivities.Update.Wishlisted
{
    public class WishlistedUpdateController : TaskActivityController
    {
        private IGetDeserializedDelegate<Models.ProductsPageResult> getProductsPageResultDelegate;
        private IDataController<long> wishlistedDataController;

        public WishlistedUpdateController(
            IGetDeserializedDelegate<Models.ProductsPageResult> getProductsPageResultDelegate,
            IDataController<long> wishlistedDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.getProductsPageResultDelegate = getProductsPageResultDelegate;
            this.wishlistedDataController = wishlistedDataController;
        }

        public override async Task ProcessTaskAsync()
        {
            var updateWishlistTask = taskStatusController.Create(taskStatus, "Update wishlisted products");

            var requestContentTask = taskStatusController.Create(updateWishlistTask, "Request wishlist content");

            var wishlistedProductPageResult = await getProductsPageResultDelegate.GetDeserialized(
                Uris.Paths.Account.Wishlist);

            taskStatusController.Complete(requestContentTask);

            var saveDataTask = taskStatusController.Create(updateWishlistTask, "Add new wishlisted products");

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                await wishlistedDataController.UpdateAsync(saveDataTask, product.Id);
            }

            taskStatusController.Complete(saveDataTask);

            taskStatusController.Complete(updateWishlistTask);
        }
    }
}
