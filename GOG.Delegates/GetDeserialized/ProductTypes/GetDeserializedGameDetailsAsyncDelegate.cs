using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Map;
using Interfaces.Models.Dependencies;

using Interfaces.Controllers.Network;

using Interfaces.Controllers.Serialization;

using GOG.Interfaces.Delegates.GetDeserialized;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    // TODO: Refactor?
    public class GetDeserializedGameDetailsAsyncDelegate : IGetDeserializedAsyncDelegate<GameDetails>
    {
        readonly IGetResourceAsyncDelegate getResourceAsyncDelegate;
        readonly ISerializationController<string> serializationController;
        readonly IConvertDelegate<string,string> convertLanguageToCodeDelegate;
        readonly IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate;
        readonly IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate;
        readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate;
        readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate;
        readonly IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;
        readonly IConvertDelegate<
            OperatingSystemsDownloads[][],
            OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate;
        readonly IMapDelegate<string> mapStringDelegate;

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Network.NetworkController,Controllers",
            Dependencies.JSONSerializationController,
            "Delegates.Convert.ConvertLanguageToCodeDelegate,Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmGameDetailsContainsLanguageDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ProductTypes.ItemizeGameDetailsDownloadLanguagesDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ProductTypes.ItemizeGameDetailsDownloadsDelegate,GOG.Delegates",
            "Delegates.Replace.ReplaceMultipleStringsDelegate,Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertOperatingSystemsDownloads2DArrayToArrayDelegate,GOG.Delegates",
            "Delegates.Map.System.MapStringDelegate,Delegates")]
        public GetDeserializedGameDetailsAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            ISerializationController<string> serializationController,
            IConvertDelegate<string,string> convertLanguageToCodeDelegate,
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
            this.getResourceAsyncDelegate = getResourceAsyncDelegate;
            this.serializationController = serializationController;
            this.convertLanguageToCodeDelegate = convertLanguageToCodeDelegate;
            this.convertGameDetailsDownloadLanguagesToEmptyStringDelegate = convertGameDetailsDownloadLanguagesToEmptyStringDelegate;

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

            var data = await getResourceAsyncDelegate.GetResourceAsync(uri, parameters);
            var gameDetails = serializationController.Deserialize<GameDetails>(data);

            if (gameDetails == null) return null;

            var gameDetailsLanguageDownloads = new List<OperatingSystemsDownloads>();

            if (!confirmStringContainsLanguageDownloadsDelegate.Confirm(data)) return gameDetails;
            var downloadStrings = itemizeGameDetailsDownloadsDelegate.Itemize(data);

            foreach (var downloadString in downloadStrings)
            {
                var downloadLanguages = itemizeGameDetailsDownloadLanguagesDelegate.Itemize(downloadString);
                if (downloadLanguages == null)
                    throw new InvalidOperationException("Cannot find any download languages or download language format changed.");

                // ... and remove download lanugage strings from downloads
                var downloadsStringSansLanguages = replaceMultipleStringsDelegate.ReplaceMultiple(
                    downloadString,
                    string.Empty,
                    downloadLanguages.ToArray());

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    serializationController.Deserialize<OperatingSystemsDownloads[][]>(
                    downloadsStringSansLanguages);

                // and convert GOG two-dimensional array of downloads to single-dimensional array
                var languageDownloads = convert2DArrayToArrayDelegate.Convert(downloads);

                if (languageDownloads.Count() != downloadLanguages.Count())
                    throw new InvalidOperationException("Number of extracted language downloads doesn't match number of languages.");

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
