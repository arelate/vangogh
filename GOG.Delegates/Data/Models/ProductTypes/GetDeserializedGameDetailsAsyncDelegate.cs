using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attributes;
using Delegates.Collections.System;
using Delegates.Conversions;
using Delegates.Conversions.Strings;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using GOG.Delegates.Itemizations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    // TODO: Refactor?
    public class GetDeserializedGameDetailsAsyncDelegate : IGetDataAsyncDelegate<GameDetails, string>
    {
        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;

        private readonly IConvertDelegate<string, GameDetails> convertJSONToGameDetailsDelegate;

        private readonly IConvertDelegate<string, OperatingSystemsDownloads[][]>
            convertJSONToOperatingSystemsDownloads2DArrayDelegate;

        private readonly IConvertDelegate<string, string> convertLanguageToCodeDelegate;
        private readonly IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate;
        private readonly IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate;
        private readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate;
        private readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate;
        private readonly IConvertDelegate<(string, string[]), string> convertStringToReplaceMarkersWithEmptyStringDelegate;

        private readonly IConvertDelegate<
            OperatingSystemsDownloads[][],
            OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate;

        private readonly IMapDelegate<string> mapStringDelegate;

        [Dependencies(
            typeof(Network.GetUriDataPolitelyAsyncDelegate),
            typeof(ConvertJSONToGameDetailsDelegate),
            typeof(ConvertJSONToOperatingSystemsDownloads2DArrayDelegate),
            typeof(ConvertLanguageToCodeDelegate),
            typeof(ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate),
            typeof(ConfirmGameDetailsContainsLanguageDelegate),
            typeof(ItemizeGameDetailsDownloadLanguagesDelegate),
            typeof(ItemizeGameDetailsDownloadsDelegate),
            typeof(ConvertStringToReplaceMarkersWithEmptyStringDelegate),
            typeof(ConvertOperatingSystemsDownloads2DArrayToArrayDelegate),
            typeof(MapStringDelegate))]
        public GetDeserializedGameDetailsAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IConvertDelegate<string, GameDetails> convertJSONToGameDetailsDelegate,
            IConvertDelegate<string, OperatingSystemsDownloads[][]>
                convertJSONToOperatingSystemsDownloads2DArrayDelegate,
            IConvertDelegate<string, string> convertLanguageToCodeDelegate,
            IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate,
            IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate,
            IConvertDelegate<(string, string[]), string> convertStringToReplaceMarkersWithEmptyStringDelegate,
            IConvertDelegate<
                OperatingSystemsDownloads[][],
                OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate,
            IMapDelegate<string> mapStringDelegate)
        {
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.convertJSONToGameDetailsDelegate = convertJSONToGameDetailsDelegate;
            this.convertJSONToOperatingSystemsDownloads2DArrayDelegate =
                convertJSONToOperatingSystemsDownloads2DArrayDelegate;
            this.convertLanguageToCodeDelegate = convertLanguageToCodeDelegate;
            this.convertGameDetailsDownloadLanguagesToEmptyStringDelegate =
                convertGameDetailsDownloadLanguagesToEmptyStringDelegate;
            this.confirmStringContainsLanguageDownloadsDelegate = confirmStringContainsLanguageDownloadsDelegate;
            this.itemizeGameDetailsDownloadLanguagesDelegate = itemizeGameDetailsDownloadLanguagesDelegate;
            this.itemizeGameDetailsDownloadsDelegate = itemizeGameDetailsDownloadsDelegate;
            this.convertStringToReplaceMarkersWithEmptyStringDelegate = convertStringToReplaceMarkersWithEmptyStringDelegate;
            this.convert2DArrayToArrayDelegate = convert2DArrayToArrayDelegate;
            this.mapStringDelegate = mapStringDelegate;
        }

        public async Task<GameDetails> GetDataAsync(string uri)
        {
            // GOG.com quirk
            // GameDetails as sent by GOG.com servers have an intersting data structure for downloads:
            // it's represented as an array, where first element is a string with the language,
            // followed by actual download details, something like:
            // [ "English", { // download1 }, { // download2 } ]
            // Which of course is not a problem for JavaScript, but is a problem for
            // deserializing into strongly typed C# data structures. 
            // To work around this we wrapped encapsulated usual network requests,
            // data transformation and desearialization in a sinlge package. 
            // To process downloads we do the following:
            // - if response contains language downloads, signified by [[
            // - extract actual language information and remove it from the string
            // - deserialize downloads into OperatingSystemsDownloads collection
            // - assign languages, since we know we should have as many downloads array as languages

            var data = await getUriDataAsyncDelegate.GetDataAsync(uri);
            var gameDetails = convertJSONToGameDetailsDelegate.Convert(data);

            if (gameDetails == null) return null;

            var gameDetailsLanguageDownloads = new List<OperatingSystemsDownloads>();

            if (!confirmStringContainsLanguageDownloadsDelegate.Confirm(data)) return gameDetails;
            var downloadStrings = itemizeGameDetailsDownloadsDelegate.Itemize(data);

            foreach (var downloadString in downloadStrings)
            {
                var downloadLanguages = itemizeGameDetailsDownloadLanguagesDelegate.Itemize(downloadString);
                if (downloadLanguages == null)
                    throw new InvalidOperationException(
                        "Cannot find any download languages or download language format changed.");

                // ... and remove download lanugage strings from downloads
                var downloadsStringSansLanguages = convertStringToReplaceMarkersWithEmptyStringDelegate.Convert(
                    (downloadString,
                    downloadLanguages.ToArray()));

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    convertJSONToOperatingSystemsDownloads2DArrayDelegate.Convert(
                        downloadsStringSansLanguages);

                // and convert GOG two-dimensional array of downloads to single-dimensional array
                var languageDownloads = convert2DArrayToArrayDelegate.Convert(downloads);

                if (languageDownloads.Count() != downloadLanguages.Count())
                    throw new InvalidOperationException(
                        "Number of extracted language downloads doesn't match number of languages.");

                // map language downloads with the language code we extracted earlier
                var languageDownloadIndex = 0;

                mapStringDelegate.Map(downloadLanguages, language =>
                {
                    var formattedLanguage = convertGameDetailsDownloadLanguagesToEmptyStringDelegate.Convert(language);
                    var languageCode = convertLanguageToCodeDelegate.Convert(formattedLanguage);

                    languageDownloads[languageDownloadIndex++].Language = languageCode;
                });

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();

            return gameDetails;
        }
    }
}