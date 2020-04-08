using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Respond;
using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using Interfaces.Validation;
using Interfaces.Routing;
using Interfaces.ValidationResults;
using Attributes;
using Models.ProductTypes;
using GOG.Models;

namespace GOG.Delegates.Respond.Validate
{
    [RespondsToRequests(Method = "validate", Collection = "productfiles")]
    public class RespondToValidateProductFilesRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IGetDirectoryDelegate productFileDirectoryDelegate;
        private readonly IGetFilenameDelegate productFileFilenameDelegate;
        private readonly IFormatDelegate<string, string> formatValidationFileDelegate;
        private readonly IFileValidationController fileValidationController;
        private readonly IDataController<ValidationResults> validationResultsDataController;
        private readonly IDataController<GameDetails> gameDetailsDataController;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate;
        private readonly IDataController<long> updatedDataController;
        private readonly IRoutingController routingController;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

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
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToValidateProductFilesRequestDelegate(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IFileValidationController fileValidationController,
            IDataController<ValidationResults> validationResultsDataController,
            IDataController<GameDetails> gameDetailsDataController,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IDataController<long> updatedDataController,
            IRoutingController routingController,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
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

            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Validate products");

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

                setProgressDelegate.SetProgress();

                var localFiles = new List<string>();

                startDelegate.Start("Enumerate local product files");
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

                completeDelegate.Complete();


                // check if current validation results allow us to skip validating current product
                // otherwise - validate all the files again

                // ...

                var fileValidationResults = new List<IFileValidationResults>(localFiles.Count);

                startDelegate.Start("Validate product files");

                foreach (var localFile in localFiles)
                {
                    setProgressDelegate.SetProgress();

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

                completeDelegate.Complete();

                validationResults.Files = fileValidationResults.ToArray();

                startDelegate.Start("Update validation results");
                await validationResultsDataController.UpdateAsync(validationResults);
                completeDelegate.Complete();
            }

            completeDelegate.Complete();
        }
    }
}