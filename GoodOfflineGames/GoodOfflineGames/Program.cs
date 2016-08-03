using System;
using System.IO;

using Interfaces.Serialization;

using Controllers.Settings;
using Controllers.Serialization;

using Controllers.Console;
using Controllers.Language;
using Controllers.Stream;
using Controllers.Storage;
using Controllers.File;
using Controllers.Directory;

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

            var languageController = new LanguageController();

            var jsonController = new JSONStringController();

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

            Console.WriteLine(settings);
        }
    }
}
