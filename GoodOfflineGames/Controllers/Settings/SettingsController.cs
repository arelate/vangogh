using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Console;
using Interfaces.IO.Stream;
using Interfaces.Serialization;
using Interfaces.Language;
using Interfaces.Settings;
using Models.Settings;

namespace Controllers
{
    public class SettingsController : ISettingsController<Settings>
    {
        private IOpenReadableDelegate openReadableDelegate;
        private ISerializationController<string> serializationController;
        private IConsoleController consoleController;
        private ILanguageController languageController;

        public SettingsController(
            IOpenReadableDelegate openReadableDelegate,
            ISerializationController<string> serializationController,
            ILanguageController languageController,
            IConsoleController consoleController)
        {
            this.openReadableDelegate = openReadableDelegate;
            this.serializationController = serializationController;
            this.consoleController = consoleController;
            this.languageController = languageController;
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

            // initialize structures if not speicifed in the settings.json file
            if (settings.Authenticate == null) settings.Authenticate = new SettingsAuthenticate();
            if (settings.Update == null) settings.Update = new SettingsUpdate();
            if (settings.Download == null) settings.Download = new SettingsDownload();
            if (settings.Validate == null) settings.Validate = new SettingsValidate();
            if (settings.Cleanup == null) settings.Cleanup = new SettingsCleanup();

            // request additional data if needed
            if (string.IsNullOrEmpty(settings.Authenticate.Username))
            {
                consoleController.WriteLine("Please enter your GOG.com username (email):", MessageType.Default);
                settings.Authenticate.Username = consoleController.ReadLine();
            }

            if (string.IsNullOrEmpty(settings.Authenticate.Password))
            {
                consoleController.WriteLine("Please enter password for {0}:", MessageType.Default, settings.Authenticate.Username);
                settings.Authenticate.Password = consoleController.ReadPrivateLine();
            }

            if (settings.Download.Languages == null ||
                settings.Download.Languages.Length == 0)
            {
                settings.Download.Languages = new string[1] { "en" };
            }
            else
            {
                // validate that download languages are actually codes and not language names
                for (var ii = 0; ii < settings.Download.Languages.Length; ii++)
                {
                    var languageOrLanguageCode = settings.Download.Languages[ii];
                    if (languageController.IsLanguageCode(languageOrLanguageCode)) continue;
                    else
                    {
                        var code = languageController.GetLanguageCode(languageOrLanguageCode);
                        if (!string.IsNullOrEmpty(code))
                            settings.Download.Languages[ii] = code;
                    }
                }
            }

            if (settings.Download.OperatingSystems == null ||
                settings.Download.OperatingSystems.Length == 0)
            {
                settings.Download.OperatingSystems = new string[1] { "Windows" };
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
