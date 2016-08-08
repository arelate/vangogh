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

            // Load settings that (might) have authorization information, and request to run or not specific task activities

            var settingsController = new SettingsController(
                storageController,
                jsonController,
                languageController,
                consoleController);

            taskReportingController.AddTask("Load settings");
            var settings = settingsController.Load().Result;
            taskReportingController.CompleteTask();

            // Create and add all task activity controllers
            // Task activities are encapsulated set of activity - so no data can be passed around!
            // Individual task activity would need to load data it needs from the disk / network

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

            var accountProductsUpdateTaskActivityController = new AccountProductsUpdateTaskActivityController(
                requestPageController,
                jsonController,
                storageController,
                taskReportingController);

            var updatedProductsUpdateTaskActivityController = new UpdatedProductsUpdateTaskActivityController(
                requestPageController,
                jsonController,
                storageController,
                taskReportingController);

            // Iterate and process all tasks

            var taskActivityControllers = new List<TaskActivityController>();

            taskActivityControllers.Add(authorizationTaskActivityController);
            taskActivityControllers.Add(productsUpdateTaskActivityController);
            taskActivityControllers.Add(accountProductsUpdateTaskActivityController);
            taskActivityControllers.Add(updatedProductsUpdateTaskActivityController);

            foreach (var taskActivityController in taskActivityControllers)
                try
                {
                    taskActivityController.ProcessTask().Wait();
                }
                catch (AggregateException ex)
                {
                    List<string> errorMessages = new List<string>();
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        errorMessages.Add(innerException.Message);
                    }
                    taskReportingController.ReportFailure(string.Join(", ", errorMessages));
                    break;
                }

            consoleController.WriteLine("Press ENTER to continue...");
            consoleController.ReadLine();
        }
    }
}
