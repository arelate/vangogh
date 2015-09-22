using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GOG.Interfaces;
using GOG.SharedModels;

namespace GOG.SharedControllers
{
    public class SettingsController: ISettingsController<Settings>
    {
        private IStreamReadableController streamReadableController;
        private ISerializationController<string> serializationController;
        private IConsoleController consoleController;

        public SettingsController(
            IStreamReadableController streamReadableController,
            ISerializationController<string> serializationContoller,
            IConsoleController consoleController)
        {
            this.streamReadableController = streamReadableController;
            this.serializationController = serializationContoller;
            this.consoleController = consoleController;
        }

        public async Task<Settings> Load()
        {
            string filename = "settings.json";
            Settings settings = null;

            try
            {
                using (Stream streamReadable = streamReadableController.OpenReadable(filename))
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

        public async Task Save(Settings data)
        {
            // no-op
        }
    }
}
