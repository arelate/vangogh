using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using GOG.Models;
using GOG.Interfaces.Models;
using Interfaces.Serialization;
using Controllers.Serialization;

namespace GoodOfflineGames
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\boggydigital\Desktop\account.json";
            var contents = "";

            using (var fileStream = new FileStream(path, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                    contents = streamReader.ReadToEnd();

            var jsonController = new JSONStringController();
            var data = jsonController.Deserialize<AccountProduct>(contents);

            Console.WriteLine(data);
        }
    }
}
