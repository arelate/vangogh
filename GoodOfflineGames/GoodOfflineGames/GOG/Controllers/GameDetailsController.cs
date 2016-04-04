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

            var data = "{\"title\":\"Don\\u0027t Starve\",\"backgroundImage\":\"\\/\\/images-1.gog.com\\/0b83d6db90343a788803fa38f17bc8044f243124245bee17bac7d1bbf2d69223\",\"cdKey\":\"6BMW-EQAL-RH0V-9Y2D-U187\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/dont_starve\\/en1installer11\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve\\/installer_win_en\",\"name\":\"Don\\u0027t Starve\",\"version\":\"20160401 (gog-21)\",\"date\":\"\",\"size\":\"268 MB\"}],\"mac\":[{\"manualUrl\":\"\\/downlink\\/dont_starve\\/en2installer12\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve\\/installer__en\",\"name\":\"Don\\u0027t Starve\",\"version\":\"20160401 (gog-9)\",\"date\":\"\",\"size\":\"182 MB\"}],\"linux\":[{\"manualUrl\":\"\\/downlink\\/dont_starve\\/en3installer3\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve\\/installer__en\",\"name\":\"Don\\u0027t Starve\",\"version\":\"20160401 (gog-5)\",\"date\":\"\",\"size\":\"345 MB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/dont_starve\\/22233\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve\\/22233\",\"name\":\"wallpapers\",\"type\":\"wallpapers\",\"info\":4,\"size\":\"16 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/dont_starve\\/22243\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve\\/22243\",\"name\":\"avatars\",\"type\":\"avatars\",\"info\":10,\"size\":\"1 MB\"},{\"manualUrl\":\"\\/downlink\\/file\\/dont_starve\\/66473\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve\\/66473\",\"name\":\"Russian localization (ZoneOfGames.ru)\",\"type\":\"game add-ons\",\"info\":1,\"size\":\"4 MB\"}],\"dlcs\":[{\"title\":\"Don\\u0027t Starve: Reign of Giants\",\"backgroundImage\":\"\\/\\/images-2.gog.com\\/cc9c234f14fa47a6d53d2e24fb16b9dc05da97c6426594ef692c218d6b3da1cc\",\"cdKey\":\"X81D-70NP-VRJB-UBED-D5L1\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/dont_starve_reign_of_giants\\/en1installer3\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve_reign_of_giants\\/installer_win_en\",\"name\":\"DLC\",\"version\":\"20160331 (gog-20)\",\"date\":\"\",\"size\":\"68 MB\"}],\"mac\":[{\"manualUrl\":\"\\/downlink\\/dont_starve_reign_of_giants\\/en2installer3\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve_reign_of_giants\\/installer__en\",\"name\":\"DLC\",\"version\":\"20160331 (gog-9)\",\"date\":\"\",\"size\":\"69 MB\"}],\"linux\":[{\"manualUrl\":\"\\/downlink\\/dont_starve_reign_of_giants\\/en3installer2\",\"downloaderUrl\":\"gogdownloader:\\/\\/dont_starve_reign_of_giants\\/installer__en\",\"name\":\"DLC\",\"version\":\"20160331 (gog-3)\",\"date\":\"\",\"size\":\"139 MB\"}]}]],\"extras\":[],\"dlcs\":[],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1398833940,\"messages\":[],\"changelog\":null,\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/dont_starve\",\"isBaseProductMissing\":false,\"missingBaseProduct\":null}],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1366696800,\"messages\":[],\"changelog\":\"\\u003Ch4\\u003EUpdate (04 April 2016)\\u003C\\/h4\\u003E\\n\\u003Cul\\u003E\\n\\u003Cli\\u003EThe 64-bit Linux version has been added.\\u003C\\/li\\u003E\\n\\u003C\\/ul\\u003E\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/dont_starve\",\"isBaseProductMissing\":false,\"missingBaseProduct\":null}";

            var gd = Deserialize(data);
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

            // extract downloads strings...

            var downloadStrings = new List<string>();

            while (gameDetailsDownloadsController.ContainsLanguageDownloads(data))
            {
                var downloadString = gameDetailsDownloadsController.ExtractSingle(data);
                downloadStrings.Add(downloadString);
                // ...and sanitize it from original string
                data = gameDetailsDownloadsController.SanitizeSingle(data, downloadString);
            }

            // deserialize game details object without downloads information
            GameDetails gameDetails = stringDeserializeController.Deserialize<GameDetails>(data);

            gameDetails.LanguageDownloads = new List<OperatingSystemsDownloads>();

            if (gameDetails == null)
                throw new InvalidOperationException("Cannot deserialize game details.");

            // now we can proceed downloads strings
            // extract the languages that are used as first element in each array...
            foreach (var downloadString in downloadStrings)
            {
                var languages = gameDetailsDownloadsController.ExtractMultiple(downloadString);
                if (languages == null)
                    throw new InvalidOperationException("Download string doesn't seem to contain languages. Please check if GOG.com changed JSON data format.");

                // ... and sanitize lanugage strings from downloads
                var sanitizedDownloadsString =
                    gameDetailsDownloadsController.SanitizeMultiple(
                    downloadString, languages);

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    stringDeserializeController.Deserialize<OperatingSystemsDownloads[][]>(
                    sanitizedDownloadsString);

                // and convert GOG multidimensional array of downloads to linear list
                gameDetails.LanguageDownloads.AddRange(gameDetailsDownloadsController.ExtractLanguageDownloads(
                    downloads,
                    languages,
                    requestedLanguages));
            }

            return gameDetails;
        }
    }
}
