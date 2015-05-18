using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GOG
{
    class SettingsController
    {
        public static async Task<Settings> LoadSettings(
            IConsoleController consoleController,
            IStreamController streamController)
        {
            string filename = "settings.json";
            Settings settings = null;

            try
            {
                using (Stream streamReadable = streamController.OpenReadable(filename))
                {
                    using (StreamReader streamReader = new StreamReader(streamReadable, Encoding.UTF8))
                    {
                        string settingsString = await streamReader.ReadToEndAsync();
                        settings = JSONController.Parse<Settings>(settingsString);
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
                consoleController.WriteLine("Please enter your GOG.com username (email):");
                settings.Username = consoleController.ReadLine();
            }

            if (string.IsNullOrEmpty(settings.Password))
            {
                consoleController.WriteLine("Please enter password for {0}:", settings.Username);
                settings.Password = consoleController.ReadPrivateLine();
            }

            return settings;
        }
    }
}
