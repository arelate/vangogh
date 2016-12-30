using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.DownloadSources;
using Interfaces.ImageUri;
using Interfaces.Data;
using Interfaces.Reporting;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.Dependencies.Screenshots
{
    public class ScreenshotsDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> scheduledScreenshotsUpdatesDataController;
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IImageUriController screenshotUriController;
        private ITaskReportingController taskReportingController;

        public ScreenshotsDownloadSourcesController(
            IDataController<long> scheduledScreenshotsUpdatesDataController,
            IDataController<ProductScreenshots> screenshotsDataController,
            IImageUriController screenshotUriController,
            ITaskReportingController taskReportingController)
        {
            this.scheduledScreenshotsUpdatesDataController = scheduledScreenshotsUpdatesDataController;
            this.screenshotsDataController = screenshotsDataController;
            this.screenshotUriController = screenshotUriController;
            this.taskReportingController = taskReportingController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync()
        {
            var screenshotsSources = new Dictionary<long, IList<string>>();
            var counter = 0;
            var total = scheduledScreenshotsUpdatesDataController.Count();

            taskReportingController.StartTask("Process {0} scheduled screenshot updates", total);

            foreach (var id in scheduledScreenshotsUpdatesDataController.EnumerateIds())
            {
                taskReportingController.StartTask("Process screenshot update {0}/{1}", ++counter, total);

                var screenshot = await screenshotsDataController.GetByIdAsync(id);

                if (screenshot == null)
                {
                    taskReportingController.ReportWarning("Screenshots scheduled to be updated do not exist");
                    continue;
                }

                screenshotsSources.Add(id, new List<string>());

                foreach (var uri in screenshot.Uris)
                    screenshotsSources[id].Add(screenshotUriController.ExpandUri(uri));

                taskReportingController.CompleteTask();
            }

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Clear scheduled screenshot updates");
            await scheduledScreenshotsUpdatesDataController.RemoveAsync(scheduledScreenshotsUpdatesDataController.EnumerateIds().ToArray());
            taskReportingController.CompleteTask();

            return screenshotsSources;
        }
    }
}
