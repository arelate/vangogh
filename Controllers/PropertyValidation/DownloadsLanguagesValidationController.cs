using Interfaces.PropertyValidation;
using Interfaces.Language;

namespace Controllers.PropertyValidation
{
    public class DownloadsLanguagesValidationDelegate : IValidatePropertiesDelegate<string[]>
    {
        private ILanguageController languageController;
        private string[] defaultLanguages = new string[1] { "en" };

        public DownloadsLanguagesValidationDelegate(ILanguageController languageController)
        {
            this.languageController = languageController;
        }

        public string[] ValidateProperties(string[] downloadsLanguages)
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
        }
    }
}
