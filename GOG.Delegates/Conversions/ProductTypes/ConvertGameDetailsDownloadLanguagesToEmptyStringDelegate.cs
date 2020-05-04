using System.Text.RegularExpressions;
using Attributes;
using Delegates.Conversions.Strings;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Conversions.ProductTypes
{
    public class ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate : IConvertDelegate<string, string>
    {
        private readonly IConvertDelegate<(string, string[]), string> 
            convertStringToReplaceMarkersWithEmptyStringDelegate;

        [Dependencies(
            typeof(ConvertStringToReplaceMarkersWithEmptyStringDelegate))]
        public ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate(
            IConvertDelegate<(string, string[]), string> 
                convertStringToReplaceMarkersWithEmptyStringDelegate)
        {
            this.convertStringToReplaceMarkersWithEmptyStringDelegate = 
                convertStringToReplaceMarkersWithEmptyStringDelegate;
        }

        public string Convert(string downloadLanguage)
        {
            downloadLanguage = convertStringToReplaceMarkersWithEmptyStringDelegate.Convert(
                (downloadLanguage,
                new string[] {"\"", ","}));

            downloadLanguage = Regex.Unescape(downloadLanguage);

            return downloadLanguage;
        }
    }
}