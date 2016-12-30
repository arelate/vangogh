using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Collection;
using Interfaces.Extraction;
using Interfaces.Network;
using Interfaces.Data;
using Interfaces.ProductTypes;

using Models.Uris;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Screenshots
{
    public class ScreenshotUpdateController : TaskActivityController
    {
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IDataController<long> scheduledScreenshotsUpdatesDataController;
        private IDataController<Product> productsDataController;
        private INetworkController networkController;
        private IExtractionController screenshotExtractionController;

        public ScreenshotUpdateController(
            IDataController<ProductScreenshots> screenshotsDataController,
            IDataController<long> scheduledScreenshotsUpdatesDataController,
            IDataController<Product> productsDataController,
            INetworkController networkController,
            IExtractionController screenshotExtractionController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.screenshotsDataController = screenshotsDataController;
            this.scheduledScreenshotsUpdatesDataController = scheduledScreenshotsUpdatesDataController;
            this.productsDataController = productsDataController;
            this.networkController = networkController;
            this.screenshotExtractionController = screenshotExtractionController;
        }

        public override async Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Update all products missing screenshots");

            taskReportingController.StartTask("Get a list of updates for product screenshots");

            var productsMissingScreenshots = new List<long>();

            foreach (var id in productsDataController.EnumerateIds())
                if (!screenshotsDataController.ContainsId(id))
                    productsMissingScreenshots.Add(id);

            taskReportingController.CompleteTask();

            var counter = 0;

            foreach (var id in productsMissingScreenshots)
            {
                var product = await productsDataController.GetByIdAsync(id);

                taskReportingController.StartTask(
                        "Update product screenshots {0}/{1}: {2}",
                        ++counter,
                        productsMissingScreenshots.Count,
                        product.Title);

                taskReportingController.StartTask("Request product page containing screenshots information");
                var productPageUri = string.Format(Uris.Paths.GetUpdateUri(ProductTypes.Screenshot), product.Url);
                var productPageContent = await networkController.Get(productPageUri);
                taskReportingController.CompleteTask();

                taskReportingController.StartTask("Exract screenshots from the page");
                var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

                if (extractedProductScreenshots == null) continue;

                var productScreenshots = new ProductScreenshots()
                {
                    Id = product.Id,
                    Uris = new List<string>(extractedProductScreenshots)
                };
                taskReportingController.CompleteTask();

                taskReportingController.StartTask("Update product screenshots");
                await screenshotsDataController.UpdateAsync(productScreenshots);
                taskReportingController.CompleteTask();

                taskReportingController.StartTask("Schedule screenshot files update");
                await scheduledScreenshotsUpdatesDataController.UpdateAsync(product.Id);
                taskReportingController.CompleteTask();

                taskReportingController.CompleteTask();
            }

            taskReportingController.CompleteTask();
        }
    }
}
