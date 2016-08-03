using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Console;
using Interfaces.IO.Storage;
using Interfaces.Serialization;
using Interfaces.Language;
using Interfaces.Settings;
using Models.Settings;

namespace Controllers.Settings
{
    public class SettingsController : ISettingsController<Models.Settings.Settings>
    {
        //private IOpenReadableDelegate openReadableDelegate;
        private IStorageController<string> storageController;
        private ISerializationController<string> serializationController;
        private IConsoleController consoleController;
        private ILanguageController languageController;

        private string[] defaultOperatingSystems = new string[1] { "Windows" };
        private string[] defaultLanguages = new string[1] { "en" };

        public SettingsController(
            //IOpenReadableDelegate openReadableDelegate,
            IStorageController<string> storageController,
            ISerializationController<string> serializationController,
            ILanguageController languageController,
            IConsoleController consoleController)
        {
            //this.openReadableDelegate = openReadableDelegate;
            this.storageController = storageController;
            this.serializationController = serializationController;
            this.consoleController = consoleController;
            this.languageController = languageController;
        }

        public async Task<Models.Settings.Settings> Load()
        {
            string filename = "settings.json";
            Models.Settings.Settings settings = null;

            try
            {
                var settingsContent = await storageController.Pull(filename);
                settings = serializationController.Deserialize<Models.Settings.Settings>(settingsContent);
            }
            catch
            {
                settings = new Models.Settings.Settings();
            }

            // initialize structures if not specified in the settings.json file
            if (settings.Authenticate == null) settings.Authenticate = new SettingsAuthenticate();
            if (settings.Update == null) settings.Update = new SettingsUpdate();
            if (settings.Download == null) settings.Download = new SettingsDownload();
            if (settings.Validate == null) settings.Validate = new SettingsValidate();
            if (settings.Cleanup == null) settings.Cleanup = new SettingsCleanup();

            // request additional data if needed
            if (string.IsNullOrEmpty(settings.Authenticate.Username))
            {
                consoleController.WriteLine("Please enter your GOG.com username (email):");
                settings.Authenticate.Username = consoleController.ReadLine();
            }

            if (string.IsNullOrEmpty(settings.Authenticate.Password))
            {
                consoleController.WriteLine("Please enter password for {0}:", MessageType.Default, settings.Authenticate.Username);
                settings.Authenticate.Password = consoleController.PrivateReadLine();
            }

            if (settings.Download.Languages == null ||
                settings.Download.Languages.Length == 0)
            {
                settings.Download.Languages = defaultLanguages;
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
                settings.Download.OperatingSystems = defaultOperatingSystems;
            }

            return settings;
        }
    }
}
