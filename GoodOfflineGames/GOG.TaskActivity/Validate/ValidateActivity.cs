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
    public class ValidateActivity : Activity
    {
        private IGetDirectoryDelegate productFileDirectoryDelegate;
        private IGetFilenameDelegate productFileFilenameDelegate;
        private IEnumerateDelegate<string> validationFileEnumerateDelegate;
        private IValidationController validationController;
        private IDataController<ValidationResult> validationResultsDataController;
        private IDataController<GameDetails> gameDetailsDataController;
        private IEnumerateDelegate<GameDetails> manualUrlsEnumerationController;
        private IDataController<long> updatedDataController;
        private IRoutingController routingController;

        public ValidateActivity(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IEnumerateDelegate<string> validationFileEnumerateDelegate,
            IValidationController validationController,
            IDataController<ValidationResult> validationResultsDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateDelegate<GameDetails> manualUrlsEnumerationController,
            IDataController<long> updatedDataController,
            IRoutingController routingController,
            IStatusController statusController) :
            base(statusController)
        {
            this.productFileDirectoryDelegate = productFileDirectoryDelegate;
            this.productFileFilenameDelegate = productFileFilenameDelegate;
            this.validationFileEnumerateDelegate = validationFileEnumerateDelegate;
            this.validationController = validationController;
            this.validationResultsDataController = validationResultsDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.manualUrlsEnumerationController = manualUrlsEnumerationController;

            this.updatedDataController = updatedDataController;
            this.routingController = routingController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var validateProductsTask = statusController.Create(status, "Validate products");

            var counter = 0;

            var updated = updatedDataController.EnumerateIds();
            var updatedCount = updatedDataController.Count();

            foreach (var id in updated)
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
                    ++counter,
                    updatedCount,
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

                var fileValidationResults = new List<IFileValidation>(localFiles.Count);

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
                        fileValidationResults.Add(await validationController.ValidateAsync(
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
