using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Destination.Filename;
using Interfaces.Settings;
using Interfaces.SerializedStorage;

namespace Controllers.Settings
{
    public class SettingsController : ILoadDelegate, ISettingsProperty
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
    }
}
