using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Settings;
using Interfaces.SerializedStorage;

namespace Controllers.Settings
{
    public class SettingsController : ILoadDelegate, ISettingsProperty
    {
        private string filename;
        private ISerializedStorageController serializedStorageController;

        public ISettings Settings { get; private set; }

        public SettingsController(
            string filename,
            ISerializedStorageController serializedStorageController)
        {
            this.filename = filename;
            this.serializedStorageController = serializedStorageController;
        }

        public async Task LoadAsync()
        {
            Settings = await serializedStorageController.DeserializePullAsync<Models.Settings.Settings>(filename);

            // set defaults

            if (Settings == null) Settings = new Models.Settings.Settings();

            if (Settings.UpdateData == null) Settings.UpdateData = new string[0];
            if (Settings.UpdateDownloads == null) Settings.UpdateDownloads = new string[0];
            if (Settings.DownloadsLanguages == null) Settings.DownloadsLanguages = new string[0];
            if (Settings.DownloadsOperatingSystems == null) Settings.DownloadsOperatingSystems = new string[0];
            if (Settings.ProcessDownloads == null) Settings.ProcessDownloads = new string[0];
            if (Settings.Cleanup == null) Settings.Cleanup = new string[0];
        }
    }
}
