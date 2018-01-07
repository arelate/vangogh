using System.Text.RegularExpressions;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.Replace;

namespace GOG.Delegates.Format
{
    public class FormatDownloadLanguageDelegate : IFormatDelegate<string, string>
    {
        private IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;

        public FormatDownloadLanguageDelegate(IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate)
        {
            this.replaceMultipleStringsDelegate = replaceMultipleStringsDelegate;
        }

        public string Format(string downloadLanguage)
        {
            downloadLanguage = replaceMultipleStringsDelegate.ReplaceMultiple(
                downloadLanguage,
                string.Empty,
                new string[2] { "\"", "," });

            downloadLanguage = Regex.Unescape(downloadLanguage);

            return downloadLanguage;
        }
    }
}
