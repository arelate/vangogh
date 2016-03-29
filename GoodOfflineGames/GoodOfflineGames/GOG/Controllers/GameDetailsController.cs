using System;
using System.Collections.Generic;

using GOG.Model;
using GOG.SharedModels;
using GOG.Interfaces;
using Newtonsoft.Json;

namespace GOG.Controllers
{
    public class GameDetailsController : ProductCoreController<GameDetails>
    {
        private IDeserializeDelegate<string> stringDeserializeController;
        private ICollection<string> supportedLanguages;

        public GameDetailsController(
            IList<GameDetails> gameDetails,
            IGetStringDelegate getStringDelegate,
            IDeserializeDelegate<string> stringDeserializeController,
            ICollection<string> supportedLanguages) :
            base(gameDetails, getStringDelegate)
        {
            this.stringDeserializeController = stringDeserializeController;
            this.supportedLanguages = supportedLanguages;
        }

        protected override string GetRequestTemplate()
        {
            return Urls.AccountGameDetailsTemplate;
        }

        private void ExpandDynamicDownloads(ref GameDetails details)
        {
            if (details == null) return;

            details.LanguageDownloads = new List<OperatingSystemsDownloads>();

            //foreach (var entry in details.DynamicDownloads)
            //{
            //    if (entry.Length != 2)
            //        throw new InvalidOperationException("Unsupported data format");

            //    var language = entry[0];

            //    if (!supportedLanguages.Contains(language)) continue;

            //    string downloadsString = JsonConvert.SerializeObject(entry[1]);
            //    var downloads = stringDeserializeController.Deserialize<OperatingSystemsDownloads>(
            //        downloadsString);

            //    downloads.Language = language;
            //    details.LanguageDownloads.Add(downloads);
            //}

            //details.DynamicDownloads = null;

            if (details.DLCs == null) return;

            for (var ii = 0; ii < details.DLCs.Count; ii++)
            {
                var dlc = details.DLCs[ii];
                ExpandDynamicDownloads(ref dlc);
            }
        }



        protected override GameDetails Deserialize(string data)
        {
            // GameDetails are complicated as GOG.com currently serves mixed type array
            // where first entry is string "Language" and next entries are downloads
            // so in order to overcome this we do it like this:
            // 1) use Json.NET to deserialize with downloads mapped to dynamic[][] collection
            // 2) walk through DynamicDownloads collection
            // 3) for each language we expand second collection item (that is operating system downloads)
            // 4) for each DLC we recursively expand DynamicDownloads
            // 5) we nullify DynamicDownloads after expansion to allow further serialization

            // TODO: keep an eye on further changes that might allow to simplify this

            GameDetails gameDetails = JsonConvert.DeserializeObject<GameDetails>(data);

            ExpandDynamicDownloads(ref gameDetails);

            return gameDetails;
        }

        public override GameDetails NewDeserialize()
        {
            var testData = "{\"title\":\"a\",\"backgroundImage\":\"b\",\"cdKey\":\"\",\"textInformation\":\"\",\"downloads\":[[\"English\",{\"windows\":[{\"manualUrl\":\"\\/downlink\\/never_alone_arctic_collection\\/en1installer1\",\"downloaderUrl\":\"gogdownloader:\\/\\/never_alone_arctic_collection\\/installer_win_en\",\"name\":\"Never Alone Arctic Collection\",\"version\":\"1.0 (gog-1)\",\"date\":\"\",\"size\":\"1.9 GB\"}],\"mac\":[{\"manualUrl\":\"\\/downlink\\/never_alone_arctic_collection\\/en2installer1\",\"downloaderUrl\":\"gogdownloader:\\/\\/never_alone_arctic_collection\\/installer__en\",\"name\":\"Never Alone Arctic Collection\",\"version\":\"1.0 (gog-1)\",\"date\":\"\",\"size\":\"2 GB\"}],\"linux\":[{\"manualUrl\":\"\\/downlink\\/never_alone_arctic_collection\\/en3installer1\",\"downloaderUrl\":\"gogdownloader:\\/\\/never_alone_arctic_collection\\/installer__en\",\"name\":\"Never Alone Arctic Collection\",\"version\":\"1.0 (gog-1)\",\"date\":\"\",\"size\":\"3.9 GB\"}]}]],\"extras\":[{\"manualUrl\":\"\\/downlink\\/file\\/never_alone_arctic_collection\\/62403\",\"downloaderUrl\":\"gogdownloader:\\/\\/never_alone_arctic_collection\\/62403\",\"name\":\"soundtrack (mp3)\",\"type\":\"audio\",\"info\":1,\"size\":\"118 MB\"}],\"dlcs\":[],\"tags\":[],\"isPreOrder\":false,\"releaseTimestamp\":1446731400,\"messages\":[],\"changelog\":\"\",\"forumLink\":\"https:\\/\\/www.gog.com\\/forum\\/never_alone_arctic_collection\",\"isBaseProductMissing\":false,\"missingBaseProduct\":null}";

            return Deserialize(testData);
        }
    }
}
