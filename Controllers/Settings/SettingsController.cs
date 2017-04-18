using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Settings;
using Interfaces.SerializedStorage;
using System;

namespace Controllers.Settings
{
    public class SettingsController : 
        ILoadAsyncDelegate, 
        ISettingsProperty,
        IGetDirectoryDelegate
    {
        private IGetFilenameDelegate getFilenameDelegate;
        private ISerializedStorageController serializedStorageController;

        public ISettings Settings { get; private set; }

        public SettingsController(
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController)
        {
            this.getFilenameDelegate = getFilenameDelegate;
            this.serializedStorageController = serializedStorageController;
        }

        public async Task LoadAsync()
        {
            Settings = await serializedStorageController.DeserializePullAsync<
                Models.Settings.Settings>(
                getFilenameDelegate.GetFilename());

            // set defaults

            if (Settings == null) Settings =
                new Models.Settings.Settings();

            if (Settings.DownloadsLanguages == null)
                Settings.DownloadsLanguages = new string[0];
            if (Settings.DownloadsOperatingSystems == null)
                Settings.DownloadsOperatingSystems = new string[0];
        }

        public string GetDirectory(string directory = null)
        {
            if (Settings.Directories == null ||
                Settings.Directories.Count == 0) return string.Empty;

            if (Settings.Directories.ContainsKey(directory))
                return Settings.Directories[directory];

            return string.Empty;
        }
    }
}
