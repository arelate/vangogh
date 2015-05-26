using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public class GOGDataController: IDataRequestController
    {
        private const string gogDataPrefix = "var gogData = ";
        private Regex regex = new Regex(gogDataPrefix + "(.*)");
        private ISerializationController serializationController;
        private IStringRequestController stringRequestController;

        public GOGDataController(ISerializationController serializationController, IStringRequestController stringRequestController)
        {
            this.serializationController = serializationController;
            this.stringRequestController = stringRequestController;
        }

        public async Task<T> RequestData<T>(string uri, Dictionary<string, string> parameters = null)
        {
            var gamePageContent = await stringRequestController.RequestString(uri);

            var match = regex.Match(gamePageContent);
            string gogDataString = match.Value.Substring(
                gogDataPrefix.Length, // drop the prefix var gogData = 
                match.Value.Length - gogDataPrefix.Length - 1); // and closing ";"

            return serializationController.Parse<T>(gogDataString);
        }
    }
}
