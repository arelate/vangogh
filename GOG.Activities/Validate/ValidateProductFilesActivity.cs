using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Validation;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Data;
using Interfaces.Enumeration;
using Interfaces.Routing;
using Interfaces.Status;
using Interfaces.ValidationResult;

using Models.ValidationResult;

using GOG.Models;

namespace GOG.Activities.Validate
{
    public class ValidateProductFilesActivity : Activity
    {
        private IGetDirectoryDelegate productFileDirectoryDelegate;
        private IGetFilenameDelegate productFileFilenameDelegate;
        private IEnumerateDelegate<string> validationFileEnumerateDelegate;
        private IFileValidationController fileValidationController;
        private IDataController<ValidationResult> validationResultsDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<GameDetails> manualUrlsEnumerationController;
        private IEnumerateIdsDelegate productEnumerateDelegate;
        private IRoutingController routingController;

        public ValidateProductFilesActivity(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IEnumerateDelegate<string> validationFileEnumerateDelegate,
            IFileValidationController fileValidationController,
            IDataController<ValidationResult> validationResultsDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<GameDetails> manualUrlsEnumerationController,
            IEnumerateIdsDelegate productEnumerateDelegate,
            IRoutingController routingController,
            IStatusController statusController) :
            base(statusController)
        {
            this.productFileDirectoryDelegate = productFileDirectoryDelegate;
            this.productFileFilenameDelegate = productFileFilenameDelegate;
            this.validationFileEnumerateDelegate = validationFileEnumerateDelegate;
            this.fileValidationController = fileValidationController;
            this.validationResultsDataController = validationResultsDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.manualUrlsEnumerationController = manualUrlsEnumerationController;

            this.productEnumerateDelegate = productEnumerateDelegate;
            this.routingController = routingController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var validateProductsTask = statusController.Create(status, "Validate products");

            var current = 0;

            var validateProductsList = productEnumerateDelegate.EnumerateIds();
            var validateProductsCount = validateProductsList.Count();

            foreach (var id in validateProductsList)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);
                var validationResults = await validationResultsDataController.GetByIdAsync(id);

                if (validationResults == null)
                    validationResults = new ValidationResult()
                    {
                        Id = id,
                        Title = gameDetails.Title
                    };

                statusController.UpdateProgress(validateProductsTask,
                    ++current,
                    validateProductsCount,
                    gameDetails.Title);

                var localFiles = new List<string>();

                var getLocalFilesTask = statusController.Create(validateProductsTask, "Enumerate local product files");
                foreach (var manualUrl in manualUrlsEnumerationController.Enumerate(gameDetails))
                {
                    var resolvedUri = await routingController.TraceRouteAsync(id, manualUrl);

                    // use directory from source and file from resolved URI
                    var localFile = Path.Combine(
                        productFileDirectoryDelegate.GetDirectory(manualUrl),
                        productFileFilenameDelegate.GetFilename(resolvedUri));

                    localFiles.Add(localFile);
                }
                statusController.Complete(getLocalFilesTask);


                // check if current validation results allow us to skip validating current product
                // otherwise - validate all the files again

                // ...

                var fileValidationResults = new List<IFileValidationResult>(localFiles.Count);

                var validateFilesTask = statusController.Create(
                    validateProductsTask,
                    "Validate product files");

                var currentFile = 0;

                foreach (var localFile in localFiles)
                {
                    statusController.UpdateProgress(validateFilesTask,
                        ++currentFile,
                        localFiles.Count,
                        localFile);

                    var validationFile = validationFileEnumerateDelegate.Enumerate(localFile).Single();

                    try
                    {
                        fileValidationResults.Add(await fileValidationController.ValidateFileAsync(
                            localFile,
                            validationFile,
                            validateFilesTask));
                    }
                    catch (Exception ex)
                    {
                        statusController.Fail(validateProductsTask,
                            $"{localFile}: {ex.Message}");
                    }
                }

                statusController.Complete(validateFilesTask);

                validationResults.Files = fileValidationResults.ToArray();

                var updateValidationResultsTask = statusController.Create(validateProductsTask, "Update validation results");
                await validationResultsDataController.UpdateAsync(validateProductsTask, validationResults);
                statusController.Complete(updateValidationResultsTask);
            }

            statusController.Complete(validateProductsTask);
        }
    }
}
