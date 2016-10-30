using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.Data;

using Models.Uris;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Wishlisted
{
    public class WishlistedUpdateController: TaskActivityController
    {
        private INetworkController networkController;
        private IExtractionController gogDataExtractionController;
        private ISerializationController<string> serializationController;
        private IDataController<long> wishlistedDataController;

        public WishlistedUpdateController(
            INetworkController networkController,
            IExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController,
            IDataController<long> wishlistedDataController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.networkController = networkController;
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
            this.wishlistedDataController = wishlistedDataController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Request wishlist content");
            var wishlistedContent = await networkController.Get(Uris.Paths.Account.Wishlist);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Extract wishlist data");
            var wishlistedGogDataCollection = gogDataExtractionController.ExtractMultiple(wishlistedContent);
            if (wishlistedGogDataCollection == null)
            {
                taskReportingController.ReportFailure("Extracted wishlist data is null.");
                return;
            }
            if (wishlistedGogDataCollection.Count() == 0)
            {
                taskReportingController.ReportFailure("Extracted wishlist data is empty.");
                return;
            }
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Deserialize wishlist data");

            var wishlistedGogData = wishlistedGogDataCollection.First();
            var wishlistedProductPageResult = serializationController.Deserialize<Models.ProductsPageResult>(wishlistedGogData);

            if (wishlistedProductPageResult == null ||
                wishlistedProductPageResult.Products == null)
            {
                taskReportingController.ReportFailure("Failed to deserialize wishlist data");
                return;
            }

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Save wishlist data");

            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                if (wishlistedDataController.Contains(product.Id)) continue;

                await wishlistedDataController.Update(product.Id);
            }

            taskReportingController.CompleteTask();
        }
    }
}
