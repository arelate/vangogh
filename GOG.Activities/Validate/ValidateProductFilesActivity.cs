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
        private IEnumerateAsyncDelegate<GameDetails> manualUrlsEnumerationController;
        private IEnumerateIdsAsyncDelegate productEnumerateDelegate;
        private IRoutingController routingController;

        public ValidateProductFilesActivity(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IEnumerateDelegate<string> validationFileEnumerateDelegate,
            IFileValidationController fileValidationController,
            IDataController<ValidationResult> validationResultsDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IEnumerateAsyncDelegate<GameDetails> manualUrlsEnumerationController,
            IEnumerateIdsAsyncDelegate productEnumerateDelegate,
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
            var validateProductsStatus = await statusController.CreateAsync(status, "Validate products");

            var current = 0;

            var validateProductsList = await productEnumerateDelegate.EnumerateIdsAsync(validateProductsStatus);
            var validateProductsCount = validateProductsList.Count();

            foreach (var id in validateProductsList)
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id, validateProductsStatus);
                var validationResults = await validationResultsDataController.GetByIdAsync(id, validateProductsStatus);

                if (validationResults == null)
                    validationResults = new ValidationResult()
                    {
                        Id = id,
                        Title = gameDetails.Title
                    };

                await statusController.UpdateProgressAsync(validateProductsStatus,
                    ++current,
                    validateProductsCount,
                    gameDetails.Title);

                var localFiles = new List<string>();

                var getLocalFilesTask = await statusController.CreateAsync(validateProductsStatus, "Enumerate local product files");
                foreach (var manualUrl in await manualUrlsEnumerationController.EnumerateAsync(gameDetails, getLocalFilesTask))
                {
                    var resolvedUri = await routingController.TraceRouteAsync(id, manualUrl, getLocalFilesTask);

                    // use directory from source and file from resolved URI
                    var localFile = Path.Combine(
                        productFileDirectoryDelegate.GetDirectory(manualUrl),
                        productFileFilenameDelegate.GetFilename(resolvedUri));

                    localFiles.Add(localFile);
                }
                await statusController.CompleteAsync(getLocalFilesTask);


                // check if current validation results allow us to skip validating current product
                // otherwise - validate all the files again

                // ...

                var fileValidationResults = new List<IFileValidationResult>(localFiles.Count);

                var validateFilesTask = await statusController.CreateAsync(
                    validateProductsStatus,
                    "Validate product files");

                var currentFile = 0;

                foreach (var localFile in localFiles)
                {
                    await statusController.UpdateProgressAsync(validateFilesTask,
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
                        await statusController.FailAsync(validateProductsStatus,
                            $"{localFile}: {ex.Message}");
                    }
                }

                await statusController.CompleteAsync(validateFilesTask);

                validationResults.Files = fileValidationResults.ToArray();

                var updateValidationResultsTask = await statusController.CreateAsync(validateProductsStatus, "Update validation results");
                await validationResultsDataController.UpdateAsync(validateProductsStatus, validationResults);
                await statusController.CompleteAsync(updateValidationResultsTask);
            }

            await statusController.CompleteAsync(validateProductsStatus);
        }
    }
}
