using System;
using System.Linq;
using System.Collections.Generic;

using GOG.Model;
using GOG.SharedModels;
using GOG.Interfaces;

namespace GOG.Controllers
{
    public class GameDetailsController : ProductCoreController<GameDetails>
    {
        private IDeserializeDelegate<string> stringDeserializeController;
        private IGameDetailsDownloadsController gameDetailsDownloadsController;
        private ICollection<string> requestedLanguages;

        public GameDetailsController(
            IList<GameDetails> gameDetails,
            IGetStringDelegate getStringDelegate,
            IDeserializeDelegate<string> stringDeserializeController,
            IGameDetailsDownloadsController gameDetailsDownloadsController,
            ICollection<string> requestedLanguages) :
            base(gameDetails, getStringDelegate)
        {
            this.stringDeserializeController = stringDeserializeController;
            this.gameDetailsDownloadsController = gameDetailsDownloadsController;
            this.requestedLanguages = requestedLanguages;
        }

        protected override string GetRequestTemplate()
        {
            return Urls.AccountGameDetailsTemplate;
        }

        protected override GameDetails Deserialize(string data)
        {
            // GameDetails are complicated as GOG.com currently serves mixed type array
            // where first entry is string "Language" and next entries are downloads
            // so in order to overcome this we do it like this:
            // 1) extract downloads substring from the raw JSON
            // 2) deserialize 
            // 3) for each language we expand second collection item (that is operating system downloads)
            // 4) for each DLC we recursively expand DynamicDownloads
            // 5) we nullify DynamicDownloads after expansion to allow further serialization

            // extract downloads string...
            var downloadsString = gameDetailsDownloadsController.ExtractSingle(data);
            // ...and sanitize it from original string
            data = gameDetailsDownloadsController.SanitizeSingle(data, downloadsString);

            // deserialize game details object without downloads information
            GameDetails gameDetails = stringDeserializeController.Deserialize<GameDetails>(data);

            if (gameDetails == null)
                throw new InvalidOperationException("Cannot deserialize game details.");

            // now we can proceed downloads string
            // extract the languages that are used as first element in each array...
            var languages = gameDetailsDownloadsController.ExtractMultiple(downloadsString);
            if (languages == null)
                throw new InvalidOperationException("Download string doesn't seem to contain languages. Please check if GOG.com changed JSON data format.");

            // ... and sanitize lanugage strings from downloads
            var sanitizedDownloadsString = 
                gameDetailsDownloadsController.SanitizeMultiple(
                downloadsString, languages);

            // now it should be safe to deserialize langugage downloads
            var downloads = 
                stringDeserializeController.Deserialize<OperatingSystemsDownloads[][]>(
                sanitizedDownloadsString);

            // and convert GOG multidimensional array of downloads to linear list
            gameDetails.LanguageDownloads = gameDetailsDownloadsController.ExtractLanguageDownloads(
                downloads, 
                languages,
                requestedLanguages);

            return gameDetails;
        }
    }
}
