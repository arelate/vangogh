using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GOG.Interfaces;
using GOG.SharedModels;

namespace GOG.SharedControllers
{
    public class SettingsController : ISettingsController<Settings>
    {
        private IOpenReadableDelegate openReadableDelegate;
        private ISerializationController<string> serializationController;
        private IConsoleController consoleController;
        private ILanguageCodesController languageCodesController;

        public SettingsController(
            IOpenReadableDelegate openReadableDelegate,
            ISerializationController<string> serializationController,
            ILanguageCodesController languageCodesController,
            IConsoleController consoleController)
        {
            this.openReadableDelegate = openReadableDelegate;
            this.serializationController = serializationController;
            this.consoleController = consoleController;
            this.languageCodesController = languageCodesController;
        }

        public async Task<Settings> Load()
        {
            string filename = "settings.json";
            Settings settings = null;

            try
            {
                using (Stream streamReadable = openReadableDelegate.OpenReadable(filename))
                {
                    using (StreamReader streamReader = new StreamReader(streamReadable, Encoding.UTF8))
                    {
                        string settingsString = await streamReader.ReadToEndAsync();
                        settings = serializationController.Deserialize<Settings>(settingsString);
                    }
                }
            }
            catch (Exception)
            {
                settings = new Settings();
            }

            // request additional data if needed
            if (string.IsNullOrEmpty(settings.Username))
            {
                consoleController.WriteLine("Please enter your GOG.com username (email):", ConsoleColor.White);
                settings.Username = consoleController.ReadLine();
            }

            if (string.IsNullOrEmpty(settings.Password))
            {
                consoleController.WriteLine("Please enter password for {0}:", ConsoleColor.White, settings.Username);
                settings.Password = consoleController.ReadPrivateLine();
            }

            if (settings.DownloadLanguageCodes == null ||
                settings.DownloadLanguageCodes.Length == 0)
            {
                settings.DownloadLanguageCodes = new string[1] { "en" };
            }
            else
            {
                // validate that download languages are actually codes and not language names
                for (var ii = 0; ii < settings.DownloadLanguageCodes.Length; ii++)
                {
                    var languageOrLanguageCode = settings.DownloadLanguageCodes[ii];
                    if (languageCodesController.IsLanguageCode(languageOrLanguageCode)) continue;
                    else
                    {
                        var code = languageCodesController.GetLanguageCode(languageOrLanguageCode);
                        if (!string.IsNullOrEmpty(code))
                            settings.DownloadLanguageCodes[ii] = code;
                    }
                }
            }

            if (settings.DownloadOperatingSystems == null ||
                settings.DownloadOperatingSystems.Length == 0)
            {
                settings.DownloadOperatingSystems = new string[1] { "Windows" };
            }

            return settings;
        }

        public Task Save(Settings data)
        {
            // no-op
            throw new NotImplementedException();
        }
    }
}
