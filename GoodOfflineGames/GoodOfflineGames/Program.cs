using System;
using System.IO;

using GOG.Models;
using Interfaces.Serialization;
using Controllers.Serialization;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\boggydigital\Desktop\api1433856545.json";
            var contents = "";

            using (var fileStream = new FileStream(path, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                    contents = streamReader.ReadToEnd();

            var jsonController = new JSONStringController();

            var data = jsonController.Deserialize<ApiProduct>(contents);

            Console.WriteLine(data);
        }
    }
}
