using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;
using GOG.Models;
using Delegates.Format.Uri;
using Delegates.Data.Routes;
using Delegates.Activities;
using Delegates.Confirmations.Validation;
using Delegates.Itemizations.ProductTypes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Respond.Validate
{
    [RespondsToRequests(Method = "validate", Collection = "productfiles")]
    public class RespondToValidateProductFilesRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IGetValueDelegate<string,string> productFileDirectoryDelegate;
        private readonly IGetValueDelegate<string,string> getProductFileFilenameDelegate;
        private readonly IFormatDelegate<string, string> formatValidationFileDelegate;
        private readonly IConfirmExpectationAsyncDelegate<string, string> confirmFileValidationExpectationsAsyncDelegate;
        private readonly IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate;
        private readonly IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate;
        private readonly IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate;
        private readonly IGetDataAsyncDelegate<string, (long Id, string Source)> getRouteDataAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetProductFilesDirectoryDelegate),
            typeof(GetUriFilenameDelegate),
            typeof(FormatValidationFileDelegate),
            typeof(ConfirmFileValidationExpectationsAsyncDelegate),
            typeof(ItemizeAllUpdatedAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate),
            typeof(Itemize.ItemizeGameDetailsManualUrlsAsyncDelegate),
            typeof(GetRouteDataAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToValidateProductFilesRequestDelegate(
            IGetValueDelegate<string,string> productFileDirectoryDelegate,
            IGetValueDelegate<string,string> getProductFileFilenameDelegate,
            IFormatDelegate<string, string> formatValidationFileDelegate,
            IConfirmExpectationAsyncDelegate<string, string> confirmFileValidationExpectationsAsyncDelegate,
            IItemizeAllAsyncDelegate<long> itemizeAllUpdatedAsyncDelegate,
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate,
            IItemizeAsyncDelegate<GameDetails, string> itemizeGameDetailsManualUrlsAsyncDelegate,
            IGetDataAsyncDelegate<string, (long Id, string Source)> getRouteDataAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.productFileDirectoryDelegate = productFileDirectoryDelegate;
            this.getProductFileFilenameDelegate = getProductFileFilenameDelegate;
            this.formatValidationFileDelegate = formatValidationFileDelegate;
            this.confirmFileValidationExpectationsAsyncDelegate = confirmFileValidationExpectationsAsyncDelegate;
            this.getGameDetailsByIdAsyncDelegate = getGameDetailsByIdAsyncDelegate;
            this.itemizeGameDetailsManualUrlsAsyncDelegate = itemizeGameDetailsManualUrlsAsyncDelegate;

            this.itemizeAllUpdatedAsyncDelegate = itemizeAllUpdatedAsyncDelegate;
            this.getRouteDataAsyncDelegate = getRouteDataAsyncDelegate;

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

                setProgressDelegate.SetProgress();

                var localFiles = new List<string>();

                startDelegate.Start("Enumerate local product files");
                foreach (var manualUrl in
                    await itemizeGameDetailsManualUrlsAsyncDelegate.ItemizeAsync(gameDetails))
                {
                    var resolvedUri = await getRouteDataAsyncDelegate.GetDataAsync((id, manualUrl));

                    // use directory from source and file from resolved URI
                    var localFile = Path.Combine(
                        productFileDirectoryDelegate.GetValue(manualUrl),
                        getProductFileFilenameDelegate.GetValue(resolvedUri));

                    localFiles.Add(localFile);
                }

                completeDelegate.Complete();


                // check if current validation results allow us to skip validating current product
                // otherwise - validate all the files again

                // ...

                startDelegate.Start("Validate product files");

                foreach (var localFile in localFiles)
                {
                    setProgressDelegate.SetProgress();

                    var validationFile = formatValidationFileDelegate.Format(localFile);

                    try
                    {
                        if (!await confirmFileValidationExpectationsAsyncDelegate.ConfirmAsync(
                            localFile,
                            validationFile))
                            throw new InvalidDataException();
                    }
                    catch (Exception ex)
                    {
                        // await statusController.FailAsync(validateProductsStatus,
                        //     $"{localFile}: {ex.Message}");
                    }
                }

                completeDelegate.Complete();
            }

            completeDelegate.Complete();
        }
    }
}