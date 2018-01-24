using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;

using Interfaces.Controllers.Data;

using Interfaces.Validation;
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
        private IFormatDelegate<string, string> formatValidationFileDelegate;
        private IFileValidationController fileValidationController;
        private IDataController<long, ValidationResult> validationResultsDataController;
        private IDataController<long, GameDetails> gameDetailsDataController;
        private IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate;
        private IItemizeAllAsyncDelegate<long> itemizeAllProductsAsyncDelegate;
        private IRoutingController routingController;

        public ValidateProductFilesActivity(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IFileValidationController fileValidationController,
            IDataController<long, ValidationResult> validationResultsDataController,
            IDataController<long, GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IItemizeAllAsyncDelegate<long> itemizeAllProductsAsyncDelegate,
            IRoutingController routingController,
            IStatusController statusController) :
            base(statusController)
        {
            this.productFileDirectoryDelegate = productFileDirectoryDelegate;
            this.productFileFilenameDelegate = productFileFilenameDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.fileValidationController = fileValidationController;
            this.validationResultsDataController = validationResultsDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsManualUrlsAsyncDelegate = itemizeGameDetailsManualUrlsAsyncDelegate;

            this.itemizeAllProductsAsyncDelegate = itemizeAllProductsAsyncDelegate;
            this.routingController = routingController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var validateProductsStatus = await statusController.CreateAsync(status, "Validate products");

            var current = 0;

            var validateProductsList = await itemizeAllProductsAsyncDelegate.ItemizeAllAsync(validateProductsStatus);
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
                foreach (var manualUrl in 
                    await itemizeGameDetailsManualUrlsAsyncDelegate.ItemizeAsync(gameDetails, getLocalFilesTask))
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

                    var validationFile = formatValidationFileDelegate.Format(localFile);

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

                var updateValidationResultsTask = await statusController.CreateAsync(
                    validateProductsStatus, 
                    "Update validation results");

                await validationResultsDataController.UpdateAsync(validationResults, validateProductsStatus);
                await statusController.CompleteAsync(updateValidationResultsTask);
            }

            await statusController.CompleteAsync(validateProductsStatus);
        }
    }
}
