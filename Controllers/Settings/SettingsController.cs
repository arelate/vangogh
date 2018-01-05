using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Settings;
using Interfaces.SerializedStorage;
using Interfaces.Status;

namespace Controllers.Settings
{
    public class SettingsController :
        ISettingsController,
        IGetDirectoryDelegate
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private ISerializedStorageController serializedStorageController;
        private IStatusController statusController;

        private ISettings settings;

        public SettingsController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController)
        {
            this.getFilenameDelegate = getFilenameDelegate;
            this.serializedStorageController = serializedStorageController;
            this.statusController = statusController;
        }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task LoadAsync(IStatus status = null)
        {
            var loadStatus = await statusController.CreateAsync(status, "Load settings");

            settings = await serializedStorageController.DeserializePullAsync<
                Models.Settings.Settings>(
                getFilenameDelegate.GetFilename(),
                loadStatus);

            // set defaults

            if (settings == null) settings = new Models.Settings.Settings();

            if (settings.DownloadsLanguages == null)
                settings.DownloadsLanguages = new string[0];
            if (settings.DownloadsOperatingSystems == null)
                settings.DownloadsOperatingSystems = new string[0];
            if (settings.Directories == null)
                settings.Directories = new Dictionary<string, string>();

            DataAvailable = true;

            await statusController.CompleteAsync(loadStatus);
        }

        public string GetDirectory(string directory = null)
        {
            if (!DataAvailable) LoadAsync().Wait();

            if (settings.Directories == null ||
                settings.Directories.Count == 0) return string.Empty;

            if (settings.Directories.ContainsKey(directory))
                return settings.Directories[directory];

            return string.Empty;
        }

        public async Task<ISettings> GetSettingsAsync(IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return settings;
        }
    }
}
