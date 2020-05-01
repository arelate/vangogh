using System.Text.RegularExpressions;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Replace;
using Attributes;
using Delegates.Replace;

namespace GOG.Delegates.Convert.ProductTypes
{
    public class ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate : IConvertDelegate<string, string>
    {
        private readonly IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        [Dependencies(
            typeof(ReplaceMultipleStringsDelegate))]
        public ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate(
            IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate)
        {
            this.replaceMultipleStringsDelegate = replaceMultipleStringsDelegate;
        }

        public string Convert(string downloadLanguage)
        {
            downloadLanguage = replaceMultipleStringsDelegate.ReplaceMultiple(
                downloadLanguage,
                string.Empty,
                new string[] {"\"", ","});

            downloadLanguage = Regex.Unescape(downloadLanguage);

            return downloadLanguage;
        }
    }
}