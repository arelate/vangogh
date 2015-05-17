using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleController = new ConsoleController();
            var ioController = new IOController();
            var storage = new Storage(ioController);

            var jsonDataPrefix = "var data = ";
            var filename = "data.js";
            ProductsResult storedGames = null;

            try
            {
                var storedGamesJson = storage.Pull(filename)
                    .Result
                    .Replace(jsonDataPrefix, string.Empty);
                storedGames = JSON.Parse<ProductsResult>(storedGamesJson);
            }
            catch
            {
                // file not found or couldn't be read
                storedGames = new ProductsResult();
            }

            Settings settings = Settings.LoadSettings(consoleController, ioController).Result;

            Auth.AuthorizeOnSite(settings, consoleController).Wait();

            // get all available games from gog.com/games

            var gamesResult = ProductsResult.RequestUpdated(storedGames,
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                consoleController,
                "Getting all games available on GOG.com...").Result;

            // get all owned games from gog.com/account

            var accountResult = ProductsResult.RequestUpdated(storedGames,
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                consoleController,
                "Getting account games...").Result;

            // mark all owned games as owned

            accountResult.MarkAllAsOwned();

            // merge owned games into all available games 

            gamesResult.MergeOwned(accountResult);

            // serialize and save on disk

            var gamesResultJson = "var data = " + JSON.Stringify(gamesResult);
            storage.Put(filename, gamesResultJson).Wait();

            // download new images 
            var images = new Images(ioController, consoleController);
            images.Update(gamesResult).Wait();

            // nothing left to do here

            Console.WriteLine("All done. Press ENTER to quit...");
            Console.ReadLine();
        }
    }

    #region Console controller

    class ConsoleController : IConsoleController
    {
        public string Read()
        {
            return Console.Read().ToString();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public string ReadPrivateLine()
        {
            ConsoleKeyInfo key;
            string privateData = string.Empty;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                privateData += key.KeyChar;
            }
            return privateData;
        }

        public void Write(string message, params object[] data)
        {
            Console.Write(message, data);
        }

        public void WriteLine(string message, params object[] data)
        {
            Console.WriteLine(message, data);
        }
    }

    #endregion

    #region IOController

    class IOController : IIOController
    {
        public Stream OpenReadable(string uri)
        {
            return new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream OpenWritable(string uri)
        {
            return new FileStream(uri, FileMode.Create, FileAccess.Write, FileShare.Read);
        }

        public bool ExistsFile(string uri)
        {
            return File.Exists(uri);
        }

        public bool ExistsDirectory(string uri)
        {
            return Directory.Exists(uri);
        }

        public void CreateDirectory(string uri)
        {
            Directory.CreateDirectory(uri);
        }
    }

    #endregion

}
