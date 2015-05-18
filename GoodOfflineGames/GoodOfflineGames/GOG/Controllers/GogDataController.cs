using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GOG
{
    public static class GogDataController
    {
        private const string gogDataPrefix = "var gogData = ";
        private static Regex regex = new Regex(gogDataPrefix + "(.*)");

        public static async Task<T> RequestData<T>(string uri)
        {
            var gamePageContent = await NetworkController.RequestString(uri);

            var match = regex.Match(gamePageContent);
            string gogDataString = match.Value.Substring(
                gogDataPrefix.Length, // drop the prefix var gogData = 
                match.Value.Length - gogDataPrefix.Length - 1); // and closing ";"

            return JSONController.Parse<T>(gogDataString);
        }
    }
}
