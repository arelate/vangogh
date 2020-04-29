using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Data;
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
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate;
        private readonly IGetDataAsyncDelegate<ValidationResults, long> getValidationResultsByIdAsyncDelegate;
        private readonly IUpdateAsyncDelegate<ValidationResults> updateValidationResultsAsyncDelegate;
        private readonly ICommitAsyncDelegate commitValidationResultsAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate;
        private readonly IRoutingController routingController;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "Delegates.GetDirectory.ProductTypes.GetProductFilesDirectoryDelegate,Delegates",
            "Delegates.GetFilename.GetUriFilenameDelegate,Delegates",
            "Delegates.Format.Uri.FormatValidationFileDelegate,Delegates",
            "Controllers.Validation.FileValidationController,Controllers",
            "Delegates.Itemize.ProductTypes.ItemizeAllUpdatedAsyncDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate,GOG.Delegates",
            "Delegates.Data.Models.ProductTypes.GetValidationResultsByIdAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateValidationResultsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.CommitValidationResultsAsyncDelegate,Delegates",
            "GOG.Delegates.Itemize.ItemizeGameDetailsManualUrlsAsyncDelegate,GOG.Delegates",
            "Controllers.Routing.RoutingController,Controllers",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToValidateProductFilesRequestDelegate(
            IGetDirectoryDelegate productFileDirectoryDelegate,
            IGetFilenameDelegate productFileFilenameDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IFileValidationController fileValidationController,
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate,
            IGetDataAsyncDelegate<ValidationResults, long> getValidationResultsByIdAsyncDelegate,
            IUpdateAsyncDelegate<ValidationResults> updateValidationResultsAsyncDelegate,
            ICommitAsyncDelegate commitValidationResultsAsyncDelegate,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IRoutingController routingController,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.productFileDirectoryDelegate = productFileDirectoryDelegate;
            this.productFileFilenameDelegate = productFileFilenameDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.fileValidationController = fileValidationController;
            this.getGameDetailsByIdAsyncDelegate = getGameDetailsByIdAsyncDelegate;
            this.getValidationResultsByIdAsyncDelegate = getValidationResultsByIdAsyncDelegate;
            this.updateValidationResultsAsyncDelegate = updateValidationResultsAsyncDelegate;
            this.commitValidationResultsAsyncDelegate = commitValidationResultsAsyncDelegate;
            this.itemizeGameDetailsManualUrlsAsyncDelegate = itemizeGameDetailsManualUrlsAsyncDelegate;

            this.itemizeAllUpdatedAsyncDelegate = itemizeAllUpdatedAsyncDelegate;
            this.routingController = routingController;

            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Validate products");

            // TODO: Should this be itemizeAllUpdatedGameDetails instead?
            await foreach (var id in itemizeAllUpdatedAsyncDelegate.ItemizeAllAsync())
            {
                var gameDetails = await getGameDetailsByIdAsyncDelegate.GetDataAsync(id);
                var validationResults = await getValidationResultsByIdAsyncDelegate.GetDataAsync(id);

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
                await updateValidationResultsAsyncDelegate.UpdateAsync(validationResults);
                completeDelegate.Complete();
            }

            await commitValidationResultsAsyncDelegate.CommitAsync();
            completeDelegate.Complete();
        }
    }
}