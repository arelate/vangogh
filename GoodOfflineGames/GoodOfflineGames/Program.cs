using System;
using System.Collections.Generic;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Network;
using Controllers.Download;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.Extraction;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Reporting;
using Controllers.Settings;
using Controllers.RequestPage;
using Controllers.Throttle;
using Controllers.ImageUri;
using Controllers.Formatting;
using Controllers.UriResolution;
using Controllers.Destination;
using Controllers.Destination.Data;
using Controllers.Cookies;
using Controllers.PropertiesValidation;
using Controllers.Validation;
using Controllers.Conversion;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Indexing;
using Controllers.RecycleBin;

using Interfaces.ProductTypes;
using Interfaces.TaskActivity;
using Interfaces.DataStoragePolicy;

using GOG.Models;
using GOG.Models.Custom;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;

using GOG.TaskActivities.Authorization;
using GOG.TaskActivities.Load;
using GOG.TaskActivities.Update.PageResult;
using GOG.TaskActivities.Update.NewUpdatedAccountProducts;
using GOG.TaskActivities.Update.Wishlisted;
using GOG.TaskActivities.Update.Dependencies.Product;
using GOG.TaskActivities.Update.Dependencies.GameDetails;
using GOG.TaskActivities.Update.Dependencies.GameProductData;
using GOG.TaskActivities.Update.Products;
using GOG.TaskActivities.Update.Screenshots;
using GOG.TaskActivities.Download.Dependencies.ProductImages;
using GOG.TaskActivities.Download.Dependencies.Screenshots;
using GOG.TaskActivities.Download.Dependencies.ProductFiles;
using GOG.TaskActivities.Download.Dependencies.Validation;
using GOG.TaskActivities.Download.ProductImages;
using GOG.TaskActivities.Download.Screenshots;
using GOG.TaskActivities.Download.ProductFiles;
using GOG.TaskActivities.Download.Processing;

using GOG.TaskActivities.Validation;

using Models.Uris;
using Models.QueryParameters;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Foundation Controllers

            string recycleBinUri = "_recycleBin";
            string productFilesDestination = "productFiles";

            var streamController = new StreamController();
            var fileController = new FileController();

            var recycleBinController = new RecycleBinController(
                recycleBinUri,
                fileController);

            var directoryController = new DirectoryController();
            var storageController = new StorageController(
                streamController,
                fileController);
            var serializationController = new JSONStringController();

            var consoleController = new ConsoleController();

            var taskReportingController = new TaskReportingController(
                consoleController);

            var cookiesController = new CookiesController(
                storageController,
                serializationController);

            var uriController = new UriController();
            var networkController = new NetworkController(
                cookiesController,
                uriController);

            var bytesFormattingController = new BytesFormattingController();
            var secondsFormattingController = new SecondsFormattingController();
            var downloadReportingController = new DownloadReportingController(
                bytesFormattingController,
                secondsFormattingController,
                consoleController);
            var downloadController = new DownloadController(
                networkController,
                streamController,
                fileController,
                downloadReportingController);

            var requestPageController = new RequestPageController(
                networkController);

            var languageController = new LanguageController();

            var loginTokenExtractionController = new LoginTokenExtractionController();
            var loginIdExtractionController = new LoginIdExtractionController();
            var loginUsernameExtractionController = new LoginUsernameExtractionController();

            var gogDataExtractionController = new GOGDataExtractionController();
            var screenshotExtractionController = new ScreenshotExtractionController();

            var collectionController = new CollectionController();

            var throttleController = new ThrottleController(
                taskReportingController,
                secondsFormattingController);

            var imageUriController = new ImageUriController();
            var screenshotUriController = new ScreenshotUriController();

            var gogUriDestinationController = new GOGUriDestinationController();
            var productFilesDestinationController = new ProductFilesDestinationController(
                productFilesDestination,
                gogUriDestinationController);
            var imagesDestinationController = new ImagesDestinationController();
            var screenshotsFilesDestinationController = new ScreenshotsFilesDestinationController();
            var validationDestinationController = new ValidationDestinationController();

            #endregion

            #region Data Controllers

            // Data controllers for products, game details, game product data, etc.

            var javaScriptPrefix = "var data=";
            var jsonToJavaScriptConversionController = new JSONToJavaScriptConvetsionController(javaScriptPrefix);

            var serializedStorageController = new SerializedStorageController(
                storageController,
                serializationController,
                jsonToJavaScriptConversionController);

            var productCoreIndexingController = new ProductCoreIndexingController();
            var passthroughIndexingController = new PassthroughIndexingController();

            var productsDestinationController = new ProductsDestinationController();
            var accountProductsDestinationController = new AccountProductDestinationController();
            var gameDetailsDestinationController = new GameDetailsDestinationController();
            var screenshotsDestinationController = new ScreenshotsDestinationController();
            var apiProductsDestinationController = new ApiProductsDestinationController();
            var gameProductDataDestinationController = new GameProductDataDestinationController();
            var wishlistedDestinationController = new WishlistedDestinationController();
            var updatedDestinationController = new UpdatedDestinationController();
            var productDownloadsDestinationController = new ProductDownloadsDestinationController();
            var scheduledScreenshotsUpdatesDestinationController = new ScheduledScreenshotsUpdatesDestinationController();
            var scheduledValidationsDestinationController = new ScheduledValidationsDestinationController();
            var lastKnownValidDestinationController = new LastKnownValidDestinationController();

            var productsDataController = new DataController<Product>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                productCoreIndexingController,
                collectionController,
                productsDestinationController);

            var accountProductsDataController = new DataController<AccountProduct>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                productCoreIndexingController,
                collectionController,
                accountProductsDestinationController);

            var gameDetailsDataController = new DataController<GameDetails>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                gameDetailsDestinationController,
                recycleBinController);

            var gameProductDataController = new DataController<GameProductData>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                gameProductDataDestinationController,
                recycleBinController);

            var wishlistedDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                wishlistedDestinationController);

            var updatedDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                updatedDestinationController);

            var apiProductsDataController = new DataController<ApiProduct>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                apiProductsDestinationController,
                recycleBinController);

            var scheduledScreenshotsUpdatesDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                scheduledScreenshotsUpdatesDestinationController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                screenshotsDestinationController,
                recycleBinController);

            var productDownloadsDataController = new DataController<ProductDownloads>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                productDownloadsDestinationController);

            var scheduledValidationsDataController = new DataController<ScheduledValidation>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                productCoreIndexingController,
                collectionController,
                scheduledValidationsDestinationController);

            var lastKnownValidDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                lastKnownValidDestinationController);

            #endregion

            #region Settings: Load, Validation

            var settingsController = new SettingsController(
                storageController,
                serializationController);

            taskReportingController.StartTask("Load settings");
            var settings = settingsController.Load().Result;
            taskReportingController.CompleteTask();
            consoleController.WriteLine(string.Empty);

            taskReportingController.StartTask("Validate settings");

            taskReportingController.StartTask("Validate download settings");
            var downloadPropertiesValidationController = new DownloadPropertiesValidationController(languageController);
            settings.Download = downloadPropertiesValidationController.ValidateProperties(settings.Download) as Models.Settings.DownloadProperties;
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();
            consoleController.WriteLine(string.Empty);

            // set user agent string used for network requests
            networkController.UserAgent = settings.Connection.UserAgent;

            #endregion

            #region Task Activity Controllers

            // Create and add all task activity controllers
            // Task activities are encapsulated set of activity - so no data can be passed around!

            #region Load

            var loadDataController = new LoadDataController(
                taskReportingController,
                productsDataController,
                accountProductsDataController,
                gameDetailsDataController,
                gameProductDataController,
                screenshotsDataController,
                apiProductsDataController,
                wishlistedDataController,
                updatedDataController,
                scheduledScreenshotsUpdatesDataController,
                productDownloadsDataController,
                scheduledValidationsDataController,
                lastKnownValidDataController);

            #endregion

            #region Authorization

            var authenticationPropertiesValidationController = new AuthenticationPropertiesValidationController(consoleController);

            var authorizationController = new AuthorizationController(
                uriController,
                networkController,
                serializationController,
                loginTokenExtractionController,
                loginIdExtractionController,
                loginUsernameExtractionController,
                consoleController,
                settings.Authentication,
                authenticationPropertiesValidationController,
                taskReportingController);

            #endregion

            #region Update.PageResults

            var productsPageResultsController = new ProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.GetUpdateUri(ProductTypes.Product),
                QueryParameters.GetQueryParameters(ProductTypes.Product),
                taskReportingController);

            var productsExtractionController = new ProductsExtractionController();

            var productsUpdateController = new PageResultUpdateController<
                ProductsPageResult,
                Product>(
                    ProductTypes.Product,
                    productsPageResultsController,
                    productsExtractionController,
                    requestPageController,
                    productsDataController,
                    taskReportingController);

            var accountProductsPageResultsController = new AccountProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.GetUpdateUri(ProductTypes.AccountProduct),
                QueryParameters.GetQueryParameters(ProductTypes.AccountProduct),
                taskReportingController);

            var accountProductsExtractionController = new AccountProductsExtractionController();

            var accountProductsUpdateController = new PageResultUpdateController<
                AccountProductsPageResult,
                AccountProduct>(
                    ProductTypes.AccountProduct,
                    accountProductsPageResultsController,
                    accountProductsExtractionController,
                    requestPageController,
                    accountProductsDataController,
                    taskReportingController);

            #endregion

            #region Update.NewUpdatedAccountProducts

            var newUpdatedAccountProductsController = new NewUpdatedAccountProductsController(
                updatedDataController,
                accountProductsDataController,
                taskReportingController);

            #endregion

            #region Update.Wishlisted

            var wishlistedUpdateController = new WishlistedUpdateController(
                networkController,
                gogDataExtractionController,
                serializationController,
                wishlistedDataController,
                taskReportingController);

            #endregion

            #region Update.Products

            // dependencies for update controllers

            var productUpdateUriController = new ProductUpdateUriController();

            var gameProductDataUpdateUriController = new GameProductDataUpdateUriController();
            var gameProductDataSkipUpdateController = new GameProductDataSkipUpdateController(
                productsDataController);
            var gameProductDataDecodingController = new GameProductDataDecodingController(
                gogDataExtractionController,
                serializationController);

            var gameDetailsRequiredUpdatesController = new GameDetailsRequiredUpdatesController(
                updatedDataController);
            var gameDetailsConnectionController = new GameDetailsConnectionController();
            var gameDetailsDownloadDetailsController = new GameDetailsDownloadDetailsController(
                serializationController,
                languageController);

            // product update controllers

            var gameProductDataUpdateController = new GameProductDataUpdateController(
                gameProductDataController,
                productsDataController,
                networkController,
                serializationController,
                gameProductDataUpdateUriController,
                gameProductDataSkipUpdateController,
                gameProductDataDecodingController,
                taskReportingController);

            var apiProductUpdateController = new ApiProductUpdateController(
                apiProductsDataController,
                productsDataController,
                networkController,
                serializationController,
                productUpdateUriController,
                taskReportingController);

            var gameDetailsUpdateController = new GameDetailsUpdateController(
                gameDetailsDataController,
                accountProductsDataController,
                networkController,
                serializationController,
                throttleController,
                productUpdateUriController,
                gameDetailsRequiredUpdatesController,
                gameDetailsConnectionController,
                gameDetailsDownloadDetailsController,
                taskReportingController);

            #endregion

            var screenshotUpdateController = new ScreenshotUpdateController(
                screenshotsDataController,
                scheduledScreenshotsUpdatesDataController,
                productsDataController,
                networkController,
                screenshotExtractionController,
                taskReportingController);

            // dependencies for download controllers

            var productsImagesDownloadSourcesController = new ProductsImagesDownloadSourcesController(
                updatedDataController,
                productsDataController,
                imageUriController);

            var accountProductsImagesDownloadSourcesController = new AccountProductsImagesDownloadSourcesController(
                updatedDataController,
                accountProductsDataController,
                imageUriController);

            var screenshotsDownloadSourcesController = new ScreenshotsDownloadSourcesController(
                scheduledScreenshotsUpdatesDataController,
                screenshotsDataController,
                screenshotUriController,
                taskReportingController);

            var productFilesDownloadSourcesController = new ProductFilesDownloadSourcesController(
                updatedDataController,
                gameDetailsDataController,
                settings.Download.Languages,
                settings.Download.OperatingSystems);

            var productExtrasDownloadSourcesController = new ProductExtrasDownloadSourcesController(
                updatedDataController,
                gameDetailsDataController);

            var validationUriResolutionController = new ValidationUriResolutionController();

            var validationDownloadSourcesController = new ValidationDownloadSourcesController(
                updatedDataController,
                productDownloadsDataController,
                validationUriResolutionController);

            // schedule download controllers

            var updateProductsImagesDownloadsController = new UpdateImagesDownloadsController(
                productsImagesDownloadSourcesController,
                imagesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController);

            var updateAccountProductsImagesDownloadsController = new UpdateImagesDownloadsController(
                accountProductsImagesDownloadSourcesController,
                imagesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController);

            var updateScreenshotsDownloadsController = new UpdateScreenshotsDownloadsController(
                screenshotsDownloadSourcesController,
                screenshotsDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController);

            var updateProductFilesDownloadsController = new UpdateFilesDownloadsController(
                ProductDownloadTypes.ProductFile,
                productFilesDownloadSourcesController,
                productFilesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController);

            var updateProductExtrasDownloadsController = new UpdateFilesDownloadsController(
                ProductDownloadTypes.Extra,
                productExtrasDownloadSourcesController,
                productFilesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController);

            // downloads processing

            var processScheduledDownloadsController = new ProcessScheduledDownloadsController(
                updatedDataController,
                productDownloadsDataController,
                scheduledValidationsDataController,
                downloadController,
                productFilesDestinationController,
                taskReportingController);

            // validation controllers

            var validationDataDownloadController = new ValidationDataDownloadController(
                validationUriResolutionController,
                validationDestinationController,
                downloadController);

            var byteToStringConversionController = new BytesToStringConvertionController();

            var validationController = new ValidationController(
                validationDestinationController,
                fileController,
                streamController,
                byteToStringConversionController);

            var validationFilesDownloadController = new ValidationFilesDownloadController(
                updatedDataController,
                productDownloadsDataController,
                validationDataDownloadController,
                taskReportingController);

            var processValidationController = new ProcessValidationController(
                gogUriDestinationController,
                validationController,
                updatedDataController,
                lastKnownValidDataController,
                scheduledValidationsDataController,
                taskReportingController);

            #endregion

            #region TACs Execution

            var taskActivityControllers = new List<ITaskActivityController>()
            {
                loadDataController,
                authorizationController,
                //productsUpdateController,
                //accountProductsUpdateController,
                //newUpdatedAccountProductsController,
                //wishlistedUpdateController,
                //gameProductDataUpdateController,
                //apiProductUpdateController,
                //gameDetailsUpdateController,
                //screenshotUpdateController,
                //updateProductsImagesDownloadsController,
                //updateAccountProductsImagesDownloadsController,
                //updateScreenshotsDownloadsController,
                //updateProductFilesDownloadsController,
                //updateProductExtrasDownloadsController,
                processScheduledDownloadsController,
                validationFilesDownloadController,
                processValidationController
            };

            foreach (var taskActivityController in taskActivityControllers)
                try
                {
                    taskActivityController.ProcessTask().Wait();
                    consoleController.WriteLine(string.Empty);
                }
                catch (AggregateException ex)
                {
                    List<string> errorMessages = new List<string>();

                    foreach (var innerException in ex.InnerExceptions)
                        errorMessages.Add(innerException.Message);

                    taskReportingController.ReportFailure(string.Join(", ", errorMessages));
                    break;
                }

            consoleController.WriteLine("Press ENTER to continue...");
            consoleController.ReadLine();

            #endregion
        }
    }
}
