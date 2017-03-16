using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Routing;
using Interfaces.Session;
using Interfaces.Expectation;
using Interfaces.Uri;
using Interfaces.FileDownload;
using Interfaces.TaskStatus;

namespace GOG.Controllers.FileDownload
{
    public class ValidationDownloadFromSourceDelegate : IDownloadFileFromSourceDelegate
    {
        private IRoutingController routingController;
        private ISessionController sessionController;
        private IExpectedDelegate<string> validationExpectedForUriDelegate;
        private IUriController uriController;
        private IFileDownloadController fileDownloadController;
        private ITaskStatusController taskStatusController;

        public ValidationDownloadFromSourceDelegate(
            IRoutingController routingController,
            ISessionController sessionController,
            IExpectedDelegate<string> validationExpectedForUriDelegate,
            IUriController uriController,
            IFileDownloadController fileDownloadController,
            ITaskStatusController taskStatusController)
        {
            this.routingController = routingController;
            this.sessionController = sessionController;
            this.validationExpectedForUriDelegate = validationExpectedForUriDelegate;
            this.uriController = uriController;
            this.fileDownloadController = fileDownloadController;
            this.taskStatusController = taskStatusController;
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, ITaskStatus taskStatus)
        {
            var downloadValidationFileTask = taskStatusController.Create(taskStatus, "Download validation file");

            var resolvedUri = await routingController.TraceRouteAsync(id, sourceUri);

            if (string.IsNullOrEmpty(resolvedUri))
            {
                taskStatusController.Warn(
                    downloadValidationFileTask,
                    $"Skipping validation file for manual url {sourceUri} because product {id}: {title} has no available routes");
                return;
            }

            // only download validation files for uris are expected to produce validation
            // but close download validation files task and don't return earlier
            if (validationExpectedForUriDelegate.Expected(resolvedUri))
            {
                // GOG.com quirk
                // Note: See ManualUrlDownloadFromSourceDelegate for session key explaination
                // Validation files are constructed from direct file Uri to GOG CDN
                // and since those Uris 1) don't contain session key 2) would have contained outdated key
                // we request new session key again. The network operation uses HEAD request, so should be light

                var session = await sessionController.CreateSession(sourceUri);

                var validationUri = uriController.ConcatenateUriWithParameters(
                    resolvedUri,
                    new string[] { session });

                try
                {
                    await fileDownloadController.DownloadFileFromSourceAsync(
                        id,
                        title,
                        validationUri,
                        destination,
                        downloadValidationFileTask);
                }
                catch (Exception ex)
                {
                    taskStatusController.Fail(
                        downloadValidationFileTask,
                        $"Couldn't download validation {sourceUri}, resolved as {resolvedUri} " +
                        $"to {destination} for product {id}: {title}, error message: {ex.Message}");
                }
            }

            taskStatusController.Complete(downloadValidationFileTask);
        }
    }
}
