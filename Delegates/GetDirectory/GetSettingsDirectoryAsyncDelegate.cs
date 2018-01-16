using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Stash;

using Interfaces.Status;

using Interfaces.Models.Settings;

namespace Delegates.GetDirectory
{
    public class GetSettingsDirectoryAsyncDelegate: IGetDirectoryAsyncDelegate
    {
        private string settingsDirectoryKey;
        private IStashController<ISettings, Models.Settings.Settings> settingsStashController;

        public GetSettingsDirectoryAsyncDelegate(
            string settingsDirectoryKey,
            IStashController<ISettings, Models.Settings.Settings> settingsStashController)
        {
            this.settingsDirectoryKey = settingsDirectoryKey;
            this.settingsStashController = settingsStashController;
        }

        public async Task<string> GetDirectoryAsync(string directory, IStatus status)
        {
            var settings = await settingsStashController.GetDataAsync(status);

            if (settings.Directories == null ||
                settings.Directories.Count == 0) return directory;

            if (settings.Directories.ContainsKey(settingsDirectoryKey))
                return settings.Directories[settingsDirectoryKey];

            return directory;
        }
    }
}
