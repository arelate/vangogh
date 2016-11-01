using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Interfaces.Reporting;
using Interfaces.Download;
using Interfaces.Data;
using Interfaces.Destination;

using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Processing
{
    public class ProcessScheduledDownloadsController : TaskActivityController
    {
        private IDataController<ScheduledDownload> scheduledDownloadsDataController;
        private IDataController<ScheduledValidation> scheduledValidationsDataController;
        private IDownloadController downloadController;
        private IDestinationController destinationController;

        public ProcessScheduledDownloadsController(
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<ScheduledValidation> scheduledValidationsDataController,
            IDownloadController downloadController,
            IDestinationController destinationController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.scheduledDownloadsDataController = scheduledDownloadsDataController;
            this.scheduledValidationsDataController = scheduledValidationsDataController;
            this.downloadController = downloadController;
            this.destinationController = destinationController;
        }

        public override async Task ProcessTask()
        {
            var counter = 0;
            var total = scheduledDownloadsDataController.Count();

            var scheduledDownloadIds = scheduledDownloadsDataController.EnumerateIds().ToArray();

            taskReportingController.StartTask("Process scheduled downloads");
            foreach (var id in scheduledDownloadIds)
            {
                var scheduledDownload = await scheduledDownloadsDataController.GetById(id);

                taskReportingController.StartTask(
                        "Process downloads for product {0}/{1}: {2}",
                        ++counter,
                        total,
                        scheduledDownload.Title);

                var downloadEntries = scheduledDownload.Downloads.ToArray();

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    taskReportingController.StartTask(
                        "Download entry {0}/{1}: {2}",
                        ii + 1,
                        downloadEntries.Length,
                        System.Enum.GetName(typeof(ScheduledDownloadTypes), entry.Type));

                    await downloadController.DownloadFile(entry.Source, entry.Destination);

                    // remove currently downloaded entry
                    scheduledDownload.Downloads.RemoveAt(0);
                    if (scheduledDownload.Downloads.Count == 0)
                        await scheduledDownloadsDataController.Remove(scheduledDownload);
                    else
                        await scheduledDownloadsDataController.Update(scheduledDownload);

                    taskReportingController.CompleteTask();

                    if (entry.Type == ScheduledDownloadTypes.File)
                    {
                        taskReportingController.StartTask("Schedule validation for downloaded file");

                        var scheduledValidation = await scheduledValidationsDataController.GetById(id);
                        if (scheduledValidation == null)
                        {
                            scheduledValidation = new ScheduledValidation();
                            scheduledValidation.Id = scheduledDownload.Id;
                            scheduledValidation.Title = scheduledDownload.Title;
                            scheduledValidation.Files = new List<string>();
                        }

                        var filePath = Path.Combine(entry.Destination,
                            destinationController.GetFilename(entry.Source));

                        scheduledValidation.Files.Add(filePath);
                        await scheduledValidationsDataController.Update(scheduledValidation);

                        taskReportingController.CompleteTask();
                    }
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
