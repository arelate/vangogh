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
                    "Would not try to get validation file for manual url {0} that has not been routed properly for product {1}: {2}",
                    sourceUri,
                    id,
                    title);
                return;
            }

            // only download validation files for uris are expected to produce validation
            // but close download validation files task and don't return earlier
            if (validationExpectedForUriDelegate.Expected(resolvedUri))
            {
                var session = await sessionController.CreateSession(sourceUri);

                var validationUri = uriController.ConcatenateUriWithParameters(
                    resolvedUri,
                    new string[] { session });

                await fileDownloadController.DownloadFileFromSourceAsync(
                    id,
                    title,
                    validationUri,
                    destination,
                    downloadValidationFileTask);
            }

            taskStatusController.Complete(downloadValidationFileTask);
        }
    }
}
