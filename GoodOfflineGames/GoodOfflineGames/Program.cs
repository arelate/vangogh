using System;
using System.Collections.Generic;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Network;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.Extraction;
using Controllers.Collection;
using Controllers.Console;
using Controllers.Reporting;
using Controllers.Settings;
using Controllers.RequestPage;
using Controllers.Politeness;

using Interfaces.TaskActivity;

using GOG.TaskActivities.Authorization;
using GOG.TaskActivities.Update.PageResult;
using GOG.TaskActivities.Update.NewUpdatedAccountProducts;
using GOG.TaskActivities.Update.Wishlist;
using GOG.TaskActivities.Update.Dependencies.Product;
using GOG.TaskActivities.Update.Dependencies.GameDetails;
using GOG.TaskActivities.Update.Dependencies.GameProductData;
using GOG.TaskActivities.Update.Products;
using GOG.TaskActivities.Update.Screenshots;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            var streamController = new StreamController();
            var fileController = new FileController();
            var directoryController = new DirectoryController();
            var storageController = new StorageController(
                streamController,
                fileController);

            var consoleController = new ConsoleController();
            var taskReportingController = new TaskReportingController(consoleController);

            var uriController = new UriController();
            var networkController = new NetworkController(uriController);
            var requestPageController = new RequestPageController(
                networkController);

            var languageController = new LanguageController();

            var serializationController = new JSONStringController();

            var extractionController = new TokenExtractionController();
            var gogDataExtractionController = new GOGDataExtractionController();
            var screenshotExtractionController = new ScreenshotExtractionController();

            var collectionController = new CollectionController();

            var politenessController = new PolitenessController();

            var productStorageController = new ProductStorageController(
                storageController,
                serializationController);

            // Load settings that (might) have authorization information, and request to run or not specific task activities

            var settingsController = new SettingsController(
                storageController,
                serializationController,
                languageController,
                consoleController);

            taskReportingController.StartTask("Load settings");
            //var settings = settingsController.Load().Result;
            taskReportingController.CompleteTask();
            consoleController.WriteLine(string.Empty);

            // Create and add all task activity controllersa
            // Task activities are encapsulated set of activity - so no data can be passed around!
            // Individual task activity would need to load data it needs from the disk / network

            //// temporary code to transform old dictionary schema to new list to align better with productStorageController
            //var screenshotsContent = storageController.Pull("screenshots.js").Result;
            //screenshotsContent = screenshotsContent.Replace("var screenshots=", "");
            //var screenshots = serializationController.Deserialize<Dictionary<long, string[]>>(screenshotsContent);
            //var newScreenshots = new List<GOG.Models.Custom.ProductScreenshots>(screenshots.Count);
            //foreach (var kvp in screenshots)
            //{
            //    var newScreenshot = new GOG.Models.Custom.ProductScreenshots();
            //    if (kvp.Value == null) continue;
            //    newScreenshot.Id = kvp.Key;
            //    newScreenshot.Uris = new List<string>(kvp.Value);
            //    newScreenshots.Add(newScreenshot);
            //}
            //var newScreenshotsContent = "var screenshots=" + serializationController.Serialize(newScreenshots);
            //storageController.Push("newscreenshots.js", newScreenshotsContent).Wait();
            //return;

            //var authorizationController = new AuthorizationController(
            //    uriController,
            //    networkController,
            //    extractionController,
            //    consoleController,
            //    settings.Authenticate,
            //    taskReportingController);

            var productsUpdateController = new ProductsUpdateController(
                requestPageController,
                serializationController,
                productStorageController,
                taskReportingController);

            var accountProductsUpdateController = new AccountProductsUpdateController(
                requestPageController,
                serializationController,
                productStorageController,
                taskReportingController);

            var newUpdatedAccountProductsController = new NewUpdatedAccountProductsController(
                productStorageController,
                taskReportingController);

            var wishlistedUpdateController = new WishlistedUpdateController(
                networkController,
                gogDataExtractionController,
                serializationController,
                productStorageController,
                taskReportingController);

            // dependencies for update controllers

            var productUpdateUriController = new ProductUpdateUriController();

            var gameProductDataUpdateUriController = new GameProductDataUpdateUriController();
            var gameProductDataSkipUpdateController = new GameProductDataSkipUpdateController();
            var gameProductDataDecodingController = new GameProductDataDecodingController(
                gogDataExtractionController, 
                serializationController);

            var gameDetailsRequiredUpdatesController = new GameDetailsRequiredUpdatesController(productStorageController);
            var gameDetailsConnectionController = new GameDetailsConnectionController();

            var gameProductDataUpdateController = new GameProductDataUpdateController(
                productStorageController,
                collectionController,
                networkController,
                serializationController,
                null,
                gameProductDataUpdateUriController,
                gameProductDataSkipUpdateController,
                gameProductDataDecodingController,
                taskReportingController);

            var apiProductUpdateController = new ApiProductUpdateController(
                productStorageController,
                collectionController,
                networkController,
                serializationController,
                null,
                productUpdateUriController,
                taskReportingController);

            var gameDetailsUpdateController = new GameDetailsUpdateController(
                productStorageController,
                collectionController,
                networkController,
                serializationController,
                politenessController,
                productUpdateUriController,
                gameDetailsRequiredUpdatesController,
                gameDetailsConnectionController,
                taskReportingController);

            var screenshotUpdateController = new ScreenshotUpdateController(
                productStorageController,
                collectionController,
                networkController,
                screenshotExtractionController,
                taskReportingController);

            // Iterate and process all tasks

            var taskActivityControllers = new List<ITaskActivityController>()
            {
                //authorizationController,
                //productsUpdateController,
                //accountProductsUpdateController,
                //newUpdatedAccountProductsController,
                //wishlistedUpdateController,
                gameProductDataUpdateController,
                //apiProductUpdateController,
                //gameDetailsUpdateController,
                //screenshotUpdateController
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
        }
    }
}
