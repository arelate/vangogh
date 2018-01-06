using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Language;

namespace Delegates.Correct
{
    public class CorrectDownloadsLanguagesAsyncDelegate : ICorrectAsyncDelegate<string[]>
    {
        private ILanguageController languageController;
        private string[] defaultLanguages = new string[1] { "en" };

        public CorrectDownloadsLanguagesAsyncDelegate(ILanguageController languageController)
        {
            this.languageController = languageController;
        }

        public async Task<string[]> CorrectAsync(string[] downloadsLanguages)
        {
            return await Task.Run(() =>
            {
                if (downloadsLanguages == null ||
                    downloadsLanguages.Length == 0)
                    downloadsLanguages = defaultLanguages;

                // validate that download languages are actually codes and not language names
                for (var ii = 0; ii < downloadsLanguages.Length; ii++)
                {
                    var languageOrLanguageCode = downloadsLanguages[ii];
                    if (languageController.IsLanguageCode(languageOrLanguageCode)) continue;
                    else
                    {
                        var code = languageController.GetLanguageCode(languageOrLanguageCode);
                        if (!string.IsNullOrEmpty(code))
                            downloadsLanguages[ii] = code;
                    }
                }

                return downloadsLanguages;
            });
        }
    }
}
