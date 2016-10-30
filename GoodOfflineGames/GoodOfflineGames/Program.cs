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
using Controllers.UriRedirection;
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
using GOG.TaskActivities.Download.Dependencies.ProductExtras;
using GOG.TaskActivities.Download.Dependencies.Validation;
using GOG.TaskActivities.Download.ProductImages;
using GOG.TaskActivities.Download.Screenshots;
using GOG.TaskActivities.Download.ProductFiles;
using GOG.TaskActivities.Download.ProductExtras;
using GOG.TaskActivities.Download.Validation;
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

            var throttleController = new ThrottleController();

            var imageUriController = new ImageUriController();
            var screenshotUriController = new ScreenshotUriController();
            var uriRedirectController = new UriRedirectController(
                networkController);

            var gogUriDestinationController = new GOGUriDestinationController();
            var filesExtrasDestinationController = new FilesExtrasDestinationController(
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
            var scheduledDownloadsDestinationController = new ScheduledDownloadsDestinationController();

            var productsDataController = new DataController<Product>(
                serializedStorageController, 
                DataStoragePolicy.ItemsList,
                productCoreIndexingController, 
                collectionController,
                productsDestinationController,
                recycleBinController);

            var accountProductsDataController = new DataController<AccountProduct>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                productCoreIndexingController,
                collectionController,
                accountProductsDestinationController,
                recycleBinController);

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
                wishlistedDestinationController,
                recycleBinController);

            var updatedDataController = new DataController<long>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                passthroughIndexingController,
                collectionController,
                updatedDestinationController,
                recycleBinController);

            var apiProductsDataController = new DataController<ApiProduct>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                apiProductsDestinationController,
                recycleBinController);

            var screenshotsDataController = new DataController<ProductScreenshots>(
                serializedStorageController,
                DataStoragePolicy.IndexAndItems,
                productCoreIndexingController,
                collectionController,
                screenshotsDestinationController,
                recycleBinController);

            var scheduledDownloadsController = new DataController<ScheduledDownload>(
                serializedStorageController,
                DataStoragePolicy.ItemsList,
                productCoreIndexingController,
                collectionController,
                scheduledDownloadsDestinationController,
                recycleBinController);

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
                updatedDataController);

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

            //// dependencies for update controllers

            //var productUpdateUriController = new ProductUpdateUriController();

            //var gameProductDataUpdateUriController = new GameProductDataUpdateUriController();
            //var gameProductDataSkipUpdateController = new GameProductDataSkipUpdateController();
            //var gameProductDataDecodingController = new GameProductDataDecodingController(
            //    gogDataExtractionController,
            //    serializationController);

            //var gameDetailsRequiredUpdatesController = new GameDetailsRequiredUpdatesController(productStorageController);
            //var gameDetailsConnectionController = new GameDetailsConnectionController();
            //var gameDetailsDownloadDetailsController = new GameDetailsDownloadDetailsController(
            //    serializationController,
            //    languageController);

            //// product update controllers

            //var gameProductDataUpdateController = new GameProductDataUpdateController(
            //    productStorageController,
            //    collectionController,
            //    networkController,
            //    serializationController,
            //    null,
            //    gameProductDataUpdateUriController,
            //    gameProductDataSkipUpdateController,
            //    gameProductDataDecodingController,
            //    taskReportingController);

            //var apiProductUpdateController = new ApiProductUpdateController(
            //    productStorageController,
            //    collectionController,
            //    networkController,
            //    serializationController,
            //    null,
            //    productUpdateUriController,
            //    taskReportingController);

            //var gameDetailsUpdateController = new GameDetailsUpdateController(
            //    productStorageController,
            //    collectionController,
            //    networkController,
            //    serializationController,
            //    throttleController,
            //    productUpdateUriController,
            //    gameDetailsRequiredUpdatesController,
            //    gameDetailsConnectionController,
            //    gameDetailsDownloadDetailsController,
            //    taskReportingController);

            //var screenshotUpdateController = new ScreenshotUpdateController(
            //    productStorageController,
            //    collectionController,
            //    networkController,
            //    screenshotExtractionController,
            //    taskReportingController);

            //// dependencies for download controllers

            //var productsImagesDownloadSourcesController = new ProductsImagesDownloadSourcesController(
            //    productStorageController,
            //    imageUriController);

            //var screenshotsDownloadSourcesController = new ScreenshotsDownloadSourcesController(
            //    productStorageController,
            //    screenshotUriController);

            //var productFilesDownloadSourcesController = new ProductFilesDownloadSourcesController(
            //    settings.Download.Languages,
            //    settings.Download.OperatingSystems,
            //    productStorageController);

            //var productExtrasDownloadSourcesController = new ProductExtrasDownloadSourcesController(
            //    productStorageController);

            //var validationUriRedirectController = new ValidationUriRedirectController();

            //var validationDownloadSourcesController = new ValidationDownloadSourcesController(
            //    productStorageController,
            //    validationUriRedirectController);

            //// download controllers

            //var productImagesScheduleDownloadsController = new ProductImagesScheduleDownloadsController(
            //    productsImagesDownloadSourcesController,
            //    imagesDestinationController,
            //    productStorageController,
            //    collectionController,
            //    fileController,
            //    taskReportingController);

            //var screenshotsScheduleDownloadsController = new ScreenshotsScheduleDownloadsController(
            //    screenshotsDownloadSourcesController,
            //    screenshotsDestinationController,
            //    productStorageController,
            //    collectionController,
            //    fileController,
            //    taskReportingController);

            //var productFilesScheduleDownloadsController = new ProductFilesScheduleDownloadsController(
            //    productFilesDownloadSourcesController,
            //    uriRedirectController,
            //    filesExtrasDestinationController,
            //    productStorageController, 
            //    collectionController,
            //    fileController, 
            //    taskReportingController);

            //var productExtrasScheduleDownloadsController = new ProductExtrasScheduleDownloadsController(
            //    productExtrasDownloadSourcesController,
            //    uriRedirectController,
            //    filesExtrasDestinationController,
            //    productStorageController,
            //    collectionController,
            //    fileController,
            //    taskReportingController);

            //var validationScheduleDownloadsController = new ValidationScheduleDownloadsController(
            //    validationDownloadSourcesController,
            //    validationDestinationController,
            //    productStorageController,
            //    collectionController,
            //    fileController,
            //    taskReportingController);

            //var processScheduledDownloadsController = new ProcessScheduledDownloadsController(
            //    productStorageController,
            //    downloadController,
            //    collectionController,
            //    taskReportingController);

            //// validation controllers

            //var byteToStringConversionController = new BytesToStringConvertionController();

            //var validationController = new ValidationController(
            //    validationDestinationController,
            //    fileController,
            //    streamController,
            //    byteToStringConversionController);

            //var processValidationController = new ProcessValidationController(
            //    gogUriDestinationController,
            //    validationController,
            //    productStorageController,
            //    taskReportingController);

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
                //productImagesScheduleDownloadsController,
                //screenshotsScheduleDownloadsController,
                //productFilesScheduleDownloadsController,
                //productExtrasScheduleDownloadsController,
                //validationScheduleDownloadsController,
                //processScheduledDownloadsController,
                //processValidationController
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
