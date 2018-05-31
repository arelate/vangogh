using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Language;

using Interfaces.Models.Settings;

using Interfaces.Status;

using Models.Settings;

namespace Delegates.Correct
{
    public class CorrectSettingsDownloadsLanguagesAsyncDelegate : ICorrectAsyncDelegate<Settings>
    {
        readonly ILanguageController languageController;
        readonly string[] defaultLanguages = { "en" };

        public CorrectSettingsDownloadsLanguagesAsyncDelegate(ILanguageController languageController)
        {
            this.languageController = languageController;
        }

        public async Task<Settings> CorrectAsync(Settings settings, IStatus status)
        {
            return await Task.Run(() =>
            {
                if (settings == null)
                    throw new System.ArgumentNullException();

                if (settings.DownloadsLanguages == null ||
                    settings.DownloadsLanguages.Length == 0)
                    settings.DownloadsLanguages = defaultLanguages;

                // validate that download languages are actually codes and not language names
                for (var ii = 0; ii < settings.DownloadsLanguages.Length; ii++)
                {
                    var languageOrLanguageCode = settings.DownloadsLanguages[ii];
                    if (languageController.IsLanguageCode(languageOrLanguageCode)) continue;

                    var code = languageController.GetLanguageCode(languageOrLanguageCode);
                    if (!string.IsNullOrEmpty(code))
                        settings.DownloadsLanguages[ii] = code;
                }

                return settings;
            });
        }
    }
}
