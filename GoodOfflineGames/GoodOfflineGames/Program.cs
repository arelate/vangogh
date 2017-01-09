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
using Controllers.Eligibility;
using Controllers.Conversion;
using Controllers.Data;
using Controllers.SerializedStorage;
using Controllers.Indexing;
using Controllers.RecycleBin;
using Controllers.Routing;
using Controllers.TaskStatus;

using Interfaces.ProductTypes;
using Interfaces.TaskActivity;
using Interfaces.DataStoragePolicy;

using GOG.Models;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;
using GOG.Controllers.Enumeration;

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
using GOG.TaskActivities.Download.Validation;
using GOG.TaskActivities.Download.Processing;
using GOG.TaskActivities.Cleanup;

using GOG.TaskActivities.Validation;

using Models.Uris;
using Models.QueryParameters;

using Models.ProductRoutes;
using Models.ProductScreenshots;
using Models.ProductDownloads;

using Models.TaskStatus;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Foundation Controllers

            string recycleBinUri = "recycleBin";
            string productFilesDestination = "productFiles";

            var streamController = new StreamController();
            var fileController = new FileController();
            var directoryController = new DirectoryController();

            var recycleBinController = new RecycleBinController(
                recycleBinUri,
                fileController,
                directoryController);

            var storageController = new StorageController(
                streamController,
                fileController);
            var serializationController = new JSONStringController();

            var consoleController = new ConsoleController();

            //var taskReportingController = new TaskReportingController(
            //    consoleController);

            var bytesFormattingController = new BytesFormattingController();
            var secondsFormattingController = new SecondsFormattingController();

            var applicationTaskStatus = new TaskStatus() { Title = "Welcome to GoodOfflineGames" };

            var taskStatusViewController = new TaskStatusViewController(
                applicationTaskStatus,
                bytesFormattingController,
                secondsFormattingController,
                consoleController);

            var taskStatusController = new TaskStatusController(taskStatusViewController);

            var cookiesController = new CookiesController(
                storageController,
                serializationController);

            var uriController = new UriController();
            var networkController = new NetworkController(
                cookiesController,
                uriController);

            var downloadController = new DownloadController(
                networkController,
                streamController,
                fileController,
                taskStatusController);

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
                150, // don't throttle if less than N items
                2 * 60 * 1000);

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
            var productRoutesDestinationController = new ProductRoutesDestinationController();
            var scheduledScreenshotsUpdatesDestinationController = new ScheduledScreenshotsUpdatesDestinationController();
            var scheduledValidationsDestinationController = new ScheduledValidationsDestinationController();
            var lastKnownValidDestinationController = new LastKnownValidDestinationController();
            var scheduledCleanupDestinationController = new ScheduledCleanupDestinationController();

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

            var productRoutesDataController = new DataController<ProductRoutes>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                productRoutesDestinationController);

            var lastKnownValidDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                lastKnownValidDestinationController);

            var scheduledCleanupDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                scheduledCleanupDestinationController);

            #endregion

            #region Settings: Load, Validation

            var settingsController = new SettingsController(
                storageController,
                serializationController);

            var loadSettingsTask = taskStatusController.Create(applicationTaskStatus, "Load settings");
            var settings = settingsController.Load().Result;

            var validateSettingsTask = taskStatusController.Create(loadSettingsTask, "Validate settings");

            var validateDownloadSettingsTask = taskStatusController.Create(validateSettingsTask, "Validate download settings");
            var downloadPropertiesValidationController = new DownloadPropertiesValidationController(languageController);
            settings.Download = downloadPropertiesValidationController.ValidateProperties(settings.Download) as Models.Settings.DownloadProperties;
            taskStatusController.Complete(validateDownloadSettingsTask);

            taskStatusController.Complete(validateSettingsTask);
            taskStatusController.Complete(loadSettingsTask);

            // set user agent string used for network requests
            if (!string.IsNullOrEmpty(settings.Connection.UserAgent))
                networkController.UserAgent = settings.Connection.UserAgent;

            #endregion

            #region Task Activity Controllers

            #region Load

            var loadDataController = new LoadDataController(
                applicationTaskStatus,
                taskStatusController,
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
                productRoutesDataController,
                lastKnownValidDataController,
                scheduledCleanupDataController);

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
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #region Update.PageResults

            var productsPageResultsController = new ProductsPageResultController(
                ProductTypes.Product,
                requestPageController,
                serializationController,
                applicationTaskStatus,
                taskStatusController);

            var productsExtractionController = new ProductsExtractionController();

            var productsUpdateController = new PageResultUpdateController<
                ProductsPageResult,
                Product>(
                    ProductTypes.Product,
                    productsPageResultsController,
                    productsExtractionController,
                    requestPageController,
                    productsDataController,
                    applicationTaskStatus,
                    taskStatusController);

            var accountProductsPageResultsController = new AccountProductsPageResultController(
                ProductTypes.AccountProduct,
                requestPageController,
                serializationController,
                applicationTaskStatus,
                taskStatusController);

            var accountProductsExtractionController = new AccountProductsExtractionController();

            var accountProductsUpdateController = new PageResultUpdateController<
                AccountProductsPageResult,
                AccountProduct>(
                    ProductTypes.AccountProduct,
                    accountProductsPageResultsController,
                    accountProductsExtractionController,
                    requestPageController,
                    accountProductsDataController,
                    applicationTaskStatus,
                    taskStatusController);

            #endregion

            #region Update.NewUpdatedAccountProducts

            var newUpdatedAccountProductsController = new NewUpdatedAccountProductsController(
                updatedDataController,
                lastKnownValidDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #region Update.Wishlisted

            var wishlistedUpdateController = new WishlistedUpdateController(
                networkController,
                gogDataExtractionController,
                serializationController,
                wishlistedDataController,
                applicationTaskStatus,
                taskStatusController);

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
                applicationTaskStatus,
                taskStatusController);

            var apiProductUpdateController = new ApiProductUpdateController(
                apiProductsDataController,
                productsDataController,
                networkController,
                serializationController,
                productUpdateUriController,
                applicationTaskStatus,
                taskStatusController);

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
                applicationTaskStatus,
                taskStatusController);

            #endregion

            var screenshotUpdateController = new ScreenshotUpdateController(
                screenshotsDataController,
                scheduledScreenshotsUpdatesDataController,
                productsDataController,
                networkController,
                screenshotExtractionController,
                applicationTaskStatus,
                taskStatusController);

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
                applicationTaskStatus,
                taskStatusController);

            var routingController = new RoutingController(productRoutesDataController);

            var gameDetailsManualUrlsEnumerationController = new GameDetailsManualUrlEnumerationController(
                settings.Download.Languages,
                settings.Download.OperatingSystems,
                gameDetailsDataController);

            var gameDetailsDirectoryEnumerationController = new GameDetailsDirectoryEnumerationController(
                settings.Download.Languages,
                settings.Download.OperatingSystems,
                gameDetailsDataController,
                productFilesDestinationController);

            var gameDetailsFilesEnumerationController = new GameDetailsFileEnumerationController(
                settings.Download.Languages,
                settings.Download.OperatingSystems,
                gameDetailsDataController,
                routingController,
                productFilesDestinationController);

            var productFilesDownloadSourcesController = new ProductFilesDownloadSourcesController(
                updatedDataController,
                gameDetailsManualUrlsEnumerationController);

            var validationUriResolutionController = new ValidationUriResolutionController();

            var downloadEntryValidationEligibilityController = new DownloadEntryValidationEligibilityController();
            var updateRouteEligibilityController = new UpdateRouteEligibilityController();
            var removeEntryEligibilityController = new RemoveEntryEligibilityController();
            var fileValidationEligibilityController = new FileValidationEligibilityController();

            var validationDownloadSourcesController = new ValidationDownloadSourcesController(
                updatedDataController,
                productDownloadsDataController,
                routingController,
                validationUriResolutionController,
                downloadEntryValidationEligibilityController,
                fileValidationEligibilityController);

            // schedule download controllers

            var updateProductsImagesDownloadsController = new UpdateImagesDownloadsController(
                ProductDownloadTypes.Image,
                productsImagesDownloadSourcesController,
                imagesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var updateAccountProductsImagesDownloadsController = new UpdateImagesDownloadsController(
                ProductDownloadTypes.Image,
                accountProductsImagesDownloadSourcesController,
                imagesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var updateScreenshotsDownloadsController = new UpdateScreenshotsDownloadsController(
                ProductDownloadTypes.Screenshot,
                screenshotsDownloadSourcesController,
                screenshotsDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var updateProductFilesDownloadsController = new UpdateFilesDownloadsController(
                ProductDownloadTypes.ProductFile,
                productFilesDownloadSourcesController,
                productFilesDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            // downloads processing

            var imagesProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.Image,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                productFilesDestinationController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            var screenshotsProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.Screenshot,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                productFilesDestinationController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            var productFilesProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.ProductFile,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                productFilesDestinationController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            // validation controllers

            var updateValidationDownloadsController = new UpdateValidationDownloadsController(
                ProductDownloadTypes.Validation,
                validationDownloadSourcesController,
                validationDestinationController,
                productDownloadsDataController,
                accountProductsDataController,
                applicationTaskStatus,
                taskStatusController);

            var validationProcessScheduledDownloadsController = new ProcessScheduledDownloadsController(
                ProductDownloadTypes.Validation,
                updatedDataController,
                productDownloadsDataController,
                routingController,
                networkController,
                downloadController,
                productFilesDestinationController,
                updateRouteEligibilityController,
                removeEntryEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            var byteToStringConversionController = new BytesToStringConvertionController();

            var validationController = new ValidationController(
                validationDestinationController,
                fileController,
                streamController,
                byteToStringConversionController,
                taskStatusController);

            var processValidationController = new ProcessValidationController(
                productFilesDestinationController,
                validationController,
                productDownloadsDataController,
                updatedDataController,
                lastKnownValidDataController,
                scheduledCleanupDataController,
                routingController,
                downloadEntryValidationEligibilityController,
                applicationTaskStatus,
                taskStatusController);

            #region Cleanup

            var directoryCleanupController = new DirectoryCleanupController(
                gameDetailsDataController,
                gameDetailsDirectoryEnumerationController,
                productFilesDestinationController,
                directoryController,
                recycleBinController,
                applicationTaskStatus,
                taskStatusController);

            var filesCleanupController = new FilesCleanupController(
                scheduledCleanupDataController,
                accountProductsDataController,
                gameDetailsFilesEnumerationController,
                gameDetailsDirectoryEnumerationController,
                directoryController,
                fileValidationEligibilityController,
                validationDestinationController,
                recycleBinController,
                applicationTaskStatus,
                taskStatusController);

            #endregion

            #endregion

            #region TACs Execution

            #region Initialization Task Activities (always performed)

            var taskActivityControllers = new List<ITaskActivityController>
            {
                // load initial data
                loadDataController,
                // authorize
                authorizationController
            };

            #endregion

            #region Data Updates Task Activities

            // data updates
            if (settings.Update.Products)
                taskActivityControllers.Add(productsUpdateController);
            if (settings.Update.AccountProducts)
                taskActivityControllers.Add(accountProductsUpdateController);
            if (settings.Update.NewUpdatedAccountProducts)
                taskActivityControllers.Add(newUpdatedAccountProductsController);
            if (settings.Update.Wishlist)
                taskActivityControllers.Add(wishlistedUpdateController);

            // product/account product dependent data updates
            if (settings.Update.GameProductData)
                taskActivityControllers.Add(gameProductDataUpdateController);
            if (settings.Update.ApiProducts)
                taskActivityControllers.Add(apiProductUpdateController);
            if (settings.Update.GameDetails)
                taskActivityControllers.Add(gameDetailsUpdateController);
            if (settings.Update.Screenshots)
                taskActivityControllers.Add(screenshotUpdateController);

            #endregion

            #region Download Task Activities

            // schedule downloads
            if (settings.Download.ProductsImages)
                taskActivityControllers.Add(updateProductsImagesDownloadsController);
            if (settings.Download.AccountProductsImages)
                taskActivityControllers.Add(updateAccountProductsImagesDownloadsController);
            if (settings.Download.Screenshots)
                taskActivityControllers.Add(updateScreenshotsDownloadsController);
            if (settings.Download.ProductsFiles)
                taskActivityControllers.Add(updateProductFilesDownloadsController);

            //actually download images, screenshots, product files, extras
            if (settings.Download.ProductsImages ||
                settings.Download.AccountProductsImages)
                taskActivityControllers.Add(imagesProcessScheduledDownloadsController);
            if (settings.Download.Screenshots)
                taskActivityControllers.Add(screenshotsProcessScheduledDownloadsController);
            if (settings.Download.ProductsFiles)
                taskActivityControllers.Add(productFilesProcessScheduledDownloadsController);

            #endregion

            #region Validation Task Activities

            if (settings.Validation.Updated)
            {
                // schedule validation downloads
                taskActivityControllers.Add(updateValidationDownloadsController);
                // actually download validation
                taskActivityControllers.Add(validationProcessScheduledDownloadsController);
                // process validation
                taskActivityControllers.Add(processValidationController);
            }

            #endregion

            #region Cleanup Task Activities

            // cleanup directories
            if (settings.Cleanup.Directories)
                taskActivityControllers.Add(directoryCleanupController);
            // cleanup files
            if (settings.Cleanup.Files)
                taskActivityControllers.Add(filesCleanupController);

            #endregion



            foreach (var taskActivityController in taskActivityControllers)
            {
                try
                {
                    taskActivityController.ProcessTaskAsync().Wait();
                    consoleController.WriteLine(string.Empty);
                }
                catch (AggregateException ex)
                {
                    List<string> errorMessages = new List<string>();

                    foreach (var innerException in ex.InnerExceptions)
                        errorMessages.Add(innerException.Message);

                    taskStatusController.Fail(applicationTaskStatus, string.Join(", ", errorMessages));
                    break;
                }
            }

            consoleController.WriteLine("Press ENTER to continue...");
            consoleController.ReadLine();

            #endregion
        }
    }
}
