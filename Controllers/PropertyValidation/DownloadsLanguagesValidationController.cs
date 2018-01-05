using System.Threading.Tasks;

using Interfaces.PropertyValidation;
using Interfaces.Language;

namespace Controllers.PropertyValidation
{
    public class DownloadsLanguagesValidationDelegate : IValidatePropertiesAsyncDelegate<string[]>
    {
        private ILanguageController languageController;
        private string[] defaultLanguages = new string[1] { "en" };

        public DownloadsLanguagesValidationDelegate(ILanguageController languageController)
        {
            this.languageController = languageController;
        }

        public async Task<string[]> ValidatePropertiesAsync(string[] downloadsLanguages)
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
