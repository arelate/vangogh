using System;

using Interfaces.Console;

using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;
using Controllers.Network;
using Controllers.Language;
using Controllers.Serialization;
using Controllers.Extraction;
using Controllers.Console;
using Controllers.Settings;

using GOG.Controllers.Authorization;

using GOG.Models;

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

            var uriController = new UriController();
            var networkController = new NetworkController(uriController);

            var languageController = new LanguageController();

            var jsonController = new JSONStringController();

            var extractionController = new ExtractionController();

            var consoleController = new ConsoleController();

            var settingsController = new SettingsController(
                storageController, 
                jsonController, 
                languageController, 
                consoleController);

            var settings = settingsController.Load().Result;

            //var path = @"C:\Users\boggydigital\Desktop\accountPageResult.json";
            //var contents = "";

            //using (var fileStream = streamController.OpenReadable(path))
            //    using (var streamReader = new StreamReader(fileStream))
            //        contents = streamReader.ReadToEnd();

            //var data = jsonController.Deserialize<AccountProductsPageResult>(contents);

            var authorizationController = new AuthorizationController(
                uriController, 
                networkController, 
                extractionController, 
                consoleController);

            try
            {
                authorizationController.Authorize(settings.Authenticate).Wait();
            }
            catch (System.Security.SecurityException ex)
            {
                consoleController.WriteLine(ex.Message, MessageType.Error);
            }

            Console.WriteLine(settings);
        }
    }
}
