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
using Interfaces.Controllers.Logs;

using Interfaces.Activity;
using Interfaces.Validation;
using Interfaces.Routing;

using Interfaces.ValidationResults;

using Attributes;

using Models.ProductTypes;

using GOG.Models;

namespace GOG.Activities.Validate
{
    public class ValidateProductFilesActivity : IActivity
    {
        readonly IGetDirectoryDelegate productFileDirectoryDelegate;
        readonly IGetFilenameDelegate productFileFilenameDelegate;
        readonly IFormatDelegate<string, string> formatValidationFileDelegate;
        readonly IFileValidationController fileValidationController;
        readonly IDataController<ValidationResults> validationResultsDataController;
        readonly IDataController<GameDetails> gameDetailsDataController;
        readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate;
        readonly IDataController<long> updatedDataController;
        readonly IRoutingController routingController;
        readonly IResponseLogController responseLogController;

		[Dependencies(
			"Delegates.GetDirectory.ProductTypes.GetProductFilesDirectoryDelegate,Delegates",
			"Delegates.GetFilename.GetUriFilenameDelegate,Delegates",
			"Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
			"Controllers.Validation.FileValidationController,Controllers",
			"Controllers.Data.ProductTypes.ValidationResultsDataController,Controllers",
			"GOG.Controllers.Data.ProductTypes.GameDetailsDataController,GOG.Controllers",
			"GOG.Delegates.Itemize.ItemizeGameDetailsManualUrlsAsyncDelegate,GOG.Delegates",
			"Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
			"Controllers.Routing.RoutingController,Controllers",
			"Controllers.Logs.ResponseLogController,Controllers")]
        public ValidateProductFilesActivity(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IFileValidationController fileValidationController,
            IDataController<ValidationResults> validationResultsDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IDataController<long> updatedDataController,
            IRoutingController routingController,
            IResponseLogController responseLogController)
        {
            this.productFileDirectoryDelegate = productFileDirectoryDelegate;
            this.productFileFilenameDelegate = productFileFilenameDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.fileValidationController = fileValidationController;
            this.validationResultsDataController = validationResultsDataController;
            this.gameDetailsDataController = gameDetailsDataController;
            this.itemizeGameDetailsManualUrlsAsyncDelegate = itemizeGameDetailsManualUrlsAsyncDelegate;

            this.updatedDataController = updatedDataController;
            this.routingController = routingController;
            this.responseLogController = responseLogController;
        }

        public async Task ProcessActivityAsync()
        {
            responseLogController.OpenResponseLog("Validate products");

            await foreach (var id in updatedDataController.ItemizeAllAsync())
            {
                var gameDetails = await gameDetailsDataController.GetByIdAsync(id);
                var validationResults = await validationResultsDataController.GetByIdAsync(id);

                if (validationResults == null)
                    validationResults = new ValidationResults
                    {
                        Id = id,
                        Title = gameDetails.Title
                    };

                responseLogController.IncrementActionProgress();

                var localFiles = new List<string>();

                responseLogController.StartAction("Enumerate local product files");
                foreach (var manualUrl in 
                    await itemizeGameDetailsManualUrlsAsyncDelegate.ItemizeAsync(gameDetails))
                {
                    var resolvedUri = await routingController.TraceRouteAsync(id, manualUrl);

                    // use directory from source and file from resolved URI
                    var localFile = Path.Combine(
                        productFileDirectoryDelegate.GetDirectory(manualUrl),
                        productFileFilenameDelegate.GetFilename(resolvedUri));

                    localFiles.Add(localFile);
                }
                responseLogController.CompleteAction();


                // check if current validation results allow us to skip validating current product
                // otherwise - validate all the files again

                // ...

                var fileValidationResults = new List<IFileValidationResults>(localFiles.Count);

                responseLogController.StartAction("Validate product files");

                foreach (var localFile in localFiles)
                {
                    responseLogController.IncrementActionProgress();

                    var validationFile = formatValidationFileDelegate.Format(localFile);

                    try
                    {
                        fileValidationResults.Add(await fileValidationController.ValidateFileAsync(
                            localFile,
                            validationFile));
                    }
                    catch (Exception ex)
                    {
                        // await statusController.FailAsync(validateProductsStatus,
                        //     $"{localFile}: {ex.Message}");
                    }
                }

                responseLogController.CompleteAction();

                validationResults.Files = fileValidationResults.ToArray();

                responseLogController.StartAction("Update validation results");
                await validationResultsDataController.UpdateAsync(validationResults);
                responseLogController.CompleteAction();
            }

            responseLogController.CloseResponseLog();
        }
    }
}
