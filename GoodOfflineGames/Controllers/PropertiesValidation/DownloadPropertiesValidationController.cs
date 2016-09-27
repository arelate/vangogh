using Interfaces.PropertiesValidation;
using Interfaces.Settings;
using Interfaces.Language;

namespace Controllers.PropertiesValidation
{
    public class DownloadPropertiesValidationController: IDownloadPropertiesValidationController
    {
        private ILanguageController languageController;

        private string[] defaultOperatingSystems = new string[1] { "Windows" };
        private string[] defaultLanguages = new string[1] { "en" };

        public DownloadPropertiesValidationController(ILanguageController languageController)
        {
            this.languageController = languageController;
        }

        public IDownloadProperties ValidateProperties(IDownloadProperties downloadProperties)
        {
            if (downloadProperties == null)
                downloadProperties = new Models.Settings.DownloadProperties();

            if (downloadProperties.Languages == null ||
                downloadProperties.Languages.Length == 0)
            {
                downloadProperties.Languages = defaultLanguages;
            }
            else
            {
                // validate that download languages are actually codes and not language names
                for (var ii = 0; ii < downloadProperties.Languages.Length; ii++)
                {
                    var languageOrLanguageCode = downloadProperties.Languages[ii];
                    if (languageController.IsLanguageCode(languageOrLanguageCode)) continue;
                    else
                    {
                        var code = languageController.GetLanguageCode(languageOrLanguageCode);
                        if (!string.IsNullOrEmpty(code))
                            downloadProperties.Languages[ii] = code;
                    }
                }
            }

            if (downloadProperties.OperatingSystems == null ||
                downloadProperties.OperatingSystems.Length == 0)
            {
                downloadProperties.OperatingSystems = defaultOperatingSystems;
            }

            return downloadProperties;
        }
    }
}
