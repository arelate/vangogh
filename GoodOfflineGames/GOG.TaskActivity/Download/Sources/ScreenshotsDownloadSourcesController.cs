using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductScreenshots;

namespace GOG.TaskActivities.Download.Sources
{
    public class ScreenshotsDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> scheduledScreenshotsUpdatesDataController;
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IImageUriController screenshotUriController;
        private ITaskStatusController taskStatusController;

        public ScreenshotsDownloadSourcesController(
            IDataController<long> scheduledScreenshotsUpdatesDataController,
            IDataController<ProductScreenshots> screenshotsDataController,
            IImageUriController screenshotUriController,
            ITaskStatusController taskStatusController)
        {
            this.scheduledScreenshotsUpdatesDataController = scheduledScreenshotsUpdatesDataController;
            this.screenshotsDataController = screenshotsDataController;
            this.screenshotUriController = screenshotUriController;
            this.taskStatusController = taskStatusController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus)
        {
            var processUpdatesTask = taskStatusController.Create(taskStatus, "Process scheduled screenshot updates");

            var screenshotsSources = new Dictionary<long, IList<string>>();
            var counter = 0;
            var total = scheduledScreenshotsUpdatesDataController.Count();

            var processProductUpdateTask = taskStatusController.Create(processUpdatesTask, "Process product screenshots update");

            foreach (var id in scheduledScreenshotsUpdatesDataController.EnumerateIds())
            {
                var screenshot = await screenshotsDataController.GetByIdAsync(id);

                if (screenshot == null)
                {
                    taskStatusController.Warn(processProductUpdateTask, "Screenshots scheduled to be updated do not exist");
                    continue;
                }

                taskStatusController.UpdateProgress(processUpdatesTask, ++counter, total, screenshot.Title);

                screenshotsSources.Add(id, new List<string>());

                foreach (var uri in screenshot.Uris)
                    screenshotsSources[id].Add(screenshotUriController.ExpandUri(uri));
            }

            taskStatusController.Complete(processProductUpdateTask);

            var clearUpdatesTask = taskStatusController.Create(processUpdatesTask, "Clear scheduled screenshot updates");
            await scheduledScreenshotsUpdatesDataController.RemoveAsync(
                clearUpdatesTask,
                scheduledScreenshotsUpdatesDataController.EnumerateIds().ToArray());
            taskStatusController.Complete(clearUpdatesTask);

            taskStatusController.Complete(processUpdatesTask);

            return screenshotsSources;
        }
    }
}
