using System.Text.RegularExpressions;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Replace;


using Attributes;

namespace GOG.Delegates.Convert.ProductTypes
{
    public class ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate : IConvertDelegate<string, string>
    {
        readonly IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        [Dependencies(
            "Delegates.Replace.ReplaceMultipleStringsDelegate,Delegates")]
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
                new string[] { "\"", "," });

            downloadLanguage = Regex.Unescape(downloadLanguage);

            return downloadLanguage;
        }
    }
}
