using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using GOG.Interfaces.Delegates.GetDeserialized;
using Attributes;
using GOG.Models;
using Delegates.Convert;
using Delegates.Replace;
using Delegates.Collections.System;
using Delegates.Convert.Uri;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    // TODO: Refactor?
    public class GetDeserializedGameDetailsAsyncDelegate : IGetDeserializedAsyncDelegate<GameDetails>
    {
        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;

        private readonly IConvertDelegate<(string, IDictionary<string, string>), string>
            convertUriParametersToUriDelegate;

        private readonly IConvertDelegate<string, GameDetails> convertJSONToGameDetailsDelegate;

        private readonly IConvertDelegate<string, OperatingSystemsDownloads[][]>
            convertJSONToOperatingSystemsDownloads2DArrayDelegate;

        private readonly IConvertDelegate<string, string> convertLanguageToCodeDelegate;
        private readonly IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate;
        private readonly IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate;
        private readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate;
        private readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate;
        private readonly IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        private readonly IConvertDelegate<
            OperatingSystemsDownloads[][],
            OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate;

        private readonly IMapDelegate<string> mapStringDelegate;

        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(Data.Network.GetUriDataRateLimitedAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToGameDetailsDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToOperatingSystemsDownloads2DArrayDelegate),
            typeof(ConvertLanguageToCodeDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmGameDetailsContainsLanguageDelegate),
            typeof(GOG.Delegates.Itemize.ProductTypes.ItemizeGameDetailsDownloadLanguagesDelegate),
            typeof(GOG.Delegates.Itemize.ProductTypes.ItemizeGameDetailsDownloadsDelegate),
            typeof(ReplaceMultipleStringsDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertOperatingSystemsDownloads2DArrayToArrayDelegate),
            typeof(MapStringDelegate))]
        public GetDeserializedGameDetailsAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IConvertDelegate<string, GameDetails> convertJSONToGameDetailsDelegate,
            IConvertDelegate<string, OperatingSystemsDownloads[][]>
                convertJSONToOperatingSystemsDownloads2DArrayDelegate,
            IConvertDelegate<string, string> convertLanguageToCodeDelegate,
            IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate,
            IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate,
            IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate,
            IConvertDelegate<
                OperatingSystemsDownloads[][],
                OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate,
            IMapDelegate<string> mapStringDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
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
            this.replaceMultipleStringsDelegate = replaceMultipleStringsDelegate;
            this.convert2DArrayToArrayDelegate = convert2DArrayToArrayDelegate;
            this.mapStringDelegate = mapStringDelegate;
        }

        public async Task<GameDetails> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
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

            var uriParameters = convertUriParametersToUriDelegate.Convert((uri, parameters));
            var data = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);
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
                var downloadsStringSansLanguages = replaceMultipleStringsDelegate.ReplaceMultiple(
                    downloadString,
                    string.Empty,
                    downloadLanguages.ToArray());

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