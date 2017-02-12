using System.Threading.Tasks;

using Interfaces.Storage;
using Interfaces.Serialization;
using Interfaces.Settings;

namespace Controllers.Settings
{
    public class SettingsController : ISettingsController<Models.Settings.Settings>
    {
        private IStorageController<string> storageController;
        private ISerializationController<string> serializationController;

        public SettingsController(
            IStorageController<string> storageController,
            ISerializationController<string> serializationController)
        {
            this.storageController = storageController;
            this.serializationController = serializationController;
        }

        public async Task<Models.Settings.Settings> Load()
        {
            string filename = "settings.json";
            Models.Settings.Settings settings = null;

            var settingsContent = await storageController.PullAsync(filename);
            settings = serializationController.Deserialize<Models.Settings.Settings>(settingsContent);

            return settings;
        }
    }
}
