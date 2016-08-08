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
using Controllers.Console;
using Controllers.Reporting;
using Controllers.Settings;
using Controllers.RequestPage;

using GOG.Controllers.TaskActivity;
using Interfaces.TaskActivity;

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

            var jsonController = new JSONStringController();

            var extractionController = new ExtractionController();

            var settingsController = new SettingsController(
                storageController,
                jsonController,
                languageController,
                consoleController);

            // Load settings that (might) have authorization information, and request to run or not specific task activities

            taskReportingController.AddTask("Load settings");
            var settings = settingsController.Load().Result;
            taskReportingController.CompleteTask();

            // Create and add all task activity controllers
            // Task activities are encapsulated set of activity - so no data can be passed around!

            var authorizationTaskActivityController = new AuthorizationTaskActivityController(
                uriController,
                networkController,
                extractionController,
                consoleController,
                settings.Authenticate,
                taskReportingController);

            var productsUpdateTaskActivityController = new ProductsUpdateTaskActivityController(
                requestPageController,
                jsonController,
                storageController,
                taskReportingController);

            var taskActivityControllers = new List<ITaskActivityController>();

            taskActivityControllers.Add(authorizationTaskActivityController);
            taskActivityControllers.Add(productsUpdateTaskActivityController);

            // Iterate and process all tasks

            foreach (var taskActivityController in taskActivityControllers)
                taskActivityController.ProcessTask().Wait();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();

            //var accountProductsPageResultsController = new AccountProductsPageResultController(
            //    requestPageController,
            //    jsonController,
            //    Uris.Paths.Account.GetFilteredProducts,
            //    QueryParameters.AccountGetFilteredProducts);

            //var updatedProductsPageResultsController = new AccountProductsPageResultController(
            //    requestPageController,
            //    jsonController,
            //    Uris.Paths.Account.GetFilteredProducts,
            //    QueryParameters.AccountGetUpdatedFilteredProducts);

        }
    }
}
