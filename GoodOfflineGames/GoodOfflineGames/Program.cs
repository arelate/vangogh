using System;
using System.IO;

using GOG.Models;
using Interfaces.Serialization;
using Controllers.Serialization;
using Controllers.IO.Stream;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\boggydigital\Desktop\accountPageResult.json";
            var contents = "";

            var streamController = new StreamController();

            using (var fileStream = streamController.OpenReadable(path))
                using (var streamReader = new StreamReader(fileStream))
                    contents = streamReader.ReadToEnd();

            var jsonController = new JSONStringController();

            var data = jsonController.Deserialize<AccountProductsPageResult>(contents);

            Console.WriteLine(data);
        }
    }
}
