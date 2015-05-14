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
            var streamController = new StreamController();

            Settings settings = Settings.LoadSettings(consoleController, streamController).Result;

            Auth.AuthorizeOnSite(settings, consoleController).Wait();

            //string data = Network.Request(Urls.AccountGetFilteredProducts, QueryParameters.AccountGetFilteredProducts).Result;
        }
    }

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

    class StreamController : IStreamController
    {
        public Stream OpenReadable(string uri)
        {
            return new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream OpenWritable(string uri)
        {
            return new FileStream(uri, FileMode.Create, FileAccess.Write, FileShare.Read);
        }
    }

}
